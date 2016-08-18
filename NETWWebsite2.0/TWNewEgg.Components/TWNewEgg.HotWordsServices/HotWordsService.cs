using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.HotWords;
using TWNewEgg.HotWordsRepoAdapters;
using TWNewEgg.HotWordsRepoAdapters.Interface;
using TWNewEgg.HotWordsServices.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.HotWordsServices
{
    public class HotWordsService : IHotWordsService
    {
        private IHotWordsReopAdapter _HotWordRepo = null;

        public HotWordsService(IHotWordsReopAdapter argHotWordRepo)
        {
            this._HotWordRepo = argHotWordRepo;
        }

        public void Create(Models.DomainModels.HotWords.HotWords argObjHotWords)
        {
        }

        public bool Update(Models.DomainModels.HotWords.HotWords argObjHotWords)
        {
            return false;
        }

        /// <summary>
        /// 用ID取HotWords
        /// </summary>
        /// <param name="argNumId"></param>
        /// <returns></returns>
        public Models.DomainModels.HotWords.HotWords GetById(int argNumId)
        {

            IQueryable<Models.DBModels.TWSQLDB.HotWords> querySearch = null;
            Models.DBModels.TWSQLDB.HotWords objDbSearch = null;
            Models.DomainModels.HotWords.HotWords objResult = null;

            try
            {
                querySearch = this._HotWordRepo.GetById(argNumId);    
                objDbSearch = querySearch.FirstOrDefault();

                if (objDbSearch != null)
                {
                    objResult = ModelConverter.ConvertTo<Models.DomainModels.HotWords.HotWords>(objDbSearch);
                }

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return objResult;    
        }

        /// <summary>
        /// 取得所有HotWords
        /// </summary>
        /// <returns></returns>
        public List<Models.DomainModels.HotWords.HotWords> GetAll()
        {
            IQueryable<Models.DBModels.TWSQLDB.HotWords> querySearch = null;
            List<Models.DBModels.TWSQLDB.HotWords> listDbSearch = null;
            List<Models.DomainModels.HotWords.HotWords> listResult = null;

            try
            {       
                querySearch = this._HotWordRepo.GetAll();
                listDbSearch = querySearch.ToList();
                if (listDbSearch != null)
                {
                    listResult = ModelConverter.ConvertTo<List<Models.DomainModels.HotWords.HotWords>>(listDbSearch);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return listResult;
        }
        
        /// <summary>
        /// 取得在Active的HotWords
        /// </summary>
        /// <param name="argNumCategoryId"></param>
        /// <returns></returns>
        public List<Models.DomainModels.HotWords.HotWords> GetActive(int argNumCategoryId)
        {
            IQueryable<Models.DBModels.TWSQLDB.HotWords> querySearch = null;
            List<Models.DBModels.TWSQLDB.HotWords> listDbSearch = null;
            List<Models.DomainModels.HotWords.HotWords> listResult = null;

            try
            {
                //找尋顯示 && 時間內的hotwords
                querySearch = this._HotWordRepo.GetAll().Where(x => x.ShowAll == 1 && DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).OrderBy(x => x.Showorder); 
                listDbSearch = querySearch.ToList();
                if (listDbSearch != null && listDbSearch.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo <List<Models.DomainModels.HotWords.HotWords>>(listDbSearch);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return listResult;
        }
    }
}
