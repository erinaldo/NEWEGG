using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemTempRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemTempRepoAdapters
{
    public class ItemTempRepoAdapter : TWNewEgg.ItemTempRepoAdapters.Interface.IItemTempRepoAdapter
    {

        private IRepository<ItemTemp> itemTemp;

        public ItemTempRepoAdapter(IRepository<ItemTemp> itemTemp)
        {
            this.itemTemp = itemTemp;
        }

        public IQueryable<Models.DBModels.TWSQLDB.ItemTemp> GetAll()
        {
            return this.itemTemp.GetAll();
        }

        public IQueryable<Models.DBModels.TWSQLDB.ItemTemp> GetById(int argTemp_ID)
        {
            return this.itemTemp.GetAll().Where(x => x.ID == argTemp_ID);
        }

        public Models.DBModels.TWSQLDB.ItemTemp Create(Models.DBModels.TWSQLDB.ItemTemp argItemTemp)
        {
            try
            {
                this.itemTemp.Create(argItemTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return argItemTemp;
        }

        public Models.DBModels.TWSQLDB.ItemTemp Update(Models.DBModels.TWSQLDB.ItemTemp argItemTemp)
        {
            try
            {
                this.itemTemp.Update(argItemTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return argItemTemp;
        }

        public bool Delete(int argItemTempID, string UpdateUser)
        {
            bool delete = false;
            try
            {
                ItemTemp itemTemp = this.itemTemp.GetAll().Where(x => x.ID == argItemTempID).FirstOrDefault();

                itemTemp.ItemStatus = 99;
                itemTemp.UpdateDate = DateTime.Now;
                itemTemp.UpdateUser = UpdateUser;
                this.itemTemp.Update(itemTemp);
                delete = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return delete;
        }

        public bool Delete(List<int> argItemTempID, string UpdateUser)
        {
            bool deleteList = false;
            try
            {
                List<ItemTemp> itemTempList = this.itemTemp.GetAll().Where(x => argItemTempID.Contains(x.ID)).ToList();

                itemTempList.ForEach(x =>
                {
                    this.Delete(x.ID, UpdateUser);
                });

                deleteList = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return deleteList;
        }
    }
  
}
