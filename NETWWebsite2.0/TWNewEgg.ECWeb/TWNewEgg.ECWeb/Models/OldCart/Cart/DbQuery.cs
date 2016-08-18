using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class DbQuery
    {
        private SqlConnection m_sqlConn;
        private SqlCommand m_sqlCmd;

        /// <summary>
        /// default SQL connection string from System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultDbConnectionString"]
        /// </summary>
        public DbQuery()
        {
            //建構子統一設定連線字串，與Web.config連動
            this.m_sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            //this.m_sqlTrans = this.m_sqlConn.BeginTransaction("TransQuery");
            this.m_sqlCmd = new SqlCommand();
            this.m_sqlCmd.Connection = this.m_sqlConn;
        }//end DbQuery();


        public void Dispose()
        {
            if (this.m_sqlCmd != null)
                this.m_sqlCmd.Dispose();

            if (this.m_sqlConn != null)
                this.m_sqlConn.Dispose();

        }

        /// <summary>
        /// reset connection string and re-build connection, command, trans object
        /// </summary>
        /// <param name="arg_strServer">Server Name or Server IP</param>
        /// <param name="arg_strDbName">DB Name</param>
        /// <param name="arg_strUser">login User Id</param>
        /// <param name="arg_strPassword">login User Password</param>
        private void setConnectionString(string arg_strServer, string arg_strDbName, string arg_strUser, string arg_strPassword)
        {
            //預設連接字串
            //string strDbConn = "server=10.16.131.43;Initial Catalog=tw_newegg;Integrated Security=false;User ID=misa;PWD=abs906egg";

            /*
             * 先釋放所有物件及記憶體
             * 再根據新的連接指令重建
             */
            if (this.m_sqlCmd != null)
                this.m_sqlCmd.Dispose();
            if (this.m_sqlConn != null)
                this.m_sqlConn.Dispose();

            this.m_sqlConn = new SqlConnection("server=" + arg_strServer + ";Initial Catalog=" + arg_strDbName + ";Integrated Security=false;User ID=" + arg_strUser + ";PWD=" + arg_strPassword);
            this.m_sqlCmd = new SqlCommand();
            this.m_sqlCmd = new SqlCommand();
            this.m_sqlCmd.Connection = this.m_sqlConn;
            this.m_sqlCmd.Connection = this.m_sqlConn;
        }//end setConnectionString

        /// <summary>
        /// Execute  query with Parameter
        /// </summary>
        /// <param name="arg_strSql">SQL command string</param>
        /// <param name="aryParameter">Parameters array</param>
        /// <returns>has data: return data table, else return null</returns>
        public DataSet Query(string arg_strSql, SqlParameter[] aryParameter)
        {
            if (arg_strSql.Length <= 0)
                return null;

            DataSet dsResult = null;
            SqlDataAdapter oAdapter = null;

            this.m_sqlCmd.Parameters.Clear();
            this.m_sqlCmd.CommandText = arg_strSql;

            if (aryParameter != null)
            {
                foreach (SqlParameter oParameter in aryParameter)
                    this.m_sqlCmd.Parameters.Add(oParameter);
            }//end if (aryParameter != null)

            dsResult = new DataSet();
            oAdapter = new SqlDataAdapter(this.m_sqlCmd);

            try
            {
                oAdapter.Fill(dsResult);
                if (dsResult.Tables == null || dsResult.Tables.Count <= 0)
                {
                    dsResult.Dispose();
                    dsResult = null;
                }
                else if (dsResult.Tables[0].Rows.Count <= 0)
                {
                    dsResult.Dispose();
                    dsResult = null;
                }
            }
            catch (Exception ex)
            {
                oAdapter.Dispose();
                throw (ex);
            }
            finally
            {
                if (this.m_sqlCmd != null)
                {
                    this.m_sqlCmd.Dispose();
                    this.m_sqlCmd = null;
                }
                if (this.m_sqlConn != null)
                {
                    this.m_sqlConn.Dispose();
                    this.m_sqlConn = null;
                }
            }//end try

            return dsResult;
        }//end Query

        /// <summary>
        /// Execute query without parameters
        /// </summary>
        /// <param name="arg_strSql">SQL command string</param>
        /// <returns>has data: return data table, else return null</returns>
        public DataSet Query(string arg_strSql)
        {
            return this.Query(arg_strSql, null);
        }//end Query


    }//DbQuery
}//namespace