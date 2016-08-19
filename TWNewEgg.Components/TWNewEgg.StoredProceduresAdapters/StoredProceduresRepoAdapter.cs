using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels;
using System.Data;
using System.Data.Entity;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using TWNewEgg.StoredProceduresRepoAdapters.Interface;

namespace TWNewEgg.StoredProceduresRepoAdapters
{
    public class StoredProceduresRepoAdapter : IStoredProceduresRepoAdapter
    {
        private string twsqlDBConnectionString;

        public Dictionary<string, List<ViewTracksCartItems>> GetShoppingAllCart(int accountId)
        {
            string twsqlDBConnectionString = ConfigurationManager.ConnectionStrings["TWSqlDBConnection"].ConnectionString;
            if (string.IsNullOrEmpty(twsqlDBConnectionString))
            {
                throw new Exception("There is no connection string in Web.Config.");
            }
            using (var twSqlDB = new TWSqlDBContext(twsqlDBConnectionString))
            {
                twSqlDB.Database.Initialize(force: false);
                var cmd = twSqlDB.Database.Connection.CreateCommand();
                SqlParameter paramOne = new SqlParameter();
                paramOne.ParameterName = "@account_id";
                paramOne.Value = accountId;
                cmd.Parameters.Add(paramOne);

                cmd.CommandText = "[dbo].[UP_EC_ShoppingCartGetItemV3] @account_id";
                try
                {
                    Dictionary<string, List<ViewTracksCartItems>> ViewTracksCartItemsList = new Dictionary<string, List<ViewTracksCartItems>>();

                    twSqlDB.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    //Get oversea buynow item
                    List<ViewTracksCartItems> overSeaBuyNow = ((IObjectContextAdapter)twSqlDB).ObjectContext.Translate<ViewTracksCartItems>(reader, "ViewTracksCartItems", MergeOption.NoTracking).ToList();
                    ViewTracksCartItemsList.Add(ViewTracksCartItems.ViewTracksType.海外購物車.ToString(), ExcludeRepeatItems(overSeaBuyNow));
                    reader.NextResult();
                    //Get oversea buynext item
                    List<ViewTracksCartItems> overSeaBuyNext = ((IObjectContextAdapter)twSqlDB).ObjectContext.Translate<ViewTracksCartItems>(reader, "ViewTracksCartItems", MergeOption.NoTracking).ToList();
                    ViewTracksCartItemsList.Add(ViewTracksCartItems.ViewTracksType.海外下次買.ToString(), ExcludeRepeatItems(overSeaBuyNext));
                    reader.NextResult();
                    //Get oversea wishlist item
                    List<ViewTracksCartItems> overSeaTrackNext = ((IObjectContextAdapter)twSqlDB).ObjectContext.Translate<ViewTracksCartItems>(reader, "ViewTracksCartItems", MergeOption.NoTracking).ToList();
                    ViewTracksCartItemsList.Add(ViewTracksCartItems.ViewTracksType.海外追蹤.ToString(), ExcludeRepeatItems(overSeaTrackNext));
                    reader.NextResult();
                    //Get local buynow item
                    List<ViewTracksCartItems> buyNow = ((IObjectContextAdapter)twSqlDB).ObjectContext.Translate<ViewTracksCartItems>(reader, "ViewTracksCartItems", MergeOption.NoTracking).ToList();
                    ViewTracksCartItemsList.Add(ViewTracksCartItems.ViewTracksType.一般宅配.ToString(), ExcludeRepeatItems(buyNow));
                    reader.NextResult();
                    //Get local buynext item
                    List<ViewTracksCartItems> buyNext = ((IObjectContextAdapter)twSqlDB).ObjectContext.Translate<ViewTracksCartItems>(reader, "ViewTracksCartItems", MergeOption.NoTracking).ToList();
                    ViewTracksCartItemsList.Add(ViewTracksCartItems.ViewTracksType.一般下次買.ToString(), ExcludeRepeatItems(buyNext));
                    reader.NextResult();
                    //Get local wishlist item
                    List<ViewTracksCartItems> trackNext = ((IObjectContextAdapter)twSqlDB).ObjectContext.Translate<ViewTracksCartItems>(reader, "ViewTracksCartItems", MergeOption.NoTracking).ToList();
                    ViewTracksCartItemsList.Add(ViewTracksCartItems.ViewTracksType.一般追蹤.ToString(), ExcludeRepeatItems(trackNext));
                    reader.NextResult();
                    //Get 國內購物車加價商品
                    List<ViewTracksCartItems> trackNowadditional = ((IObjectContextAdapter)twSqlDB).ObjectContext.Translate<ViewTracksCartItems>(reader, "ViewTracksCartItems", MergeOption.NoTracking).ToList();
                    ViewTracksCartItemsList.Add(ViewTracksCartItems.ViewTracksType.國內購物車加價商品.ToString(), ExcludeRepeatItems(trackNowadditional));
                    reader.NextResult();
                    //Get 任選館購物車加價商品
                    List<ViewTracksCartItems> trackoverSeaadditional = ((IObjectContextAdapter)twSqlDB).ObjectContext.Translate<ViewTracksCartItems>(reader, "ViewTracksCartItems", MergeOption.NoTracking).ToList();
                    ViewTracksCartItemsList.Add(ViewTracksCartItems.ViewTracksType.海外購物車加價商品.ToString(), ExcludeRepeatItems(trackoverSeaadditional));
                    reader.NextResult();
                    //Get 任選館購物車加價商品
                    List<ViewTracksCartItems> trackAnyadditional = ((IObjectContextAdapter)twSqlDB).ObjectContext.Translate<ViewTracksCartItems>(reader, "ViewTracksCartItems", MergeOption.NoTracking).ToList();
                    ViewTracksCartItemsList.Add(ViewTracksCartItems.ViewTracksType.任選館購物車加價商品.ToString(), ExcludeRepeatItems(trackAnyadditional));
                    twSqlDB.Database.Connection.Close();

                    return ViewTracksCartItemsList;
                }
                catch (Exception e)
                {
                    return null;
                }

                return null;
            }
        }

