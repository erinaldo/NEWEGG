using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.HotWordsRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.DAL.Interface;

namespace TWNewEgg.HotWordsRepoAdapters
{
    public class HotWordsReopAdapter:IHotWordsReopAdapter
    {
        private IRepository<HotWords> _HotWordRepo;

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="argHotWordRepo"></param>
        public HotWordsReopAdapter(IRepository<HotWords> argHotWordRepo)
        {
            this._HotWordRepo = argHotWordRepo;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="argObjHotWords"></param>
        public void Create(HotWords argObjHotWords)
        {
            if (argObjHotWords == null)
            {
                return;
            }

            try
            {
                this._HotWordRepo.Create(argObjHotWords);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="argObjHotwords"></param>
        /// <returns></returns>
        public bool Update(HotWords argObjHotwords)
        {
            if (argObjHotwords == null)
            {
                return false;
            }

            bool boolExec = false;

            try
            {
                this._HotWordRepo.Update(argObjHotwords);
                boolExec = true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return boolExec;
        }
        /// <summary>
        /// 用ID取得HotWords
        /// </summary>
        /// <param name="argNumId"></param>
        /// <returns></returns>
        public IQueryable<HotWords> GetById(int argNumId)
        {
            IQueryable<HotWords> queryResult = null;

            try
            {
                queryResult = this._HotWordRepo.GetAll().Where(x=>x.ID == argNumId);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return queryResult;
        }
        /// <summary>
        /// 取得全部hotWords
        /// </summary>
        /// <returns></returns>
        public IQueryable<HotWords> GetAll()
        {
            IQueryable<HotWords> queryResult = null;

            try
            {
                queryResult = this._HotWordRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return queryResult;
        }
    }
}
