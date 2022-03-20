using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.CustomerModel;
using Domain.Models.StoreModel;

namespace Domain.Services
{
    public interface IStoreService
    {

        /// <summary>
        /// 指定数以上の商品在庫があるか確認
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<bool> HasEnoughInventoryAsync(Product product, int quantity);

        /// <summary>
        /// 在庫から商品を指定数減らす
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        Task RemoveInventoryAsync(Product product, int quantity);

        /// <summary>
        /// 商品を指定数追加する
        /// カートから商品を戻す場合にも利用
        /// 商品がない場合は、新規追加
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        Task AddInventoryAsync(Product product, int quantity);

        /// <summary>
        /// 商品在庫を取得する
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<int> GetInventoryAsync(Product product);
    }
}
