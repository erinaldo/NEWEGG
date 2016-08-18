using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TWNewEgg.PropertyRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Property;
using Rhino.Mocks;
using TWNewEgg.PropertyServices.Interface;
using System.Collections.Generic;
using AutoMapper;

namespace TWNewEgg.PropertyServices.Test
{
    [TestClass]
    public class TestPropertyService
    {
        private IPropertyRepoAdapter _propertyRepoAdapter;
        private IPropertyService _propertyService;
        private List<ItemPropertyGroup> groups;
        private List<ItemPropertyName> names;
        private List<ItemPropertyValue> values;

        public TestPropertyService()
        {
            SetStubs();
            SetData();
            this._propertyService = MockRepository.GenerateMock<PropertyService>(this._propertyRepoAdapter);
        }

        [TestMethod]
        public void GetPropertyGroups()
        {
            Mapper.CreateMap<ItemPropertyGroup, PropertyGroup>()
                .ForMember(dest => dest.GroupID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupNameTW));
            Mapper.CreateMap<ItemPropertyName, PropertyKey>()
                .ForMember(dest => dest.PNID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PNName, opt => opt.MapFrom(src => src.PropertyNameTW));
            Mapper.CreateMap<ItemPropertyValue, PropertyValue>()
                .ForMember(dest => dest.PVID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PVName, opt => opt.MapFrom(src => src.PropertyValueTW));

            this._propertyRepoAdapter
                .Stub(dao => dao.GetGroups(Arg<int>.Is.Anything, Arg<string[]>.Is.Anything))
                .Return(this.groups.AsQueryable());
            this._propertyRepoAdapter
                .Stub(dao => dao.GetNames(Arg<List<int>>.Is.Anything, Arg<string[]>.Is.Anything))
                .Return(this.names.AsQueryable());
            this._propertyRepoAdapter
                .Stub(dao => dao.GetValues(Arg<List<int>>.Is.Anything, Arg<string[]>.Is.Anything))
                .Return(this.values.AsQueryable());

            List<PropertyGroup> result = this._propertyService.GetPropertyGroups(1);

            this._propertyRepoAdapter
                .AssertWasCalled(dao => dao.GetGroups(Arg<int>.Is.Anything, Arg<string[]>.Is.Anything),
                opt => opt.Repeat.Once());
            this._propertyRepoAdapter
                .AssertWasCalled(dao => dao.GetNames(Arg<List<int>>.Is.Anything, Arg<string[]>.Is.Anything),
                opt => opt.Repeat.AtLeastOnce());
            this._propertyRepoAdapter
                .AssertWasCalled(dao => dao.GetValues(Arg<List<int>>.Is.Anything, Arg<string[]>.Is.Anything),
                opt => opt.Repeat.AtLeastOnce());

            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result[0].GroupProperties.Count > 0);
            Assert.IsTrue(result[0].GroupProperties[0].PropertyValues.Count > 0);
        }

        private void SetData()
        {
            groups = new List<ItemPropertyGroup>();
            groups.Add(new ItemPropertyGroup()
            {
                ID = 1,
                CategoryID = 1,
                GroupName = "主食"
            });
            groups.Add(new ItemPropertyGroup()
            {
                ID = 2,
                CategoryID = 2,
                GroupName = "飲料"
            });

            names = new List<ItemPropertyName>();
            names.Add(new ItemPropertyName()
            {
                ID=1,
                GroupID=1,
                PropertyNameTW="麵"
            });
            names.Add(new ItemPropertyName()
            {
                ID = 2,
                GroupID = 1,
                PropertyNameTW = "飯"
            });

            values = new List<ItemPropertyValue>();
            values.Add(new ItemPropertyValue()
            {
                ID=1,
                PropertyNameID = 1,
                PropertyValueTW="義大利麵"
            });
            values.Add(new ItemPropertyValue()
            {
                ID = 2,
                PropertyNameID = 1,
                PropertyValueTW = "牛肉麵"
            });
        }

        private void SetStubs()
        {
            this._propertyRepoAdapter = MockRepository.GenerateStub<IPropertyRepoAdapter>();
        }
    }
}
