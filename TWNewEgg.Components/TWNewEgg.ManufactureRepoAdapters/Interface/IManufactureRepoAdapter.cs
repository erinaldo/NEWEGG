using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ManufactureRepoAdapters.Interface
{
    public interface IManufactureRepoAdapter
    {
        /// <summary>
        /// 取得所有的Manufacture
        /// </summary>
        /// <returns></returns>
        IQueryable<Manufacture> GetAll();

        /// <summary>
        /// 根據Id取得Fanufacture
        /// </summary>
        /// <param name="argNumId">Manufacture Id</param>
        /// <returns></returns>
        IQueryable<Manufacture> GetById(int argNumId);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="argObjManufacture">要修改的Manufacture</param>
        /// <returns>update success return true, else return false</returns>
        bool Update(Manufacture argObjManufacture);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="argObjManufacture"></param>
        void Create(Manufacture argObjManufacture);
    }
}
