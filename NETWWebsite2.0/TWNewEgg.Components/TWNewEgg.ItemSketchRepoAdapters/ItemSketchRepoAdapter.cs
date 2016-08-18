using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemSketchRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemSketchRepoAdapters
{
    public class ItemSketchRepoAdapter : TWNewEgg.ItemSketchRepoAdapters.Interface.IItemSketchRepoAdapter
    {

        private IRepository<ItemSketch> itemSketch;

        public ItemSketchRepoAdapter(IRepository<ItemSketch> itemSketch)
        {
            this.itemSketch = itemSketch;
        }

        public IQueryable<Models.DBModels.TWSQLDB.ItemSketch> GetAll()
        {
            return this.itemSketch.GetAll();
        }

        public IQueryable<Models.DBModels.TWSQLDB.ItemSketch> GetById(int argSketch_ID)
        {
            return this.itemSketch.GetAll().Where(x=>x.ID==argSketch_ID);
        }

        public Models.DBModels.TWSQLDB.ItemSketch Create(Models.DBModels.TWSQLDB.ItemSketch argItemSketch)
        {
            try
            {
                this.itemSketch.Create(argItemSketch);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return argItemSketch;
        }

        public Models.DBModels.TWSQLDB.ItemSketch Update(Models.DBModels.TWSQLDB.ItemSketch argItemSketch)
        {
            try
            {
                this.itemSketch.Update(argItemSketch);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return argItemSketch;
        }

        public bool Delete(int argItemSketchID, string UpdateUserID)
        {
            bool delete = false;
            try
            {
                ItemSketch getItemSketch = this.GetById(argItemSketchID).FirstOrDefault();
                getItemSketch.Status = 99;
                getItemSketch.UpdateUser = UpdateUserID;
                getItemSketch.UpdateDate = DateTime.Now;
                this.itemSketch.Update(getItemSketch);
                delete = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return delete;
        }

        public bool Delete(List<int> argItemSketchID, string UpdateUserID)
        {
            bool deleteList = false;
            try
            {
                List<ItemSketch> itemSketchList = this.itemSketch.GetAll().Where(x => argItemSketchID.Contains(x.ID)).ToList();

                itemSketchList.ForEach(x => {
                    this.Delete(x.ID, UpdateUserID);
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
