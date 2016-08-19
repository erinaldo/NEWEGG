using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.CartLocalServices.Interface
{
    public interface ICartLocalServices
    {
        void InsertIntoLocalDB(TWNewEgg.Models.DomainModels.Cart.DomainInsertSOGroupOrderItem _data);
        //string InsertTest();
    }
}
