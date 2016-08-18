using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;

namespace TWNewEgg.DAL.Repository
{
    public class MySQLSQLCommandRepository : IMySQLSQLCommandRepository
    {
        private string localMySqlConnectStr = System.Configuration.ConfigurationManager.ConnectionStrings["MySQLLocalConnection"].ConnectionString;
        private MySql.Data.MySqlClient.MySqlConnection mysqlClient;


        public void Create<T>(List<T> model, TWNewEgg.DAL.Model.MySQLModel.Table tableName)
        {
            Type temp = typeof(T);
            List<T> data = new List<T>();
            data = this.TypeToModel<T>(model);
            List<string> sqlCommand = new List<string>();
            foreach (T item in data)
            {
                List<string> strTemp = this.insertCreateSQLCommand(item, tableName);
                sqlCommand.AddRange(strTemp);
            }
            this.MySqlCommand(sqlCommand);
        }
        public void Create<T>(T model, TWNewEgg.DAL.Model.MySQLModel.Table tableName)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (System.Reflection.PropertyInfo pro in temp.GetProperties())
            {
                var value = pro.GetValue(model, null);
                pro.SetValue(obj, value, null);
            }
            List<string> sqlCommand = this.insertCreateSQLCommand(obj, tableName);
            this.MySqlCommand(sqlCommand);
        }
        public List<T> GetData<T>(string sqlCommand)
        {
            System.Data.DataTable _dataTable = new System.Data.DataTable();
            mysqlClient = new MySql.Data.MySqlClient.MySqlConnection();
            mysqlClient.ConnectionString = localMySqlConnectStr;
            MySql.Data.MySqlClient.MySqlDataAdapter myDataAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(sqlCommand, localMySqlConnectStr);
            MySql.Data.MySqlClient.MySqlCommandBuilder cmb = new MySql.Data.MySqlClient.MySqlCommandBuilder(myDataAdapter);
            myDataAdapter.Fill(_dataTable);
            var myListModel = this.ConvertDataTable<T>(_dataTable);
            return myListModel;
        }
        public void Update(string sqlCommand)
        {
            List<string> listsqlCommand = new List<string>();
            listsqlCommand.Add(sqlCommand);
            this.MySqlCommand(listsqlCommand);
        }
        /// <summary>
        /// 轉換 DataTable 為 List model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<T> ConvertDataTable<T>(System.Data.DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (System.Data.DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        /// <summary>
        /// 轉換對應的 DataRow 為 model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static T GetItem<T>(System.Data.DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (System.Data.DataColumn column in dr.Table.Columns)
            {
                foreach (System.Reflection.PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
        }
        /// <summary>
        /// 執行對 MySQL 的 SQL 語法
        /// </summary>
        /// <param name="sqlCommand"></param>
        private void MySqlCommand(List<string> sqlCommand)
        {
            mysqlClient = new MySql.Data.MySqlClient.MySqlConnection();
            mysqlClient.ConnectionString = localMySqlConnectStr;
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand();
            try
            {
                mysqlClient.Open();
                cmd.Connection = mysqlClient;
                foreach (string sqlStr in sqlCommand)
                {
                    cmd.CommandText = string.Empty;
                    cmd.CommandText = sqlStr;
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                mysqlClient.Close();
            }
            finally
            {
                mysqlClient.Close();
            }
        }
        /// <summary>
        /// 組合 insert sql 語法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<string> insertCreateSQLCommand<T>(T model, TWNewEgg.DAL.Model.MySQLModel.Table tableName)
        {
            List<string> insertList = new List<string>();
            string strElement = "insert into twsqldb." + tableName.ToString().ToLower() + "(";
            string strValue = "values(";
            foreach (System.Reflection.PropertyInfo item in model.GetType().GetProperties())
            {
                string elementName = item.Name;
                string elementValue = this.CreateValueToColumn<T>(item, model);
                if (item.Name == model.GetType().GetProperties().Last().Name)
                {
                    strElement = strElement + elementName + ")";
                    strValue = strValue + elementValue + ")";
                    insertList.Add(strElement + strValue);
                    strElement = string.Empty;
                    strValue = string.Empty;
                }
                else
                {
                    strElement = strElement + elementName + ",";
                    strValue = strValue + elementValue + ",";
                }
            }
            return insertList;
        }
        /// <summary>
        /// 轉換泛型 MODEL 為可用 MODEL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<T> TypeToModel<T>(List<T> model)
        {
            Type temp = typeof(T);
            List<T> data = new List<T>();
            foreach (var item in model)
            {
                T obj = Activator.CreateInstance<T>();
                foreach (System.Reflection.PropertyInfo pro in temp.GetProperties())
                {
                    var value = pro.GetValue(item, null);
                    pro.SetValue(obj, value, null);
                    if (pro.Name == item.GetType().GetProperties().Last().Name)
                    {
                        data.Add(obj);
                    }
                }
            }
            return data;
        }
        /// <summary>
        /// 根據 MODEL 對應的欄位與屬性回傳對應的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private string CreateValueToColumn<T>(System.Reflection.PropertyInfo propertyInfo, T model)
        {
            string returnValue = string.Empty;
            if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(Nullable<DateTime>))
            {
                DateTime _dateNow = DateTime.Now;
                if (propertyInfo.GetValue(model, null) == null)
                {
                    returnValue = "'1900/01/01'";
                }
                else
                {
                    returnValue = "'" + _dateNow.Year + "/" + _dateNow.Month + "/" + _dateNow.Day + " " + _dateNow.Hour + ":" + _dateNow.Minute + ":" + _dateNow.Second + "'";
                }
            }
            else if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(Nullable<int>))
            {
                if (propertyInfo.GetValue(model, null) == null)
                {
                    //returnValue = "''";
                    returnValue = "'0'";
                }
                else
                {
                    if (string.IsNullOrEmpty(propertyInfo.GetValue(model, null).ToString()) == true)
                    {
                        //returnValue = "''";
                        returnValue = "'0'";
                    }
                    else
                    {
                        returnValue = "'" + propertyInfo.GetValue(model, null).ToString() + "'";
                    }
                }
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                if (propertyInfo.GetValue(model, null) == null)
                {
                    returnValue = "''";
                }
                else
                {
                    if (string.IsNullOrEmpty(propertyInfo.GetValue(model, null).ToString()) == true)
                    {
                        //returnValue = "''";
                        returnValue = "' '";
                    }
                    else
                    {
                        returnValue = "'" + propertyInfo.GetValue(model, null).ToString().Replace(",", "comma") + "'";
                    }
                }
            }
            else if (propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(Nullable<decimal>))
            {
                if (propertyInfo.GetValue(model, null) == null)
                {
                    //returnValue = "''";
                    returnValue = "'0'";
                }
                else
                {
                    if (string.IsNullOrEmpty(propertyInfo.GetValue(model, null).ToString()) == true)
                    {
                        //returnValue = "''";
                        returnValue = "'0'";
                    }
                    else
                    {
                        returnValue = "'" + propertyInfo.GetValue(model, null).ToString() + "'";
                    }
                }
            }
            else
            {
                returnValue = "''";

            }
            return returnValue;
        }
    }
}
