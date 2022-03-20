using Domain.Models.StoreModel;
using Domain.Services;
using Domain.Models.CustomerModel;

namespace AngularBackend.Services.Customer
{
    public class CustomerService
    {
        private readonly IStoreService _storeService;

        public CustomerService(IStoreService storeService)
        {
            _storeService = storeService;
        }



    }
}
