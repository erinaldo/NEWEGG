using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace TWNewEgg.ECWeb.PrivilegeFilters.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireHttpsAndAuthsAttribute : AuthorizationFilterAttribute
    {
        private bool _activeAuth = true;

        public RequireHttpsAndAuthsAttribute()
        {

        }

        private static bool SkipSecures(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowNonSecuresAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowNonSecuresAttribute>().Any();
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        public RequireHttpsAndAuthsAttribute(bool activeAuth)
        {
            this._activeAuth = activeAuth;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }
            
            if (!SkipSecures(actionContext) && !CheckSecures.CheckSSLConnection(actionContext))
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "HTTPS Required"
                };
            }

            if(!this._activeAuth || SkipAuthorization(actionContext))
            {
                return;
            }


            //TODO(bw52): Parse auth head...
            //var parseHead = ParseAuthorizationHead(actionContext);
            //Parse cookies...
            string authValue = ParseAuthorizationHead(actionContext);

            //TODO(bw52): Check auth head to DB or...
            //var bool  = OnAuthorizeAccount(parseHead);

            //TODO(bw52): New Principal
            //var principal = new GenericPrincipal(identity, null);
            //Thread.CurrentPrincipal = principal;
            // inside of ASP.NET this is required
            //if (HttpContext.Current != null)
            //    HttpContext.Current.User = principal;
            IPrincipal customPrincipal;
            if (!string.IsNullOrEmpty(authValue) && authValue != TWNewEgg.Framework.Common.Cryptography.AESCryptography.DECRYPTFAIL)
            {
                customPrincipal = new CustomPrincipal(authValue);
                #region May change it in the future
                ////TODO(bw52): Check IP Addresss ?????!!!!
                //if ((customPrincipal.Identity as CustomIdentity).IPAddress != Core.NetPacketUtilities.GetUserIPAddress())
                //{
                //    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                //    {
                //        ReasonPhrase = "Authorize Required"
                //    };
                //}
                //else
                //{
                //    Thread.CurrentPrincipal = customPrincipal;

                //    if (HttpContext.Current != null)
                //    {
                //        HttpContext.Current.User = customPrincipal;
                //    }
                //}
                #endregion
                Thread.CurrentPrincipal = customPrincipal;

                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = customPrincipal;
                }
            }
            else
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "Authorize Required"
                };
            }
            base.OnAuthorization(actionContext);
        }

        protected virtual string ParseAuthorizationHead(HttpActionContext actionContext)
        {
            string authHeader = string.Empty;
            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null && auth.Scheme == "Basic")
            {
                authHeader = auth.Parameter;
            }

            if (string.IsNullOrEmpty(authHeader))
            {
                //Core.IEncapsulationAuthCookies encap = new Core.EncapsulationAuthCookies();
                //authHeader = encap.DeEncapsulate();
                //return authHeader;
                return string.Empty;
            }

            List<string> authCodes = authHeader.Split('&').ToList();
            var aiAuth = authCodes.Where(x => x.StartsWith("ai=")).FirstOrDefault();
            if (aiAuth == null)
            {
                return string.Empty;
            }

            authHeader = aiAuth.Remove(0, 3);

            if (authHeader.Length < 4)
            {
                return null;
            }

            return decryptAuth(authHeader);
        }

        private string decryptAuth(string authHeader)
        {
            string createIV = authHeader.Substring(0, 3);
            string encTok = authHeader.Substring(3);
            string decString = TWNewEgg.Framework.Common.Cryptography.AESCryptography.AESDecrypt(encTok, ivString: createIV);
            return decString;
        }

        protected virtual bool OnAuthorizeAccount(string value, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            //TODO(bw52): Default auth user method...
            return true;
        }

        
    }
}
