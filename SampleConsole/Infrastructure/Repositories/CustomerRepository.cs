using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SampleConsole.Domain.Repositories;
using SampleConsole.Domain.SampleModel;

namespace SampleConsole.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SampleDbContext _context;

        public CustomerRepository(SampleDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetCustomerById(Guid id)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> GetCustomerByName(string name)
        {
            return await _context.Customers.Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public async Task Update(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
