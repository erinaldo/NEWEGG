using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.AdditionalItemServices.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ItemSketchRepoAdapters.Interface;
using TWNewEgg.ItemTempRepoAdapters.Interface;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.AdditionalItemServices
{
    public class AdditionItemSearchService : TWNewEgg.AdditionalItemServices.Interface.IAdditionalSearch
    {
        private IItemSketchRepoAdapter _itemSketchRepo;
        private IItemRepoAdapter _itemRepo;
        private IItemTempRepoAdapter _itemTempRepo;

        public AdditionItemSearchService(IItemRepoAdapter itemRepo, IItemSketchRepoAdapter itemSketchRepo, IItemTempRepoAdapter itemTempRepo)
        {
            this._itemRepo = itemRepo;
            this._itemSketchRepo = itemSketchRepo;
            this._itemTempRepo = itemTempRepo;
        }

        public Dictionary<int, int> checkAdditionItem(int argitemID)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            try
            {
                var itemInfo = this._itemRepo.GetAll().Where(x => x.ID == argitemID).FirstOrDefault();

                if (itemInfo != null)
                {
                    int itemShowOrder = itemInfo.ShowOrder;

                    result.Add(argitemID, itemShowOrder);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Dictionary<int, int> checkAdditionTemp(int argitemTempID)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            try
            {
                var itemInfo = this._itemTempRepo.GetAll().Where(x => x.ID == argitemTempID).FirstOrDefault();

                if (itemInfo != null)
                {
                    int itemShowOrder = itemInfo.Showorder;

                    result.Add(argitemTempID, itemShowOrder);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Dictionary<int, int> checkAdditionSketch(int argitemItemSketchID)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            try
            {
                var itemInfo = this._itemSketchRepo.GetAll().Where(x => x.ID == argitemItemSketchID && x.ShowOrder.HasValue).FirstOrDefault();

                if (itemInfo != null)
                {
                    int itemShowOrder = itemInfo.ShowOrder.Value;

                    result.Add(argitemItemSketchID, itemShowOrder);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Dictionary<int, int> checkAdditionItems(List<int> argitemID)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            try
            {
                result = this._itemRepo.GetAll().Where(x => argitemID.Contains(x.ID)).Select(y => new { y.ID, y.ShowOrder }).ToDictionary(x => x.ID, x => x.ShowOrder);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Dictionary<int, int> checkAdditionTemps(List<int> argitemTempID)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            try
            {
                result = this._itemTempRepo.GetAll().Where(x => argitemTempID.Contains(x.ID)).Select(y => new { y.ID, y.Showorder }).ToDictionary(x => x.ID, x => x.Showorder);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Dictionary<int, int> checkAdditionSketchs(List<int> argitemItemSketchID)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            try
            {
                result = this._itemSketchRepo.GetAll().Where(x => argitemItemSketchID.Contains(x.ID) && x.ShowOrder.HasValue).Select(y => new { y.ID, y.ShowOrder.Value }).ToDictionary(x => x.ID, x => x.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
