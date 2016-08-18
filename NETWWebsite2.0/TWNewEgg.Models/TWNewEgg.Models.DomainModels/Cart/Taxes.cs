using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class Taxes
    {
        public Taxes(decimal _tariff, decimal _vAT, decimal _exciseTax, decimal _tradeRromotionServiceFee) {
            this.TotalTaxes = _tariff + _vAT + _exciseTax + _tradeRromotionServiceFee;
            this.Tariff = _tariff;
            this.VAT = _vAT;
            this.ExciseTax = _exciseTax;
            this.TradeRromotionServiceFee = _tradeRromotionServiceFee;
        }
        // 稅賦總計
        public decimal TotalTaxes { get; set; }
        // 關稅
        public decimal Tariff { get; set; }
        // 增值稅
        public decimal VAT { get; set; }
        // 貨物稅
        public decimal ExciseTax { get; set; }
        // 推廣貿易服務費
        public decimal TradeRromotionServiceFee { get; set; }
    }
}
