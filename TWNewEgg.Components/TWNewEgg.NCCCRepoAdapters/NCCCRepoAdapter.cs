using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.NCCCRepoAdapters.Interface;

namespace TWNewEgg.NCCCRepoAdapters
{
    public class NCCCRepoAdapter : INCCCRepoAdapter
    {
        IRepository<NCCCTrans> _ncccTransRepo;
        public NCCCRepoAdapter(IRepository<NCCCTrans> ncccTransRepo)
        {
            this._ncccTransRepo = ncccTransRepo;
            Mapper.CreateMap<NCCCTrans, NCCCTrans>()
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                .ForMember(dest => dest.ID, opt => opt.Ignore());
        }

        public NCCCTrans Create(NCCCTrans trans)
        {
            trans.CreateDate = DateTime.Now;
            trans.CreateUser = "Sys";
            this._ncccTransRepo.Create(trans);
            return trans;
        }

        public NCCCTrans Update(NCCCTrans trans)
        {
            NCCCTrans origin = this._ncccTransRepo.Get(x => x.OrderID == trans.OrderID);
            ModelConverter.ConvertTo<NCCCTrans, NCCCTrans>(trans, origin);
            origin.UpdateDate = DateTime.Now;
            origin.UpdateUser = "Sys";
            this._ncccTransRepo.Update(origin);

            return origin;
        }

        public NCCCTrans Get(string orderId)
        {
            return this._ncccTransRepo.Get(x => x.OrderID == orderId);
        }
    }
}
