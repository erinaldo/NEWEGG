using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.ItemService.Service;
using AutoMapper;

namespace TWNewEgg.ItemServices.Test
{
    [TestClass]
    public class ItemDisplayPriceTests
    {
        private IItemDisplayPriceService _itemDisplayPriceService;
        private IItemPrice _itemPrice;

        private Dictionary<int, TWNewEgg.DB.TWSQLDB.Models.ItemDisplayPrice> oldPrices;

        public ItemDisplayPriceTests()
        {
            this.SetStubs();
            this.SetData();
            this._itemDisplayPriceService = MockRepository.GenerateMock<ItemDisplayPriceService>(_itemPrice);
        }

        [TestMethod]
        public void GetItemDisplayPriceByIDs_Behavior()
        {
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.ItemDisplayPrice, TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice>();
            this._itemPrice
                .Stub(dao => dao.GetItemDisplayPriceByIDs(Arg<List<int>>.Is.Anything))
                .Return(oldPrices);
            
            this._itemDisplayPriceService.GetItemDisplayPriceByIDs(new List<int>() { 1 });

            this._itemPrice.AssertWasCalled(dao => dao.GetItemDisplayPriceByIDs(Arg<List<int>>.Is.Anything));
        }

        private void SetStubs()
        {
            this._itemPrice = MockRepository.GenerateStub<IItemPrice>();
        }

        private void SetData()
        {
            oldPrices = new Dictionary<int, DB.TWSQLDB.Models.ItemDisplayPrice>();
            oldPrices.Add(1, new DB.TWSQLDB.Models.ItemDisplayPrice()
            {
                ItemID = 1,
                DisplayPrice = 500
            });
        }
    }
}
