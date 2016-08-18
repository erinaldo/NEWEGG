using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.ECWeb.PrivilegeFilters;


namespace TWNewEgg.ECWeb.Auth
{
    public class NEUser
    {
        public static bool IsAuthticated
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated ? (HttpContext.Current.User.Identity as CustomIdentity).IsAuthenticated : false; }
        }
        public static string AuthType
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated ? (HttpContext.Current.User.Identity as CustomIdentity).AuthenticationType : string.Empty; }
        }
        public static string Email
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated ? (HttpContext.Current.User.Identity as CustomIdentity).Email : string.Empty; }
        }
        public static string Name
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated ? (HttpContext.Current.User.Identity as CustomIdentity).Name : string.Empty; }
        }
        public static string NickName
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated ? (HttpContext.Current.User.Identity as CustomIdentity).NickName : string.Empty; }
        }
        public static DateTime LoginTime
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated ? (HttpContext.Current.User.Identity as CustomIdentity).LoginTime : new DateTime(); }
        }
        public static int ID
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated ? (HttpContext.Current.User.Identity as CustomIdentity).ID : new int(); }
        }
        public static string Scopes
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated ? (HttpContext.Current.User.Identity as CustomIdentity).Scopes : string.Empty; }
        }
        public static string IPAddress
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated ? (HttpContext.Current.User.Identity as CustomIdentity).IPAddress : string.Empty; }
        }
        public static string Browser
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated ? (HttpContext.Current.User.Identity as CustomIdentity).Browser : string.Empty; }
        }
    }
}