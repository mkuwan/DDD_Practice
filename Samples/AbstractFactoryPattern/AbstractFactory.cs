using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.AbstractFactoryPattern
{
    public abstract class AbstractFactory<TI, TC> where TC: TI, new()
    {
        public virtual TC Instance => new TC();

    }


}
