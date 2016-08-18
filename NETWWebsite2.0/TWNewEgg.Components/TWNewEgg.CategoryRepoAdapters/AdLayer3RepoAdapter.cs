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
    public class AdLayer3RepoAdapter : IAdLayer3RepoAdapter
    {
        private IRepository<AdLayer3> _adLayer3RepoAdapter;
        private IRepository<AdLayer3Item> _adLayer3ItemRepoAdapter;

        public AdLayer3RepoAdapter(IRepository<AdLayer3> adLayer3RepoAdapter, IRepository<AdLayer3Item> adLayer3ItemRepoAdapter)
        {
            this._adLayer3RepoAdapter = adLayer3RepoAdapter;
            this._adLayer3ItemRepoAdapter = adLayer3ItemRepoAdapter;
        }

        public IQueryable<AdLayer3> AdLayer3_GetAll()
        {
            return this._adLayer3RepoAdapter.GetAll();
        }

        public IQueryable<AdLayer3Item> AdLayer3Item_GetAll()
        {
            return this._adLayer3ItemRepoAdapter.GetAll();
        }

        public AdLayer3 UpdateAdLayer3Data(AdLayer3 newData)
        {
            try
            {
                //檢查是否有Title
                if (string.IsNullOrEmpty(newData.Title))
                {
                    throw new Exception("Title is Null!!!");
                }
                //檢查是否有CategoryID
                if (string.IsNullOrEmpty(newData.CategoryID.ToString()) || (newData.CategoryID == 0))
                {
                    throw new Exception("CategoryID is Null!!!");
                }
                //檢查是否有AdType
                if (string.IsNullOrEmpty(newData.AdType.ToString()) || (newData.AdType == 0))
                {
                    throw new Exception("AdType is Null!!!");
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
                if (!_adLayer3RepoAdapter.GetAll().Any(x => x.ID == newData.ID))
                {
                    newData.CreateDate = DateTime.Now;
                    newData.CreateUser = newData.UpdateUser;
                    _adLayer3RepoAdapter.Create(newData);
                }
                else
                {
                    newData.UpdateDate = DateTime.Now;
                    _adLayer3RepoAdapter.Update(newData);
                }

                return newData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AdLayer3Item UpdateAdLayer3ItemData(AdLayer3Item newData)
        {
            try
            {
                //檢查是否有Title
                if (string.IsNullOrEmpty(newData.AdLayer3ID.ToString()) || (newData.AdLayer3ID <= 0))
                {
                    throw new Exception("AdLayer3ID is Null!!!");
                }
                //檢查是否有CategoryID
                if (string.IsNullOrEmpty(newData.ItemID.ToString()) || (newData.ItemID == 0))
                {
                    throw new Exception("ItemID is Null!!!");
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
                if (!_adLayer3ItemRepoAdapter.GetAll().Any(x => x.AdLayer3ID == newData.AdLayer3ID && x.ItemID == newData.ItemID))
                {
                    newData.CreateDate = DateTime.Now;
                    newData.CreateUser = newData.UpdateUser;
                    _adLayer3ItemRepoAdapter.Create(newData);
                }
                else
                {
                    newData.UpdateDate = DateTime.Now;
                    _adLayer3ItemRepoAdapter.Update(newData);
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
                if (_adLayer3ItemRepoAdapter.GetAll().Where(x => x.AdLayer3ID == ID).Any())
                {
                    List<AdLayer3Item> deleteDataList = new List<AdLayer3Item>();
                    deleteDataList = _adLayer3ItemRepoAdapter.GetAll().Where(x => x.AdLayer3ID == ID).ToList();

                    foreach (AdLayer3Item deleteData in deleteDataList)
                    {
                        _adLayer3ItemRepoAdapter.Delete(deleteData);
                    }
                }
                else
                {
                    throw new Exception(string.Format("AdLayer3ID={1} no data!!!", ID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
