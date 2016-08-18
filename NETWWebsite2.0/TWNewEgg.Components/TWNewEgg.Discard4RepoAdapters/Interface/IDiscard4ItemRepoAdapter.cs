using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

namespace TWNewEgg.Discard4RepoAdapters.Interface
{
    /// <summary>
    /// GreetingWords介面接口-------------add by bruce 20160330
    /// </summary>
    public interface IDiscard4ItemRepoAdapter
    {

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        Discard4ItemDB GetInfo(int? id);

        IQueryable<Discard4ItemDB> GetData(string salesOrderCode, string salesOrderItemCode, int itemID);

        IQueryable<Discard4ItemDB> GetAll();

        bool Update(Discard4ItemDB info);
        
        Discard4ItemDB Add(Discard4ItemDB info);
        
        bool Del(Discard4ItemDB info);
    }
}
