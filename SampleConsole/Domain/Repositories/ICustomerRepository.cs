using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleConsole.Domain.SampleModel;

namespace SampleConsole.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerById(Guid id);
        Task<List<Customer>> GetCustomers();
        Task<Customer> GetCustomerByName(string name);
        Task Update(Customer customer);
        Task Delete(Guid id);
    }
}
