using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleConsole.Domain.Repositories;
using SampleConsole.Domain.SampleModel;

namespace SampleConsole.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;


        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer> GetCustomerById(Guid id)
        {
            return await _customerRepository.GetCustomerById(id);
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await _customerRepository.GetCustomers();
        }

        public async Task<Customer> GetCustomerByName(string name)
        {
            return await _customerRepository.GetCustomerByName(name);
        }

        public void Update(Customer customer)
        {
            _customerRepository.Update(customer);
        }

        public void Delete(Guid id)
        {
            _customerRepository.Delete(id);
        }
    }
}
