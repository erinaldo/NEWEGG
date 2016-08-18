using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.AdditionalItemRepoAdapters.Interface
{
    public interface IAIForCartRepoAdapter
    {
        IQueryable<AdditionalItemForCart> GetAll();
        IQueryable<AdditionalItemForCart> GetAIByStatusSpecific(int statusInt = (int)AdditionalItemForCart.AdditionalItemStatus.Enable, int specificInt = (int)AdditionalItemForCart.SpecificStatus.AllAccount);
        AdditionalItemForCart Add(AdditionalItemForCart createAI);
        List<AdditionalItemForCart> AddList(List<AdditionalItemForCart> createAIs);
        AdditionalItemForCart Update(AdditionalItemForCart updateAI);
        void UpdateList(List<AdditionalItemForCart> updateDataCell);
        void Delete(AdditionalItemForCart deleteAI);
        void DeleteList(List<AdditionalItemForCart> deleteAIs);
    }
}
