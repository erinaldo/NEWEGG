using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_DelvTrackRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_DelvTrackRepoAdapters
{
    public class Seller_DelvTrackRepoAdapters : ISeller_DelvTrackRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_DelvTrack> _Seller_DelvTrack;

        public Seller_DelvTrackRepoAdapters(ITWSELLERPORTALDBRepository<Seller_DelvTrack> seller_DelvTrack)
        {
            this._Seller_DelvTrack = seller_DelvTrack;
        }

        public void Create(Seller_DelvTrack model)
        {
            this._Seller_DelvTrack.Create(model);
        }

        public IQueryable<Seller_DelvTrack> GetAll()
        {
            return this._Seller_DelvTrack.GetAll();
        }

        public void Update(Seller_DelvTrack model)
        {
            this._Seller_DelvTrack.Update(model);
        }
        
    }
}
