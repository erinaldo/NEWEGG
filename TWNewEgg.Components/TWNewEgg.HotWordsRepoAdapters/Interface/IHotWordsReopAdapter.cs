using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.HotWordsRepoAdapters.Interface
{
    public interface IHotWordsReopAdapter
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="argObjHotWords"></param>
        void Create(HotWords argObjHotWords);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="argObjHotwords"></param>
        /// <returns></returns>
        bool Update(HotWords argObjHotwords);

        /// <summary>
        /// 根據Id取得HotWords
        /// </summary>
        /// <param name="argNumId">HotWords.Id</param>
        /// <returns></returns>
        IQueryable<HotWords> GetById(int argNumId);

        /// <summary>
        /// 取得所有的HotWords
        /// </summary>
        /// <returns></returns>
        IQueryable<HotWords> GetAll();
    }
}
