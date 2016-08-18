using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TWNewEgg.Framework.Cache;
using TWNewEgg.Framework.Common;
using TWNewEgg.Framework.Common.SMTP.Models;
using TWNewEgg.Framework.Common.SMTP.Service;
using TWNewEgg.Framework.ServiceApi.Configuration;
using TWNewEgg.Utility;
namespace TWNewEgg.ActionFilters
{
    public class SendExceptionMail : HandleErrorAttribute
    {
        /// <summary>
        /// 寄信群組(參考ExceptionMailList)
        /// </summary>
        public string MailGroup { get; set; }

        /// <summary>
        /// 信件主旨
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 信件內容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 複寫OnException方法，例外發生時觸發，將錯誤訊息紀錄
        /// </summary>
        /// <param name="filterContext">原方法參數: ExceptionContext</param>
        public override void OnException(System.Web.Mvc.ExceptionContext filterContext)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            string message = GenerateExceptionMessage(filterContext.Exception);
            logger.Error(message);
            if (Content == null)
            {
                Content = message;
            }

            ExceptionHandler.SendErrorMail(MailGroup, Subject, Content);

            filterContext.ExceptionHandled = true;
        }

        /// <summary>
        /// 產生詳細的錯誤訊息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string GenerateExceptionMessage(Exception ex)
        {
            string errorMessage = string.Empty;
            if (ex is DbEntityValidationException)
            {
                DbEntityValidationException dbEx = (DbEntityValidationException)ex;
                List<string> errorMessageList = dbEx.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage).ToList();
                errorMessage = string.Join("," + Environment.NewLine, errorMessageList);
            }
            else
            {
                errorMessage = ex.ToString();
            }

            //回傳錯誤訊息
            return errorMessage;
        }
    }
}
