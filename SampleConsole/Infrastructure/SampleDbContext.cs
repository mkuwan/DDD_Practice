using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SampleConsole.Domain.SampleModel;
using SampleConsole.Domain.SeedWork;
using SampleConsole.Infrastructure.EntityConfigurations;
using SampleConsole.Infrastructure.Mediator;

namespace SampleConsole.Infrastructure
{
    public class SampleDbContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "scheduling";
        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

        public DbSet<Schedule> Schedules { get; set; }

        public SampleDbContext(DbContextOptions<SampleDbContext> options): base(options) { }

        public SampleDbContext(DbContextOptions options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.ApplyConfiguration(new ScheduleEntityTypeConfiguration());

        //    //modelBuilder.Entity<Schedule>().HasData(
        //    //    new { ScheduleId = 1, ScheduleDate = new DateTime(2022,3,10,10,0,0), ScheduleContent = "打ち合わせ" },
        //    //    new { ScheduleId = 2, ScheduleDate = new DateTime(2022, 3, 15, 12, 0, 0), ScheduleContent = "昼食会" }
        //    //    );
        //}




        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;


        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DbContextDispatchDomainEventAsync(this);

            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        /// <summary>
        /// BeginTransaction
        /// </summary>
        /// <returns></returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        /// <summary>
        /// Commit
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Rollback
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
