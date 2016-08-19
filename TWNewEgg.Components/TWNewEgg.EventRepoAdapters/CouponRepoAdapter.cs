using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.EventRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.EventRepoAdapters
{
    public class CouponRepoAdapter:ICouponRepoAdapter
    {
        private IRepository<Coupon> _CouponRepo = null;
        private IRepository<CouponActivity> _CouponActivity = null;

        public CouponRepoAdapter(IRepository<Coupon> argCouponRepo, IRepository<CouponActivity> argCouponActivity)
        {
            this._CouponRepo = argCouponRepo;
            this._CouponActivity = argCouponActivity;
        }
        public IQueryable<Coupon> Coupon_GetAll()
        {
            IQueryable<Coupon> queryResult = null;

            queryResult = this._CouponRepo.GetAll();

            return queryResult;
        }


        public IQueryable<CouponActivity> CouponActivity()
        {
            IQueryable<CouponActivity> queryActivityResult = null;
       
            queryActivityResult = this._CouponActivity.GetAll();

            return queryActivityResult;

        }
        public void CreateCoupon(Coupon argObjCoupon)
        {
            try
            {
                this._CouponRepo.Create(argObjCoupon);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        public bool UpdateCoupon(Coupon argObjCoupon)
        {
            bool boolExec = false;
            Coupon objDbCoupon = null;

            objDbCoupon = this._CouponRepo.GetAll().Where(x => x.id == argObjCoupon.id).FirstOrDefault();

            if (objDbCoupon != null)
            {
                try
                {
                    ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.Coupon, Models.DBModels.TWSQLDB.Coupon>(argObjCoupon, objDbCoupon);
                    this._CouponRepo.Update(objDbCoupon);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw new NotImplementedException(ex.Message, ex);
                }
            }

            return boolExec;
        }

        public bool UpdateCouponRange(List<Coupon> argListCoupon)
        {
            bool boolExec = false;

            if (argListCoupon == null || argListCoupon.Count <= 0)
            {
                return false;
            }

            try
            {
                this._CouponRepo.UpdateRange(argListCoupon);
                boolExec = true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return boolExec;
        }

        public IQueryable<Coupon> GetCouponById(int argNumId)
        {
            IQueryable<Coupon> queryResult = null;

            queryResult = this.Coupon_GetAll().Where(x => x.id == argNumId);
            return queryResult;
        }

        public int addDynamicCouponByEventIdAndUserAccount(int arg_nEventId, string arg_strAccount)
        {
            int nResult = 0;
            System.Data.Common.DbDataReader cmdReader = null;
            System.Data.Common.DbCommand cmd = null;

            cmd = this._CouponRepo.GetDatabase().Connection.CreateCommand();
            cmd.Connection.Open();
            SqlParameter paramOne = new SqlParameter();
            paramOne.ParameterName = "@EventId";
            paramOne.Value = arg_nEventId;
            cmd.Parameters.Add(paramOne);

            paramOne = new SqlParameter();
            paramOne.ParameterName = "@UserAccountId";
            paramOne.Value = arg_strAccount;
            cmd.Parameters.Add(paramOne);

            cmd.CommandText = "[dbo].[UP_EC_GrantCoupon_Dynamic]@EventId, @UserAccountId";

            try
            {
                cmdReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                if (cmdReader.HasRows)
                {
                    cmdReader.Read();
                    nResult = cmdReader.GetInt32(0); //this four are local buynext, wishlist and oversea buynext, wishlist.
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            finally
            {
                if (cmd.Connection.State != ConnectionState.Closed)
                {
                    cmd.Connection.Close();
                    cmd.Dispose();
                }
            }

            return nResult;
        }

        public void ExecGrantCouponAllUser()
        {

            try
            {
                //this._CouponRepo.GetDatabase().SqlQuery<string>("declare @dboutput varchar(100); exec UP_EC_GrantCoupon_AllUser @dboutput").ToList();

                int nResult = 0;
                System.Data.Common.DbCommand cmd = null;

                cmd = this._CouponRepo.GetDatabase().Connection.CreateCommand();
                cmd.Connection.Open();
                cmd.CommandTimeout = 7200;

                cmd.CommandText = "declare @dboutput varchar(100); exec UP_EC_GrantCoupon_AllUser @dboutput";

                try
                {
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    throw new NotImplementedException(ex.Message, ex);
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                        cmd.Dispose();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }
    }
}