        public List<ViewTracksCartItems> ExcludeRepeatItems(List<ViewTracksCartItems> ViewTracksCartItems) {
            List<ViewTracksCartItems> ViewTracksCartItemsResult = new List<ViewTracksCartItems>();
            if (ViewTracksCartItems != null && ViewTracksCartItems.Count > 0)
            {
                ViewTracksCartItemsResult = ViewTracksCartItems.GroupBy(x => x.ItemID).Select(x => x.OrderByDescending(y=>y.TrackCreateDate).FirstOrDefault()).ToList();
            }
            return ViewTracksCartItemsResult;
        }
        //        db.Database.Initialize(force: false);
        //        var cmd = db.Database.Connection.CreateCommand();

        //        SqlParameter paramOne = new SqlParameter();
        //        paramOne.ParameterName = "@account_id";
        //        paramOne.Value = accountId;
        //        cmd.Parameters.Add(paramOne);

        //        cmd.CommandText = "[dbo].[UP_EC_ShoppingCartGetItemV2] @account_id";

        //        try
        //        {
        //            db.Database.Connection.Open();
        //            var reader = cmd.ExecuteReader();

        //            //Get oversea buynow item
        //            IEnumerable<ViewTracksCartItems> overSeaBuyNow = ((IObjectContextAdapter)db).ObjectContext.Translate<ViewTracksCartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
        //            reader.NextResult();
        //            //Get oversea buynext item
        //            IEnumerable<ViewTracksCartItems> overSeaBuyNext = ((IObjectContextAdapter)db).ObjectContext.Translate<ViewTracksCartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
        //            reader.NextResult();
        //            //Get oversea wishlist item
        //            IEnumerable<ViewTracksCartItems> overSeaTrackNext = ((IObjectContextAdapter)db).ObjectContext.Translate<ViewTracksCartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
        //            reader.NextResult();
        //            //Get local buynow item
        //            IEnumerable<ViewTracksCartItems> buyNow = ((IObjectContextAdapter)db).ObjectContext.Translate<ViewTracksCartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
        //            reader.NextResult();
        //            //Get local buynext item
        //            IEnumerable<ViewTracksCartItems> buyNext = ((IObjectContextAdapter)db).ObjectContext.Translate<ViewTracksCartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
        //            reader.NextResult();
        //            //Get local wishlist item
        //            IEnumerable<ViewTracksCartItems> trackNext = ((IObjectContextAdapter)db).ObjectContext.Translate<ViewTracksCartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();

        //            //var tracks = (from A in db.track where A.track_id == Accountid select A).ToList();
        //            //return tracks;
        //            db.Database.Connection.Close();
        //            if (OpCode == 1)
        //            {
        //                switch (sortCode)
        //                {
        //                    case 0:
        //                        return buyNow;
        //                        break;
        //                    case 1:
        //                        return buyNow.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
        //                        break;
        //                    default:
        //                        return buyNow;
        //                        break;

        //                }
        //            }
        //            if (OpCode == 3)
        //            {
        //                switch (sortCode)
        //                {
        //                    case 0:
        //                        return buyNext;
        //                        break;
        //                    case 1:
        //                        return buyNext.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
        //                        break;
        //                    default:
        //                        return buyNext;
        //                        break;

        //                }
        //            }
        //            if (OpCode == 5)
        //            {
        //                switch (sortCode)
        //                {
        //                    case 0:
        //                        return trackNext;
        //                        break;
        //                    case 1:
        //                        return trackNext.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
        //                        break;
        //                    default:
        //                        return trackNext;
        //                        break;

        //                }
        //            }
        //            if (OpCode == 7)
        //            {
        //                switch (sortCode)
        //                {
        //                    case 0:
        //                        return overSeaBuyNow;
        //                        break;
        //                    case 1:
        //                        return overSeaBuyNow.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
        //                        break;
        //                    default:
        //                        return overSeaBuyNow;
        //                        break;

        //                }
        //            }
        //            if (OpCode == 8)
        //            {
        //                switch (sortCode)
        //                {
        //                    case 0:
        //                        return overSeaBuyNext;
        //                        break;
        //                    case 1:
        //                        return overSeaBuyNext.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
        //                        break;
        //                    default:
        //                        return overSeaBuyNext;
        //                        break;

        //                }
        //            }
        //            if (OpCode == 9)
        //            {
        //                switch (sortCode)
        //                {
        //                    case 0:
        //                        return overSeaTrackNext;
        //                        break;
        //                    case 1:
        //                        return overSeaTrackNext.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
        //                        break;
        //                    default:
        //                        return overSeaTrackNext;
        //                        break;

        //                }
        //            }
        //            if (OpCode == 11)
        //            {
        //                List<ViewTracksCartItems> nowBuyList = new List<ViewTracksCartItems>();
        //                nowBuyList.AddRange(overSeaBuyNow);
        //                nowBuyList.AddRange(buyNow);
        //                switch (sortCode)
        //                {
        //                    case 0:
        //                        return nowBuyList;
        //                        break;
        //                    case 1:
        //                        return nowBuyList.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
        //                        break;
        //                    default:
        //                        return nowBuyList;
        //                        break;

        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            return null;
        //        }

        //        return null;
        //}
    }
}
