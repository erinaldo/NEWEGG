using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.CategoryService.Models;

namespace TWNewEgg.CategoryService.Service
{
    public class FindProductProperty
    {
        private TWSqlDBContext twsqlDB = new TWSqlDBContext();
        private List<ProductProperty> allDBProperties = new List<ProductProperty>();
        private string[] Hide;
        public List<PropertyGroup> FindAllGroupPropertyByCategoryID(int categoryID, string[] hide)
        {
            Hide = hide;
            List<PropertyGroup> allPropertyGroups = new List<PropertyGroup>();
            var allGroups = twsqlDB.ItemPropertyGroup.Where(x => x.CategoryID == categoryID && Hide.Contains(x.Hide)).OrderBy(x => x.ShowOrder).ToList();
            List<int> groupIDs = allGroups.Select(x => x.ID).ToList();
            foreach (var groupProperty in allGroups)
            {
                var allNameIDs = twsqlDB.ItemPropertyName.Where(x => x.GroupID == groupProperty.ID).Select(x => x.ID).Distinct().ToList();
                var allNameProperties = this.FindAllNamePropertyByGroupIDs(new List<int> { groupProperty.ID })
                    .Where(x => !(x.PNName == null || x.PNName.Trim() == string.Empty))
                    .ToList();
                if (allNameProperties.Count > 0)
                {
                    allPropertyGroups.Add(
                        new PropertyGroup
                        {
                            GroupID = groupProperty.ID,
                            GroupName = groupProperty.GroupName,
                            Hide = groupProperty.Hide,
                            ShowOrder = groupProperty.ShowOrder,
                            GroupProperties = allNameProperties
                        });
                }
            }
            return allPropertyGroups;
        }

        public List<PropertyGroup> FindAllGroupPropertyByProductIDs(List<int> allProductIDs)
        {
            List<PropertyGroup> allPropertyGroups = new List<PropertyGroup>();
            allDBProperties = twsqlDB.ProductProperty.Where(x => allProductIDs.Contains(x.ProductID) && x.Show == 0).ToList();
            var allGroupIDs = allDBProperties.Select(x => x.GroupID).Distinct().ToList();
            var allGroupProperties = twsqlDB.ItemPropertyGroup.Where(x => allGroupIDs.Contains(x.ID)).OrderBy(x => x.ShowOrder).ToList();
            foreach (var groupProperty in allGroupProperties)
            {
                var allNameIDs = allDBProperties.Where(x => x.GroupID == groupProperty.ID).Select(x => x.PropertyNameID).Distinct().ToList();
                var allNameProperties = this.FindAllNamePropertyByGroupIDs(new List<int> { groupProperty.ID })
                    .Where(x => !(x.PNName == null || x.PNName.Trim() == string.Empty))
                    .ToList();
                if (allNameProperties.Count > 0)
                {
                    allPropertyGroups.Add(
                        new PropertyGroup
                        {
                            GroupID = groupProperty.ID,
                            GroupName = groupProperty.GroupNameTW,
                            Hide = groupProperty.Hide,
                            ShowOrder = groupProperty.ShowOrder,
                            GroupProperties = allNameProperties
                        });
                }
            }
            return allPropertyGroups;
        }

        private List<PropertyKey> FindAllNamePropertyByGroupIDs(List<int> allGroupIDs)
        {
            List<PropertyKey> allProperties = new List<PropertyKey>();
            var allNameProperties = twsqlDB.ItemPropertyName.Where(x => allGroupIDs.Contains(x.GroupID) && Hide.Contains(x.Hide)).Distinct().OrderBy(x => x.ShowOrder).ToList();

            foreach (var nameProperty in allNameProperties)
            {
                var allValueProperties = this.FindAllValuePropertyByNameIDs(new List<int> { nameProperty.ID })
                    .Where(x => !(x.PVName == null || x.PVName.Trim() == string.Empty))
                    .ToList();
                if (allValueProperties.Count > 0)
                {
                    allProperties.Add(
                        new PropertyKey
                        {
                            PNID = nameProperty.ID,
                            PNName = nameProperty.PropertyNameTW,
                            Hide = nameProperty.Hide,
                            ShowOrder = nameProperty.ShowOrder,
                            PropertyValues = allValueProperties
                        });
                }
            }

            return allProperties;
        }
        private List<PropertyValue> FindAllValuePropertyByNameIDs(List<int> allNameIDs)
        {
            List<PropertyValue> allProperties = new List<PropertyValue>();
            var allValueProperties = twsqlDB.ItemPropertyValue.Where(x => allNameIDs.Contains(x.PropertyNameID) && Hide.Contains(x.Hide)).Distinct().OrderBy(x => x.ShowOrder).ToList();
            foreach (var valueProperty in allValueProperties)
            {
                allProperties.Add(
                    new PropertyValue
                    {
                        PVID = valueProperty.ID,
                        PVName = valueProperty.PropertyValueTW,
                        Hide = valueProperty.Hide,
                        ShowOrder = valueProperty.ShowOrder
                    });
            }

            return allProperties;
        }
    }
}
