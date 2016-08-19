using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TWNewEgg.CategoryItem;
using TWNewEgg.CategoryItem.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.PropertyRepoAdapters.Interface;
using System.Collections.Generic;
using AutoMapper;

namespace TWNewEgg.CategoryItem.Tests
{
    [TestClass]
    public class TestICategoryItemService
    {
        private IItemRepoAdapter _itemRepoAdapter;
        private IPropertyRepoAdapter _propertyRepoAdapter;
        private ICategoryItemService _categoryItemService;
        private List<Item> items;
        private List<Item> crossCategoryItems;
        private List<ItemCategory> itemCategory;
        private List<ItemInfo> categoryItemInfos;

        public TestICategoryItemService()
        {
            SetData();
            SetStubs();
            this._categoryItemService = MockRepository.GeneratePartialMock<CategoryItemService>(
                this._itemRepoAdapter, this._propertyRepoAdapter);
        }

        [TestMethod]
        public void TestGetCategoryItems()
        {
            Mapper.CreateMap<Item, ItemInfo>();
            this._itemRepoAdapter
                .Stub(x => x.GetAvailableAndVisible(Arg<int>.Is.Anything))
                .Return(items.AsQueryable());
            this._itemRepoAdapter
                .Stub(x => x.GetCrossCategoryAvailableAndVisible(Arg<int>.Is.Anything))
                .Return(crossCategoryItems.AsQueryable());
            this._propertyRepoAdapter
                .Stub(x => x.FilterProductIds(Arg<List<int>>.Is.Anything, Arg<int>.Is.Anything, Arg<List<int>>.Is.Anything))
                .Return(items.Select(x=>x.ProductID).ToList());

            this._categoryItemService.GetCategoryItems(new CategoryItemConditions() { BrandID = 1, FilterID="1,2" });
            this._itemRepoAdapter.AssertWasCalled(
                dao => dao.GetAvailableAndVisible(Arg<int>.Is.Anything), 
                opt => opt.Repeat.AtLeastOnce());
            this._propertyRepoAdapter.AssertWasCalled(
                dao => dao.FilterProductIds(Arg<List<int>>.Is.Anything, Arg<int>.Is.Anything, Arg<List<int>>.Is.Anything),
                opt => opt.Repeat.AtLeastOnce());
        }

        private void SetStubs()
        {
            this._itemRepoAdapter = MockRepository.GenerateStub<IItemRepoAdapter>();
            this._propertyRepoAdapter = MockRepository.GenerateStub<IPropertyRepoAdapter>();
        }

        private void SetData()
        {
            items = new List<Item>();
            items.Add(new Item()
            {
                ID = 1,
                CategoryID = 1,
                Name = "Bomb",
                ProductID = 1,
                ManufactureID = 1
            });

            crossCategoryItems = new List<Item>();
            crossCategoryItems.Add(new Item()
            {
                ID = 2,
                CategoryID = 2,
                Name = "Gun",
                ProductID = 2,
                ManufactureID = 1
            });

            itemCategory = new List<ItemCategory>();
            itemCategory.Add(new ItemCategory()
            {
                CategoryID = 1,
                ItemID = 2
            });

            categoryItemInfos = new List<ItemInfo>();
            categoryItemInfos.Add(new ItemInfo()
            {
                ID = 1,
                Name = "Bomb",
                ProductID = 1
            });
            categoryItemInfos.Add(new ItemInfo()
            {
                ID = 2,
                Name = "Bomb",
                ProductID = 2
            });
        }
    }
}
