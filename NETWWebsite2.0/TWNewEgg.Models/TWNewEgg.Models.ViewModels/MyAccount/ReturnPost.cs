using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Models.ViewModels.MyAccount
{
    public class ReturnPost
    {
        public int totalpage { get; set; }
        public int Sex { get; set; }
        //姓氏
        public string Lastname { get; set; }
        //名字
        public string Firstname { get; set; }
        public string Nickname { get; set; }
        public string Code { get; set; }
        public string PayType { get; set; }
        public string Username { get; set; }
        public string Location { get; set; }
        public string Zipcode { get; set; }
        public string ADDR { get; set; }
        public string TelDay { get; set; }
        public string RecvMobile { get; set; }
        public string bankid { get; set; }
        public string bankbranch { get; set; }
        public string Paytypeid { get; set; }
        public string Bankaccount { get; set; }
        public List<Process> processlist { get; set; }
        public string salesOrder_code { get; set; }
        public string CreateDate { get; set; }
        public string Receiver { get; set; }
        public string SalesOrder_itemname { get; set; }
        public string Phone { get; set; }
        public string TelZip { get; set; }
        public string TelDay2 { get; set; }
        public string Email { get; set; }
        public string TelExtension { get; set; }
        public Nullable<int> ShipType { get; set; }
        //郵遞區號加區
        public string addr { get; set; }
        public string address { get; set; }
        public bool Paytypeboolen { get; set; }
        public string SalceorderCodeList { get; set; }
       
    }
    public class Process 
    {
        public string code { get; set; }
      
        public string ProductName { get; set; }
        public int PriceCash { get; set; }
    
    }
}