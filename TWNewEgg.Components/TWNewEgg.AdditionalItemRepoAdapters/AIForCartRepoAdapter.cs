using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.AdditionalItemRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.AdditionalItemRepoAdapters
{
    public class AIForCartRepoAdapter : IAIForCartRepoAdapter
    {
        private IRepository<AdditionalItemForCart> _AIForCart;

        public AIForCartRepoAdapter(IRepository<AdditionalItemForCart> aiForCart)
        {
            this._AIForCart = aiForCart;
        }

        public IQueryable<Models.DBModels.TWSQLDB.AdditionalItemForCart> GetAll()
        {
            return _AIForCart.GetAll();
        }

        public IQueryable<AdditionalItemForCart> GetAIByStatusSpecific(int statusInt = (int)AdditionalItemForCart.AdditionalItemStatus.Enable, int specificInt = (int)AdditionalItemForCart.SpecificStatus.AllAccount)
        {
            return this.GetAll().Where(x => x.Status == statusInt && x.Specific == specificInt);
        }

        public Models.DBModels.TWSQLDB.AdditionalItemForCart Add(Models.DBModels.TWSQLDB.AdditionalItemForCart createAI)
        {
            if (createAI == null)
            {
                return createAI;
            }
            _AIForCart.Create(createAI);
            return createAI;
        }

        public List<Models.DBModels.TWSQLDB.AdditionalItemForCart> AddList(List<Models.DBModels.TWSQLDB.AdditionalItemForCart> createAIs)
        {
            List<Models.DBModels.TWSQLDB.AdditionalItemForCart> addResults = new List<Models.DBModels.TWSQLDB.AdditionalItemForCart>();
            if (createAIs == null)
            {
                return addResults;
            }
            for (int i = 0; i < createAIs.Count; i++)
            {
                createAIs[i].CreateDate = DateTime.UtcNow.AddHours(8);
                _AIForCart.Create(createAIs[i]);
                addResults.Add(createAIs[i]);
            }
            return addResults;
        }

        public Models.DBModels.TWSQLDB.AdditionalItemForCart Update(Models.DBModels.TWSQLDB.AdditionalItemForCart updateAI)
        {
            if (updateAI == null)
            {
                return updateAI;
            }

            _AIForCart.Update(updateAI);
            return updateAI;
        }

        public void UpdateList(List<Models.DBModels.TWSQLDB.AdditionalItemForCart> updateDataCell)
        {
            if (updateDataCell != null && updateDataCell.Count >= 0)
            {
                foreach (AdditionalItemForCart additionalItem in updateDataCell)
                {
                    this.Update(additionalItem);
                }
            }
        }

        public void Delete(Models.DBModels.TWSQLDB.AdditionalItemForCart deleteAI)
        {
            throw new NotImplementedException();
        }

        public void DeleteList(List<Models.DBModels.TWSQLDB.AdditionalItemForCart> deleteAIs)
        {
            if (deleteAIs == null)
            {
                return;
            }
            for (int i = 0; i < deleteAIs.Count; i++)
            {
                _AIForCart.Delete(deleteAIs[i]);
            }
        }


        
    }
}
