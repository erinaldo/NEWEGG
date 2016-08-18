using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_DelvTrackRepoAdapters.Interface
{
    public interface ISeller_DelvTrackRepoAdapters
    {
        void Create(Seller_DelvTrack model);
        IQueryable<Seller_DelvTrack> GetAll();
        void Update(Seller_DelvTrack model);
    }
}
