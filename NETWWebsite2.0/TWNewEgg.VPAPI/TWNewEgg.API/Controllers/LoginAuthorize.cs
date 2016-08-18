using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TWNewEgg.API.Filters
{
    public class LoginAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool result = true;
            
            return result;
        }
    }
}
