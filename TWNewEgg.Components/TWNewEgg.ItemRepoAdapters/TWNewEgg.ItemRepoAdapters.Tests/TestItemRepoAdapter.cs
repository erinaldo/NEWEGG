using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System.Linq;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.ItemRepoAdapters.Interface;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TWNewEgg.ItemRepoAdapters.Tests
{
    [TestClass]
    public class TestItemRepoAdapter
    {
        private IRepository<Item> _itemRepo;
        private IRepository<ItemCategory> _itemCategoryRepo;
        private IItemRepoAdapter _itemRepoAdapter;

        public TestItemRepoAdapter() 
        {
            this.SetStubs();
            this._itemRepoAdapter = MockRepository.GenerateMock<ItemRepoAdapter>(this._itemRepo, this._itemCategoryRepo);
        }

        [TestMethod]
        public void GetAvailableAndVisible()
        {
            this._itemRepo.Stub(x => x.GetAll())
                .Return(new List<Item>().AsQueryable());
            
            this._itemRepoAdapter.GetAvailableAndVisible(1);
            this._itemRepo.AssertWasCalled(dao => dao.GetAll(), opt => opt.Repeat.AtLeastOnce());
        }

        [TestMethod]
        public void GetCrossCategoryAvailableAndVisible()
        {
            this._itemRepo.Stub(x => x.GetAll())
                .Return(new List<Item>().AsQueryable());
            this._itemCategoryRepo.Stub(x => x.GetAll())
                .Return(new List<ItemCategory>().AsQueryable());

            this._itemRepoAdapter.GetCrossCategoryAvailableAndVisible(1);
            this._itemRepo.AssertWasCalled(dao => dao.GetAll(), opt => opt.Repeat.AtLeastOnce());
            this._itemCategoryRepo.AssertWasCalled(dao => dao.GetAll(), opt => opt.Repeat.AtLeastOnce());
        }

        [TestMethod]
        public void GetIfAvailable_Exists()
        {
            this._itemRepo
                .Stub(x => x.Get(Arg<Expression<Func<Item, bool>>>.Is.Anything))
                .Return(new Item());
            var result = this._itemRepoAdapter.GetIfAvailable(1);
            this._itemRepo.AssertWasCalled(dao => dao.Get(Arg<Expression<Func<Item, bool>>>.Is.Anything));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetIfAvailable_NotExists()
        {
            this._itemRepo
                .Stub(x => x.Get(Arg<Expression<Func<Item, bool>>>.Is.Anything))
                .Return(null);
            var result = this._itemRepoAdapter.GetIfAvailable(1);
            this._itemRepo.AssertWasCalled(dao => dao.Get(Arg<Expression<Func<Item, bool>>>.Is.Anything));
            Assert.IsNull(result);
        }

        private void SetStubs()
        {
            this._itemRepo = MockRepository.GenerateStub<IRepository<Item>>();
            this._itemCategoryRepo = MockRepository.GenerateStub<IRepository<ItemCategory>>();
        }
    }
}
