using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.StoreModel;

namespace Domain.Services
{
    public class StoreService: IStoreService
    {
        public async Task<bool> HasEnoughInventoryAsync(Product product, int quantity)
        {
            return await GetInventoryAsync(product) >= quantity;
        }

        public async Task RemoveInventoryAsync(Product product, int quantity)
        {
            if (!await HasEnoughInventoryAsync(product, quantity))
            {
                throw new Exception("在庫なし");
            }

            // todo: データベースに商品在庫削除処理

            throw new NotImplementedException();
        }

        public async Task AddInventoryAsync(Product product, int quantity)
        {
            // todo: もし商品が存在しない場合は商品を登録する

            // todo: データベースに商品在庫追加処理

            throw new NotImplementedException();
        }

        public async Task<int> GetInventoryAsync(Product product)
        {
            throw new NotImplementedException();

            // todo: データベースから商品在庫取得
        }
    }
}
