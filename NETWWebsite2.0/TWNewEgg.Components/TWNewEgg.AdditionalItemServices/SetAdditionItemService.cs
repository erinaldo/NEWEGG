using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.AdditionalItemServices.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ItemSketchRepoAdapters.Interface;
using TWNewEgg.ItemTempRepoAdapters.Interface;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.AdditionalItemServices
{
    public class SetAdditionItemService : TWNewEgg.AdditionalItemServices.Interface.ISetAdditionalItem
    {
        private IItemSketchRepoAdapter _itemSketchRepo;
        private IItemRepoAdapter _itemRepo;
        private IItemTempRepoAdapter _itemTempRepo;

        public SetAdditionItemService(IItemRepoAdapter itemRepo,IItemSketchRepoAdapter itemSketchRepo,IItemTempRepoAdapter itemTempRepo)
        {
            this._itemRepo = itemRepo;
            this._itemSketchRepo = itemSketchRepo;
            this._itemTempRepo = itemTempRepo;
        }

        public bool EnableAdditionItemforItem(int argItemID, string updateUser)
        {
            bool updateResult = false;
            
            try
            {
                Item updateItem = this._itemRepo.GetAll().Where(x => x.ID == argItemID).FirstOrDefault();

                if (updateItem != null)
                {
                    if (updateItem.Name.IndexOf("加購_") < 0)
                    {
                        updateItem.Name = "加購_" + updateItem.Name;
                    }

                    updateItem.ShowOrder = (int)Item.ShowOrderStatus.AddtionalItemForCart;
                    updateItem.UpdateUser = updateUser;
                    updateItem.UpdateDate = DateTime.Now;

                    this._itemRepo.UpdateItem(updateItem);

                    updateResult = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return updateResult;
        }

        public bool EnableAdditionItemforItemTemp(int argItemTempID, string updateUser)
        {
            bool updateResult = false;
            
            try
            {
                ItemTemp updateItemtemp = this._itemTempRepo.GetAll().Where(x => x.ID == argItemTempID).FirstOrDefault();

                if (updateItemtemp != null)
                {
                    if (updateItemtemp.Name.IndexOf("加購_") < 0)
                    {
                        updateItemtemp.Name = "加購_" + updateItemtemp.Name;
                    }

                    updateItemtemp.Showorder = (int)Item.ShowOrderStatus.AddtionalItemForCart;
                    updateItemtemp.UpdateUser = updateUser;
                    updateItemtemp.UpdateDate = DateTime.Now;

                    this._itemTempRepo.Update(updateItemtemp);

                    updateResult = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return updateResult;
        }

        public bool EnableAdditionItemforItemSketch(int argItemSketchID, string updateUser)
        {
            bool updateResult = false;

            try
            {
                ItemSketch updateItemSketch = this._itemSketchRepo.GetAll().Where(x => x.ID == argItemSketchID).FirstOrDefault();

                if (updateItemSketch != null)
                {

                    if (string.IsNullOrEmpty(updateItemSketch.Name))
                    {
                        updateItemSketch.Name = "加購_";
                    }
                    else
                    {
                        if (updateItemSketch.Name.IndexOf("加購_") < 0)
                        {
                            updateItemSketch.Name = "加購_" + updateItemSketch.Name;
                        }
                    }

                    updateItemSketch.ShowOrder = (int)Item.ShowOrderStatus.AddtionalItemForCart;
                    updateItemSketch.UpdateUser = updateUser;
                    updateItemSketch.UpdateDate = DateTime.Now;

                    this._itemSketchRepo.Update(updateItemSketch);

                    updateResult = true;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return updateResult;
        }

        public bool DisableAdditionItem(int argItemID, string updateUser)
        {
            bool updateResult = false;
            Item updateItem = this._itemRepo.GetAll().Where(x => x.ID == argItemID).FirstOrDefault();
            try
            {
                if (updateItem.Name.IndexOf("加購_") == 0)
                {
                    updateItem.Name = updateItem.Name.Replace("加購_", "");
                }

                updateItem.ShowOrder = (int)Item.ShowOrderStatus.正常;
                updateItem.UpdateUser = updateUser;
                updateItem.UpdateDate = DateTime.Now;

                this._itemRepo.UpdateItem(updateItem);

                updateResult = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return updateResult;
        }

        public bool DisableAdditionItemTemp(int argItemTempID, string updateUser)
        {
            bool updateResult = false;
            ItemTemp updateItemTemp = this._itemTempRepo.GetAll().Where(x => x.ID == argItemTempID).FirstOrDefault();
            try
            {
                if (updateItemTemp.Name.IndexOf("加購_") == 0)
                {
                    updateItemTemp.Name = updateItemTemp.Name.Replace("加購_", "");
                }

                updateItemTemp.Showorder = (int)Item.ShowOrderStatus.正常;
                updateItemTemp.UpdateUser = updateUser;
                updateItemTemp.UpdateDate = DateTime.Now;

                this._itemTempRepo.Update(updateItemTemp);

                updateResult = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return updateResult;
        }

        public bool DisableAdditionItemSketch(int argItemSketchID, string updateUser)
        {
            bool updateResult = false;
            ItemSketch updateItemSketch = this._itemSketchRepo.GetAll().Where(x => x.ID == argItemSketchID).FirstOrDefault();
            try
            {
                if (!string.IsNullOrEmpty(updateItemSketch.Name))
                {
                    if (updateItemSketch.Name.IndexOf("加購_") == 0)
                    {
                        updateItemSketch.Name = updateItemSketch.Name.Replace("加購_", "");
                    }
                }

                updateItemSketch.ShowOrder = (int)Item.ShowOrderStatus.正常;
                updateItemSketch.UpdateUser = updateUser;
                updateItemSketch.UpdateDate = DateTime.Now;

                this._itemSketchRepo.Update(updateItemSketch);

                updateResult = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return updateResult;
        }
    }
}
