using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CategoryRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CategoryRepoAdapters
{
    public class CategoryTopItemRepoAdapter : ICategoryTopItemRepoAdapter
    {
        private IRepository<CategoryTopItem> _categoryTopItemRepoAdapter;

        public CategoryTopItemRepoAdapter(IRepository<CategoryTopItem> categoryTopItemRepoAdapter)
        {
            this._categoryTopItemRepoAdapter = categoryTopItemRepoAdapter;
        }

        public IQueryable<CategoryTopItem> CategoryTopItem_GetAll()
        {
            return this._categoryTopItemRepoAdapter.GetAll();
        }

        public CategoryTopItem SaveCategoryTopItemData(CategoryTopItem newData)
        {
            try
            {
                //檢查是否有CategoryID
                if (string.IsNullOrEmpty(newData.CategoryID.ToString()) || (newData.CategoryID == 0))
                {
                    throw new Exception("CategoryID is Null!!!");
                }
                //檢查是否有ItemID
                if (string.IsNullOrEmpty(newData.ItemID.ToString()) || (newData.ItemID == 0))
                {
                    throw new Exception("ItemID is Null!!!");
                }
                //檢查是否有ItemType
                if (string.IsNullOrEmpty(newData.ItemType.ToString()) || (newData.ItemType == 0))
                {
                    throw new Exception("ItemType is Null!!!");
                }
                //檢查是否有ShowAll
                if (string.IsNullOrEmpty(newData.ShowAll.ToString()))
                {
                    throw new Exception("ShowAll is Null!!!");
                }
                //檢查是否有Showorder
                if (string.IsNullOrEmpty(newData.Showorder.ToString()) || (newData.Showorder == 0))
                {
                    throw new Exception("Showorder is Null!!!");
                }

                //檢查key(ID)是否已經存在
                if (!_categoryTopItemRepoAdapter.GetAll().Any(x => x.CategoryID == newData.CategoryID && x.ItemID == newData.ItemID))
                {
                    newData.CreateDate = DateTime.Now;
                    newData.CreateUser = newData.UpdateUser;
                    _categoryTopItemRepoAdapter.Create(newData);
                }
                else
                {
                    newData.UpdateDate = DateTime.Now;
                    _categoryTopItemRepoAdapter.Update(newData);
                }

                return newData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteAdLayer3Items(int ID)
        {
            try
            {
                //檢查是否有ID
                if (string.IsNullOrEmpty(ID.ToString()) || (ID <= 0))
                {
                    throw new Exception("ID is Null!!!");
                }
                //檢查key(ID)是否已經存在
                if (_categoryTopItemRepoAdapter.GetAll().Where(x => x.CategoryID == ID).Any())
                {
                    List<CategoryTopItem> deleteDataList = new List<CategoryTopItem>();
                    deleteDataList = _categoryTopItemRepoAdapter.GetAll().Where(x => x.CategoryID == ID).ToList();

                    foreach (CategoryTopItem deleteData in deleteDataList)
                    {
                        _categoryTopItemRepoAdapter.Delete(deleteData);
                    }
                }
                else
                {
                    throw new Exception(string.Format("CategoryID={1} no data!!!", ID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
