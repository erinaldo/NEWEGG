using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_BulletinsMessageRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_BulletinsMessageRepoAdapters
{
    public class Seller_BulletinsMessageRepoAdapters : ISeller_BulletinsMessageRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_BulletinsMessage> _seller_BulletinsMessage;

        public Seller_BulletinsMessageRepoAdapters(ITWSELLERPORTALDBRepository<Seller_BulletinsMessage> seller_BulletinsMessage)
        {
            this._seller_BulletinsMessage = seller_BulletinsMessage;
        }

        public void Create(Seller_BulletinsMessage model)
        {
            this._seller_BulletinsMessage.Create(model);
        }

        public IQueryable<Seller_BulletinsMessage> GetAll()
        {
            return this._seller_BulletinsMessage.GetAll();
        }

        public void Update(Seller_BulletinsMessage model)
        {
            this._seller_BulletinsMessage.Update(model);
        }
    }
}
