using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.StoreModel;

namespace Domain.Models.CustomerModel.ValueObjects
{
    public record CartItem(Product Product, int Quantity);
}
