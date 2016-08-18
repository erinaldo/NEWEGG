using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Manufacture;

namespace TWNewEgg.ManufactureServices.Interface
{
    public interface IManufactureService
    {
        /// <summary>
        /// 取得所有的Manufacture
        /// </summary>
        /// <returns>null or list of Manufacture</returns>
        List<Manufacture> GetAll();

        /// <summary>
        /// 根據Id取得Manufacture
        /// </summary>
        /// <param name="argNumId">Id</param>
        /// <returns>null or Manufacture object</returns>
        Manufacture GetById(int argNumId);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="argObjManu"></param>
        void Create(Manufacture argObjManu);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="argObjManu"></param>
        /// <returns>update success return true, else return false</returns>
        bool Update(Manufacture argObjManu);
    }
}
