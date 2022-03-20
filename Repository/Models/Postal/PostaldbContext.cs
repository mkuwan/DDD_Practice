using System;
using System.Collections.Generic;
using Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Repository.Mediator;

namespace Repository.Models.Postal
{

    public partial class PostaldbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;


        public PostaldbContext(DbContextOptions<PostaldbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public virtual DbSet<KenAll> KenAlls { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
////#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Data Source=localhost,1433;Initial Catalog=postaldb;Persist Security Info=False;User ID=sa;Password=aspMVC98;Pooling=False;MultipleActiveResultSets=True;Encrypt=False;TrustServerCertificate=False");
//            }
//        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<KenAll>(entity =>
        //    {
        //        entity.ToTable("KenAll");

        //        entity.Property(e => e.Municipality)
        //            .HasMaxLength(50)
        //            .IsFixedLength();

        //        entity.Property(e => e.MunicipalityKana)
        //            .HasMaxLength(100)
        //            .IsFixedLength();

        //        entity.Property(e => e.PostalCode)
        //            .HasMaxLength(10)
        //            .IsFixedLength();

        //        entity.Property(e => e.Prefecture)
        //            .HasMaxLength(20)
        //            .IsFixedLength();

        //        entity.Property(e => e.PrefectureKana)
        //            .HasMaxLength(50)
        //            .IsFixedLength();

        //        entity.Property(e => e.TownArea)
        //            .HasMaxLength(255)
        //            .IsFixedLength();

        //        entity.Property(e => e.TownAreaKana)
        //            .HasMaxLength(100)
        //            .IsFixedLength();
        //    });

        //    OnModelCreatingPartial(modelBuilder);
        //}

        /// <summary>
        /// IUnitOfWork を使用した保存処理
        /// ドメインイベントの呼び出しを直前でまとめて行うことができます
        /// https://docs.microsoft.com/ja-jp/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation#implement-domain-events
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DbContextDispatchDomainEventAsync(this);

            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
