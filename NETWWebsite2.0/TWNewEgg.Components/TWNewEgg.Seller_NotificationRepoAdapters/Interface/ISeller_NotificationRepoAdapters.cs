using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_NotificationRepoAdapters.Interface
{
    public interface ISeller_NotificationRepoAdapters
    {
        void Create(Seller_Notification model);
        IQueryable<Seller_Notification> GetAll();
        void Update(Seller_Notification model);
    }
}
