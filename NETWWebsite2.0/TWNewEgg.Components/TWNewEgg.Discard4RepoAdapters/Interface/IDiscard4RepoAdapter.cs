using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Discard4RepoAdapters.Interface
{
    /// <summary>
    /// 介面接口-------------add by bruce 20160502
    /// </summary>
    public interface IDiscard4RepoAdapter
    {
        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        Discard4DB GetInfo(int? id);

        IQueryable<Discard4DB> GetData(int salesOrderGroupID, string agreedDiscard4, string createUser);

        IQueryable<Discard4DB> GetAll();

        bool Update(Discard4DB info);
        
        Discard4DB Add(Discard4DB info);
        
        bool Del(Discard4DB info);
    }
}
