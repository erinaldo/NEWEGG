using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_NotificationRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_NotificationRepoAdapters
{
    public class Seller_NotificationRepoAdapters : ISeller_NotificationRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_Notification> _seller_Notification;

        public Seller_NotificationRepoAdapters(ITWSELLERPORTALDBRepository<Seller_Notification> seller_Notification)
        {
            this._seller_Notification = seller_Notification;
        }

        public void Create(Seller_Notification model)
        {
            this._seller_Notification.Create(model);
        }

        public IQueryable<Seller_Notification> GetAll()
        {
             return this._seller_Notification.GetAll();
        }

        public void Update(Seller_Notification model)
        {
            this._seller_Notification.Update(model);
        }
    }
}
