using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.GreetingWordsRepoAdapters.Interface
{
    /// <summary>
    /// GreetingWords介面接口-------------add by bruce 20160330
    /// </summary>
    public interface IGreetingWordsRepoAdapter
    {
        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        GreetingWordsDB GetInfo(int? id);

        IQueryable<GreetingWordsDB> GetData(int category_id, string description, string codetext, int? showall);

        //IQueryable<GreetingWords> GetAll(int category_id);
        IQueryable<GreetingWordsDB> GetAll();

        //IQueryable<GreetingWords> GetByCategory(int category_id);
        //IQueryable<GreetingWords> GetByDescription(string description);
        //IQueryable<GreetingWords> GetByShowAll(int showall);
        //IQueryable<GreetingWords> GetByCodeText(string codetext);

        bool Update(GreetingWordsDB info);
        //List<GreetingWordsDB> Add(List<GreetingWordsDB> list_info);
        GreetingWordsDB Add(GreetingWordsDB info);
        //void Del(GreetingWords info);
        bool Del(GreetingWordsDB info);
    }
}
