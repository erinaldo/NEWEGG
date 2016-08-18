using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.CartService
{
    public class OutputMessage //Web api output message, this should be del, coz this should be a XML or file.
    {
        public static string wrongOP = "Wrong OP";
        public static string loginError = "Login Error";

        
        public static string over25 = "Over 25";
        public static string doNothing = "Do Nothing";
        public static string noData = "No Data";
        public static string noQty = "No Qty";
        public static string hadAlready = "Had Already";
        public static string hadAlreadyWish = "Had in Wish Already";
        public static string timeOut = "Item_Time_Out";

        public static string dbAddFail = "DB Add Failed";
        public static string dbRemoveFail = "DB Remove Failed";
        public static string dbUpdateFail = "DB Update Failed";

        public static string addSuccess = "Add Successful";
        public static string addException = "Add Exception";
        public static string removeSuccess = "Remove Successful";
        public static string removeException = "Remove Exception";
        public static string updateSuccess = "Update Successful";
        public static string updateException = "Update Exception";

        public static string cookiesError = "Cookie Error";
        public static string accountError = "Account Error";
        public static string accountNull = "Account Null";



    }
}