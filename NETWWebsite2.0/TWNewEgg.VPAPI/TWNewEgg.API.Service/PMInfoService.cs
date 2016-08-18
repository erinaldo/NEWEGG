using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using LinqToExcel;
using Remotion;
using System.Text.RegularExpressions;
using System.Web;
using System.Transactions;
using log4net;
using log4net.Config;
using System.Threading;
using TWNewEgg.API.Models.Models;

namespace TWNewEgg.API.Service
{
    public class PMInfoService
    {
        TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();

        private static ILog log = LogManager.GetLogger(typeof(PMInfoService));

        /// <summary>
        /// 搜尋全部 ItemTemp 未審核商品低於館價
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> CheckGrossMargin()
        {
            decimal price = new decimal();
            decimal cost = new decimal();
            decimal vendorGrossMargin = new decimal();
            decimal pmGrossMargin = new decimal();

            Dictionary<int, int> result = new Dictionary<int, int>();

            List<TWNewEgg.DB.TWSQLDB.Models.ItemTemp> itemTemps = new List<DB.TWSQLDB.Models.ItemTemp>();
            List<TWNewEgg.DB.TWSQLDB.Models.ProductTemp> productTemps = new List<DB.TWSQLDB.Models.ProductTemp>();

            itemTemps = db.ItemTemp.Where(x => x.Status == 1 && (x.ApproveMan == string.Empty || x.ApproveMan == null) && x.ItemID ==0).ToList();

            List<int> productIDs = itemTemps.Where(x=>x.ProduttempID.HasValue).Select(x => x.ProduttempID.Value).ToList();

            productTemps = db.ProductTemp.Where(x => productIDs.Contains(x.ID)).ToList();

            var categorys = db.Category.Select(x => new { x.ID, x.GrossMargin }).ToList();

            foreach (var index in itemTemps)
            {
                price = index.PriceCash;
                cost = productTemps.Where(x => x.ID == index.ProduttempID).Select(r => r.Cost.GetValueOrDefault()).FirstOrDefault();

                pmGrossMargin = categorys.Where(x => x.ID == index.CategoryID).Select(r => r.GrossMargin).FirstOrDefault();

                try
                {
                    vendorGrossMargin = decimal.Round(((price - cost) / price) * 100, 2);

                    if (vendorGrossMargin < pmGrossMargin)
                    {
                        result.Add(index.ID, index.CategoryID);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Msg: " + ex.Message + " ,StackTrace: " + ex.StackTrace);
                    continue;
                }
            }
            return result;
        }

        public Dictionary<int, int> CheckGrossMargin(List<int> itemSketchIDs)
        {
            decimal price = new decimal();
            decimal cost = new decimal();
            decimal vendorGrossMargin = new decimal();
            decimal pmGrossMargin = new decimal();

            Dictionary<int, int> result = new Dictionary<int, int>();

            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketch> itemSketchs = new List<DB.TWSQLDB.Models.ItemSketch>();

            var json_itemSketchIDs = Newtonsoft.Json.JsonConvert.SerializeObject(itemSketchIDs);
            log.Info("送審開始 IDs: " + json_itemSketchIDs);
            

            itemSketchs = db.ItemSketch.Where(x => itemSketchIDs.Contains(x.ID) 
                && x.PriceCash.HasValue && x.Cost.HasValue 
                && x.itemtempID.HasValue && x.ProducttempID.HasValue
                && x.CategoryID.HasValue).ToList();
           
            log.Info("DB 取得: " + itemSketchs.Count + "筆");

            var categorys = db.Category.Select(x => new { x.ID, x.GrossMargin }).ToList();

            foreach (var index in itemSketchs)
            {
                price = index.PriceCash.Value;
                cost = index.Cost.Value;

                pmGrossMargin = categorys.Where(x => x.ID == index.CategoryID.Value).Select(r => r.GrossMargin).FirstOrDefault();

                try
                {
                    if (price > 0)
                    {
                        vendorGrossMargin = decimal.Round(((price - cost) / price) * 100, 2);

                        if (vendorGrossMargin < pmGrossMargin)
                        {
                            log.Info("ItemSketch ID 低於館價: " + index.ID + ", CategoryID: " + index.CategoryID);
                            result.Add(index.itemtempID.Value, index.CategoryID.Value);
                        }
                    }                
                }
                catch (Exception ex)
                {
                    log.Error("Msg: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                    continue;
                }
            }
            return result;
        }

        /// <summary>
        /// 利用 ItemTempID，寄送PM館價信
        /// </summary>
        /// <param name="ItemTempID">ItemTempID</param>
        /// <param name="PMMails">PMMails</param>
        /// <returns></returns>
        public string SendPMGrossMarginMial(int ItemTempID, List<string> PMMails)
        {
            if (PMMails.Count > 0)
            {
                TWNewEgg.API.Models.Connector conn = new Connector();
                var itemtemp = db.ItemTemp.Where(x => x.ID == ItemTempID).FirstOrDefault();

                string message = "供應商: " + itemtemp.SellerID + " 的商品: " + itemtemp.ID + " " + itemtemp.Name + " 的毛利率低於館別設定值。";
                log.Info("供應商(itemtemp.SellerID): " + itemtemp.SellerID + " 的商品(itemtemp.ID): " + itemtemp.ID + " (itemtemp.Name): " + itemtemp.Name + " 的毛利率低於館別設定值。");
                Mail pmmail = new Mail();
                pmmail.MailType = Mail.MailTypeEnum.RemindGrossMargin;
                pmmail.MailMessage = message;
                pmmail.UserEmail = PMMails[0];

                if (PMMails.Count > 1)
                {
                    for (int index = 1; index < PMMails.Count; index++)
                    {
                        // 加上多個收件人的分隔符號
                        if (!string.IsNullOrEmpty(pmmail.RecipientBcc))
                        {
                            pmmail.RecipientBcc += ",";
                        }

                        pmmail.RecipientBcc += PMMails[index];
                    }
                }

                conn.SendMail(null, null, pmmail);
            }

            return string.Empty;
        }

        
    }
}
