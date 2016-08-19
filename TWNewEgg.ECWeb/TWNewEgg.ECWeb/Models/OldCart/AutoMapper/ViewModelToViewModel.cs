using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace TWNewEgg.ECWeb.Models.OldCart.AutoMapper
{
    public class ViewModelToViewModel
    {
        public static void RegisterRules()
        {
            #region Cart
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Cart.CartStep2Data, TWNewEgg.Website.ECWeb.Models.InsertSalesOrdersBySellerInput>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Cart.DemainInsertSalesOrdersBySellerOutput, TWNewEgg.Website.ECWeb.Models.InsertSalesOrdersBySellerOutput>().ReverseMap();
            #endregion
            #region RecentOrderItem
            Mapper.CreateMap<TWNewEgg.Website.ECWeb.Models.RecentOrderItem, TWNewEgg.Models.ViewModels.MyAccount.OrderHistory>().ReverseMap();
           #endregion
            #region SalesOrder
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.SalesOrder, TWNewEgg.Models.ViewModels.MyAccount.SalceOrder>().ReverseMap();
            #endregion
            #region SalesOrderItem
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.SalesOrderItem, TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem>().ReverseMap();


            #endregion
            #region Process
            Mapper.CreateMap<TWNewEgg.DB.TWBACKENDDB.Models.Process, TWNewEgg.Models.ViewModels.MyAccount.SalesOrderItem>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Title))
                .ForMember(x => x.DisplayPrice, opt => opt.MapFrom(x => x.Price));



            #endregion
            #region Process
            Mapper.CreateMap<TWNewEgg.DB.TWBACKENDDB.Models.Cart, TWNewEgg.Models.ViewModels.MyAccount.ReturnPost>().ReverseMap();


            #endregion

        }
    }
}