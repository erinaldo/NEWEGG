using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Design;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace TWNewEgg.DB.Service
{
    public class TestSelfRepository : ITestSelf
    {
        private List<string> noNeedTable = new List<string>();
        public TestSelfRepository()
        {
            addNoNeedTable();
        }
        /// <summary>
        /// Input tables' name which won't check with db table.
        /// </summary>
        /// <param name="noNeedList">Data Type is List&lt;string&gt;</param>
        public TestSelfRepository(List<string> noNeedList)
        {
            addNoNeedTable();
            noNeedTable.AddRange(noNeedList);
        }
        private void addNoNeedTable()
        {
            //TWSQL
            noNeedTable.Add("__MigrationHistory");
            noNeedTable.Add("ViewItemSearch");
            noNeedTable.Add("ViewTracksActive");
            noNeedTable.Add("ViewTracksBuyActive");

            //TWBACNKEND
            noNeedTable.Add("POItemDetail");
            noNeedTable.Add("ProcessInfoView");
        }
        /// <summary>
        /// Compare DB table with DB model, and output different numbers.
        /// </summary>
        /// <param name="dbName">DB Name</param>
        /// <param name="ipAddr">SQL server address, if ipAddr is empty, then it'll using the connect setting in web.config.</param>
        /// <param name="userName">Sql server username</param>
        /// <param name="password">Sql server password</param>
        /// <returns></returns>
        public Dictionary<string, string> quickTest(string dbName, string ipAddr = "", string userName = "", string password = "")
        {
            Dictionary<string, List<string>> reports = new Dictionary<string, List<string>>();
            reports = GetReports(dbName, ipAddr, userName, password);
            Dictionary<string, string> returnString = new Dictionary<string, string>();
            foreach (var aReport in reports)
            {
                if (aReport.Value.Count > 0 && (noNeedTable.Exists(x => x == aReport.Key.ToString()) == false))
                {
                    returnString.Add(aReport.Key, "DB table [" + aReport.Key + "] had [" + aReport.Value.Count + "] different with Model.");
                }
            }
            return returnString;
        }
        /// <summary>
        /// Compare DB table with DB model, and output detail message.
        /// </summary>
        /// <param name="dbName">DB Name</param>
        /// <param name="ipAddr">SQL server address, if ipAddr is empty, then it'll using the connect setting in web.config.</param>
        /// <param name="userName">Sql server username</param>
        /// <param name="password">Sql server password</param>
        /// <returns></returns>
        public Dictionary<string, List<string>> testDetail(string dbName, string ipAddr = "", string userName = "", string password = "")
        {
            Dictionary<string, List<string>> reports = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> newReports = new Dictionary<string, List<string>>();
            reports = GetReports(dbName, ipAddr, userName, password);
            foreach (var aReport in reports)
            {
                if (aReport.Value.Count > 0 && (noNeedTable.Exists(x => x == aReport.Key.ToString()) == false))
                {
                    newReports.Add(aReport.Key, aReport.Value);
                }
            }
            return newReports;

        }

        private Dictionary<string, List<string>> GetReports(string dbName, string ipAddr, string userName, string password)
        {
            Dictionary<string, List<string>> reports = new Dictionary<string, List<string>>();
            string connString = "";
            switch (dbName)
            {
                case "TWSQLDB":
                    DB.TWSqlDBContext TWSQLDBContext = new TWSqlDBContext();
                    if (ipAddr == "")
                    {
                        if (ConfigurationManager.ConnectionStrings["TWSqlDBConnection"] == null)
                        {
                            reports.Add("No Connecttion String", new List<string>() { "..." });
                            break;
                        }
                        connString = ConfigurationManager.ConnectionStrings["TWSqlDBConnection"].ConnectionString.ToString();
                    }
                    reports = CheckDB<DB.TWSqlDBContext>(TWSQLDBContext, "TWSQLDB", ipAddr, userName, password, connString);
                    break;
                case "TWBACKENDDB":
                    DB.TWBackendDBContext TWBACKENDDBContext = new TWBackendDBContext();
                    if (ipAddr == "")
                    {
                        if (ConfigurationManager.ConnectionStrings["TWBackendDBConnection"] == null)
                        {
                            reports.Add("No Connecttion String", new List<string>() { "..." });
                            break;
                        }
                        connString = ConfigurationManager.ConnectionStrings["TWBackendDBConnection"].ConnectionString.ToString();
                    }
                    reports = CheckDB<DB.TWBackendDBContext>(TWBACKENDDBContext, "TWBACKENDDB", ipAddr, userName, password, connString);
                    break;
                case "TWSELLERPORTALDB":
                    DB.TWSellerPortalDBContext TWSELLERPORTALDBContext = new TWSellerPortalDBContext();
                    if (ipAddr == "")
                    {
                        if (ConfigurationManager.ConnectionStrings["TWSellerPortalDBConnection"] == null)
                        {
                            reports.Add("No Connecttion String", new List<string>() { "..." });
                            break;
                        }
                        connString = ConfigurationManager.ConnectionStrings["TWSellerPortalDBConnection"].ConnectionString.ToString();
                    }
                    reports = CheckDB<DB.TWSellerPortalDBContext>(TWSELLERPORTALDBContext, "TWSELLERPORTALDB", ipAddr, userName, password, connString);
                    break;
                default:
                    break;
            }
            return reports;
        }
        private Dictionary<string, List<string>> CheckDB<T>(T databaseContext, string dbName, string ipAddr, string userName, string password, string connString)
        {
            Dictionary<string, List<string>> reports = new Dictionary<string, List<string>>();
            string connectString = "";
            if (connString != "")
            {
                connectString = connString;
            }
            else
            {
                connectString = "Data Source=" + ipAddr + ";Database=" + dbName + ";User ID=" + userName + ";Password=" + password;
            }

            var metadata = ((IObjectContextAdapter)databaseContext).ObjectContext.MetadataWorkspace;
            var tables = metadata.GetItemCollection(System.Data.Metadata.Edm.DataSpace.SSpace)
              .GetItems<System.Data.Metadata.Edm.EntityContainer>()
              .Single()
              .BaseEntitySets
              .OfType<EntitySet>()
              .Where(s => !s.MetadataProperties.Contains("Type")
                || s.MetadataProperties["Type"].ToString() == "Tables");

            var storeGenerator = new EntityStoreSchemaGenerator(
                "System.Data.SqlClient",
                @"" + connectString + ";",
                "namespace");
            storeGenerator.GenerateStoreMetadata();
            var dbtables = storeGenerator.StoreItemCollection
              .GetItems<EntityContainer>()
              .Single()
              .BaseEntitySets
              .OfType<EntitySet>()
              .Where(s => !s.MetadataProperties.Contains("Type")
                || s.MetadataProperties["Type"].ToString() == "Tables");


            foreach (var dbtable in dbtables)
            {
                List<string> errorList = new List<string>();
                bool diffFlag = false;
                foreach (var table in tables)
                {
                    var tableName = table.MetadataProperties.Contains("Table")
                        && table.MetadataProperties["Table"].Value != null
                      ? table.MetadataProperties["Table"].Value.ToString()
                      : table.Name;
                    var tableSchema = table.MetadataProperties["Schema"].Value.ToString();
                    if (tableSchema == "dbo")
                    {
                        if (dbtable.Name.Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var dbtableProperties = dbtable.ElementType.Properties;
                            var tableProperties = table.ElementType.Properties;
                            foreach (var dbPropertyType in dbtableProperties)
                            {
                                string diffType = "";
                                bool diffColumn = true;
                                foreach (var propertyType in tableProperties)
                                {
                                    if (dbPropertyType.Name.Equals(propertyType.Name, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (dbPropertyType.TypeUsage.EdmType.BaseType.ToString() == propertyType.TypeUsage.EdmType.BaseType.ToString())
                                        {
                                            if (dbPropertyType.Nullable == propertyType.Nullable)
                                            {
                                                diffType = "";
                                                diffColumn = false;
                                                break;
                                            }
                                            else
                                            {
                                                diffType = ((propertyType.Nullable) ? "Nullable<" + propertyType.TypeUsage.EdmType.BaseType.ToString() + ">" : propertyType.TypeUsage.EdmType.BaseType.ToString());
                                            }
                                        }
                                        else
                                        {
                                            diffType = propertyType.TypeUsage.EdmType.BaseType.ToString();
                                        }
                                        diffColumn = false;
                                    }
                                    else
                                    {
                                    }
                                }
                                if (diffColumn)
                                {
                                    errorList.Add("Column name : [" + dbPropertyType.Name + "] is not exist in model.");
                                }
                                if (diffType != "" && diffType != "Edm.String" && diffType != "Nullable<Edm.String>" && diffColumn == false)
                                {
                                    errorList.Add("DB column [" + dbPropertyType.Name + "] type is [" + ((dbPropertyType.Nullable) ? "Nullable<" + dbPropertyType.TypeUsage.EdmType.BaseType.ToString() + ">" : dbPropertyType.TypeUsage.EdmType.BaseType.ToString()) + "] which not equal with model type [" + diffType + "].");
                                }
                            }
                            reports.Add(dbtable.Name, errorList);
                            diffFlag = false;
                            break;
                        }
                        else
                        {
                            diffFlag = true;
                        }
                    }
                }
                if (diffFlag)
                {
                    errorList.Add("This table class isn't in Models.");
                    reports.Add(dbtable.Name, errorList);
                }
            }
            return reports;
        }
    }
}
