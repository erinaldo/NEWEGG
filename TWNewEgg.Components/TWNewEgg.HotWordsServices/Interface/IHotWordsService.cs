using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.HotWords;
using TWNewEgg.HotWordsRepoAdapters;
using TWNewEgg.HotWordsRepoAdapters.Interface;

namespace TWNewEgg.HotWordsServices.Interface
{
    public interface IHotWordsService
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="argObjHotWords"></param>
        void Create(Models.DomainModels.HotWords.HotWords argObjHotWords);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="argObjHotWords"></param>
        /// <returns></returns>
        bool Update(Models.DomainModels.HotWords.HotWords argObjHotWords);

        /// <summary>
        /// 根據Id取得Hotwords
        /// </summary>
        /// <param name="argNumId">HotWords.Id</param>
        /// <returns>null or HotWords object</returns>
        Models.DomainModels.HotWords.HotWords GetById(int argNumId);

        /// <summary>
        /// 取得所有的Hotwords
        /// </summary>
        /// <returns>null or list of Hotwords</returns>
        List<Models.DomainModels.HotWords.HotWords> GetAll();

        /// <summary>
        /// 取得Active的HotWords
        /// </summary>
        /// <param name="argNumCategoryId">CategoryId, 若為0, 表示首頁</param>
        /// <returns></returns>
        List<Models.DomainModels.HotWords.HotWords> GetActive(int argNumCategoryId);

    }
}
