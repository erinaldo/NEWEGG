using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TWNewEgg.ItemRepoAdapters.Interface;
using Rhino.Mocks;
using TWNewEgg.ItemServices.Interface;

namespace TWNewEgg.ItemServices.Test
{
    [TestClass]
    public class ItemDetailServiceTests
    {
        IItemDetailService _itemDetailService;
        IItemRepoAdapter _itemRepoAdapter;

        public ItemDetailServiceTests()
        {
            this._itemDetailService = MockRepository.GenerateMock<ItemDetailService>(_itemRepoAdapter);
        }

        [TestMethod]
        public void GetItemDetail_Behavior()
        {
            _itemDetailService.GetItemDetail(1);
        }

        private void SetStubs()
        {
            this._itemRepoAdapter = MockRepository.GenerateStub<IItemRepoAdapter>();
        }
    }
}
