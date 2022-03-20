using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.CustomerModel.ValueObjects;
using Domain.Models.StoreModel;
using Domain.SeedWork;
using Domain.Services;
using Microsoft.Extensions.Logging;

namespace Domain.Models.CustomerModel
{
    /// <summary>
    /// Customer Domain Model
    /// </summary>
    public sealed class Customer: EntityStringId
    {
        private readonly ILogger<Customer> _logger;

        public Customer(string id, string customerName, CustomerAddress customerAddress)
        {
            Id = id;
            CustomerName = customerName;
            CustomerAddress = customerAddress;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddJsonConsole(options => options.IncludeScopes = true);
            });
            _logger = loggerFactory.CreateLogger<Customer>();
        }

        private readonly string _customerName = String.Empty;
        public string CustomerName
        {
            get => _customerName;
            init
            {
                if (String.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(_customerName), "顧客名が入力されていません");
                _customerName = value;
            }
        }

        public CustomerAddress CustomerAddress { get; private set; }

        public void UpdateCustomerAddress(CustomerAddress customerAddress)
        {
            CustomerAddress = new CustomerAddress
            (
                customerAddress.PostalCode, 
                customerAddress.Prefecture,
                customerAddress.Municipality, 
                customerAddress.TownArea
                );
        }


        // todo: カート関係の処理ををここに入れて, Customerドメインのユースケースとしていますが、カート関係の処理を別のサービスとして別にする方法も検討する
        public List<CartItem> Cart { get; private set; } = new();
        
        /// <summary>
        /// カートに商品を追加
        /// </summary>
        /// <param name="storeService"></param>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<bool> AddCartAsync(IStoreService storeService, Product product, int quantity)
        {
            _logger.LogInformation($"カートに商品を追加したよ {product.ProductName}を{quantity}個");

            if (!await storeService.HasEnoughInventoryAsync(product, quantity))
            {
                return false;
            }

            // ストアから商品を減らす
            await storeService.RemoveInventoryAsync(product, quantity);

            // カートから削除
            var cartItem = Cart.FirstOrDefault(x => x.Product.Id == product.Id);
            if(cartItem != null)
                Cart.Remove(cartItem);

            // カートに追加
            cartItem = new CartItem(product, quantity);
            Cart.Add(cartItem);

            return true;
        }

        /// <summary>
        /// カートをクリア
        /// </summary>
        /// <param name="storeService"></param>
        /// <returns></returns>
        public async Task ClearCartAsync(IStoreService storeService)
        {
            foreach (CartItem cartItem in Cart)
            {
                await storeService.AddInventoryAsync(cartItem.Product, cartItem.Quantity);
            }

            Cart.Clear();
        }
    }
}
