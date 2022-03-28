using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleConsole.Domain.SampleModel;

namespace SampleConsole.Application.Services
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerById(Guid id);
        Task<List<Customer>> GetCustomers();
        Task<Customer> GetCustomerByName(string name);
        void Update(Customer customer);
        void Delete(Guid id);
    }
}
