using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TWNewEgg.PropertyRepoAdapters.Test
{
    [TestClass]
    public class TestPropertyRepoAdapter
    {
        private IRepository<ItemPropertyGroup> _itemPropertyGroupRepo;
        private IRepository<ItemPropertyName> _itemPropertyNameRepo;
        private IRepository<ItemPropertyValue> _itemPropertyValueRepo;
        private IRepository<ProductProperty> _productPropertyRepo;

        private PropertyRepoAdapter propertyRepoAdapter;

        public TestPropertyRepoAdapter()
        {
            this.SetStubs();
            propertyRepoAdapter = MockRepository.GenerateMock<PropertyRepoAdapter>(this._productPropertyRepo, this._itemPropertyGroupRepo,
                this._itemPropertyNameRepo, this._itemPropertyValueRepo);
        }

        [TestMethod]
        public void FilterProductIds_ThroughProductProperty()
        {
            this._itemPropertyGroupRepo
                .Stub(dao => dao.GetAll())
                .Return(new List<ItemPropertyGroup>().AsQueryable());
            this._productPropertyRepo
                .Stub(dao => dao.GetAll())
                .Return(new List<ProductProperty>().AsQueryable());
            
            propertyRepoAdapter.FilterProductIds(new List<int>(), 1, new List<int>());

            this._productPropertyRepo.AssertWasCalled(dao => dao.GetAll(), opt => opt.Repeat.AtLeastOnce());
        }

        [TestMethod]
        public void GetGroups()
        {
            this._itemPropertyGroupRepo
                .Stub(dao => dao.GetAll())
                .Return(new List<ItemPropertyGroup>().AsQueryable());

            propertyRepoAdapter.GetGroups(1, new string[1] { "F" });

            this._itemPropertyGroupRepo.AssertWasCalled(dao => dao.GetAll(), opt => opt.Repeat.Once());
        }

        [TestMethod]
        public void GetNames()
        {
            this._itemPropertyNameRepo
                .Stub(dao => dao.GetAll())
                .Return(new List<ItemPropertyName>().AsQueryable());

            propertyRepoAdapter.GetNames(new List<int>{1}, new string[1] { "F" });

            this._itemPropertyNameRepo.AssertWasCalled(dao => dao.GetAll(), opt => opt.Repeat.Once());
        }

        [TestMethod]
        public void GetValues()
        {
            this._itemPropertyValueRepo
                .Stub(dao => dao.GetAll())
                .Return(new List<ItemPropertyValue>().AsQueryable());

            propertyRepoAdapter.GetValues(new List<int> { 1 }, new string[1] { "F" });

            this._itemPropertyValueRepo.AssertWasCalled(dao => dao.GetAll(), opt => opt.Repeat.Once());
        }

        private void SetStubs()
        {
            this._itemPropertyGroupRepo = MockRepository.GenerateStub<IRepository<ItemPropertyGroup>>();
            this._itemPropertyNameRepo = MockRepository.GenerateStub<IRepository<ItemPropertyName>>();
            this._itemPropertyValueRepo = MockRepository.GenerateStub<IRepository<ItemPropertyValue>>();
            this._productPropertyRepo = MockRepository.GenerateStub<IRepository<ProductProperty>>();
        }
    }
}
