using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TWNewEgg.DB;

namespace TWNewEgg.API.View
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ServiceApiConfig.Bootstrapper();
            // AutoMapper 初始化
            AutoMapperConfig.Configure();

            //自動建立Action清單 寫入資料庫
            BuildActionList();

            //Log4Net 設定檔
            string log4netPath = Server.MapPath("~/Configurations/log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(log4netPath));
            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            //將 Cookies 的 MyLang 取出，主要是要指定語系
            HttpCookie MyLang = Request.Cookies["MyLang"];
            if (MyLang != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture =
                 new System.Globalization.CultureInfo(MyLang.Value);
                System.Threading.Thread.CurrentThread.CurrentUICulture =
                 new System.Globalization.CultureInfo(MyLang.Value);
            }
        }

        /// <summary>
        /// 掃描TWNewEgg.API.Controllers命名空間中所有類別的Method，
        /// 將有標記方法描述(TWNewEgg.API.Attributes.ActionDescription)的Public Method寫入到DB。
        /// </summary>
        private void BuildActionList()
        {
            //DB.TWBackendDBContext db = new DB.TWBackendDBContext();
            DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

            //Reset Action To Disable
            //List<DB.TWBACKENDDB.Models.API_Action> actions = db.API_Action.ToList();
            //actions.ForEach(x => x.SetEnable(false));

            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            foreach (Type type in asm.GetTypes())
            {
                string fullName = type.FullName;
                if (fullName.IndexOf("TWNewEgg.API.View") >= 0)
                {
                    string controllerName = type.Name.ToLower().Replace("controller", "");
                    object[] methods = type.GetMethods();
                    foreach (System.Reflection.MethodInfo method in methods)
                    {
                        if (method.Module.Name.IndexOf("TWNewEgg.API") >= 0 && method.IsPublic)
                        {
                            string actionName = method.Name;
                            object[] attributes = method.GetCustomAttributes(false);
                            Attributes.ActionDescriptionAttribute actionDescAttr = null;
                            Attributes.FunctionCategoryNameAttribute funcCategoryNameAttr = null; // Ron
                            Attributes.FunctionNameAttribute funcNameAttr = null;                 // Ron
                            Attributes.ActiveKeyAttribute activeKeyAttr = null;                   // Ron
                            System.Web.Mvc.HttpGetAttribute httpGetAttr = null;
                            System.Web.Mvc.HttpPostAttribute httpPostAttr = null;
                            System.Web.Mvc.HttpPutAttribute httpPutAttr = null;
                            System.Web.Mvc.HttpDeleteAttribute httpDeleteAttr = null;
                            //Fetch Attribute
                            foreach (object attr in attributes)
                            {
                                if (attr is Attributes.ActionDescriptionAttribute)
                                {
                                    actionDescAttr = (Attributes.ActionDescriptionAttribute)attr;
                                }
                                else if (attr is Attributes.FunctionCategoryNameAttribute)                   // Ron
                                {
                                    funcCategoryNameAttr = (Attributes.FunctionCategoryNameAttribute)attr;
                                }
                                else if (attr is Attributes.FunctionNameAttribute)                           // Ron
                                {
                                    funcNameAttr = (Attributes.FunctionNameAttribute)attr;
                                }
                                else if (attr is Attributes.ActiveKeyAttribute)                              // Ron
                                {
                                    activeKeyAttr = (Attributes.ActiveKeyAttribute)attr;
                                }
                                else if (attr is System.Web.Mvc.HttpGetAttribute)
                                {
                                    httpGetAttr = (System.Web.Mvc.HttpGetAttribute)attr;
                                }
                                else if (attr is System.Web.Mvc.HttpPostAttribute)
                                {
                                    httpPostAttr = (System.Web.Mvc.HttpPostAttribute)attr;
                                }
                                else if (attr is System.Web.Mvc.HttpPutAttribute)
                                {
                                    httpPutAttr = (System.Web.Mvc.HttpPutAttribute)attr;
                                }
                                else if (attr is System.Web.Mvc.HttpDeleteAttribute)
                                {
                                    httpDeleteAttr = (System.Web.Mvc.HttpDeleteAttribute)attr;
                                }
                            }
                            string httpMethod = "";
                            //HttpGetAttribute
                            if (httpGetAttr != null)
                            {
                                httpMethod += "[Get]";
                            }
                            //HttpPostAttribute
                            if (httpPostAttr != null)
                            {
                                httpMethod += "[Post]";
                            }
                            //HttpPutAttribute
                            if (httpPutAttr != null)
                            {
                                httpMethod += "[Put]";
                            }
                            //HttpDeleteAttribute
                            if (httpDeleteAttr != null)
                            {
                                httpMethod += "[Delete]";
                            }

                            //ActionDescriptionAttribute && FunctionCategoryNameAttribute && FunctionNameAttribute && ActiveKeyAttribute
                            if (actionDescAttr != null && funcCategoryNameAttr != null && funcNameAttr != null && activeKeyAttr != null && !string.IsNullOrEmpty(httpMethod))
                            {
                                string desc = actionDescAttr.Description;
                                string category = funcCategoryNameAttr.FunctionCategoryName;
                                string functionName = funcNameAttr.FunctionName;
                                string activeKey = activeKeyAttr.ActiveKey;
                                TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_Action action = db.Seller_Action.Where(x => x.ControllerName == controllerName && x.ActionName == actionName && x.HttpMethod == httpMethod).FirstOrDefault();
                                //DB.TWBACKENDDB.Models.API_Action action = db.API_Action.Where(x => x.ControllerName == controllerName && x.ActionName == actionName && x.HttpMethod==httpMethod).FirstOrDefault();
                                if (action == null)
                                {
                                    //Action not Exsit，insert Action into Seller_Action
                                    action = new DB.TWSELLERPORTALDB.Models.Seller_Action();
                                    action.ControllerName = controllerName;
                                    action.ActionName = actionName;
                                    action.InDate = DateTime.UtcNow.AddHours(8);
                                    action.InUserID = 0;
                                    db.Seller_Action.Add(action);
                                }
                                else
                                {
                                    //Action Exsit
                                    action.UpdateDate = DateTime.UtcNow.AddHours(8);
                                    action.UpdateUserID = 0;
                                }
                                action.SetEnable(true);
                                action.HttpMethod = httpMethod;

                                //update ActionDescription
                                action.ActionDescription = desc;

                                //update FunctionNameAttribute
                                TWNewEgg.DB.TWSELLERPORTALDB.Models.EDI_Seller_Function function = db.EDI_Seller_Function.Where(r => r.FunctionName.Replace(" ", string.Empty).ToUpper() == functionName.Replace(" ", string.Empty).ToUpper()).FirstOrDefault();
                                if (function != null)
                                {
                                    action.FunctionID = function.FunctionID;
                                    action.FunctionName = function.FunctionName;
                                }

                                //update ActiveKeyAttribute
                                action.FNActiveKey = activeKey;

                                //update FunctionCategoryNameAttribute
                                action.FNCategoryName = category;
                            }

                        }
                    }
                }
            }
            db.SaveChanges();
        }
    }
}
