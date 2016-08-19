using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.Framework.Cache;
using TWNewEgg.Framework.ServiceApi.Configuration;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.PromotionRepoAdapters.Interface;
using TWNewEgg.SellerServices.Interface;
using TWNewEgg.AccountServices.Interface;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.AccountRepoAdapters.Interface;
using TWNewEgg.ShoppingCartServices.Interface;
using TWNewEgg.Models.DomainModels.PaymentGateway;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.DAL.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.Redeem;
using System.Web.Script.Serialization;
using TWNewEgg.Framework.FlowEngine;
using Autofac;
using System.Xml.Serialization;
using TWNewEgg.Framework.FlowEngine.Model;
using System.Xml;

namespace TWNewEgg.ECService.Controllers
{
    public class SampleController : Controller
    {
        //
        // GET: /Sample/


        public ActionResult Index()
        {
            var components = CacheConfiguration.Instance.GetFromCache<ServiceComponents>(ConfigurationManager.COMPONENTSCONFIG);
            ViewBag.PublicMethods = CacheConfiguration.Instance.GetFromCache<PublicMethods>(ConfigurationManager.PUBLICMETHODSCONFIG);
            return View(components);
        }

        public ActionResult PromotionGiftBasicSearch(int categoryID)
        {
            var promotionGiftInfo = AutofacConfig.Container.Resolve<IPromotionRepoAdapter>();
            //var result = promotionGiftInfo.GetPromotionGiftBasic(categoryID);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestSellerServices()
        {
            List<int> SellerList = new List<int>();
            SellerList.Add(1);
            SellerList.Add(2);
            SellerList.Add(3);

            var TestSellerServices = AutofacConfig.Container.Resolve<ISellerServices>();
            var result = TestSellerServices.GetSellerWithCountryList(SellerList);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestAccountInfoDM(int ID)
        {
            var TestAccountInfoDM = AutofacConfig.Container.Resolve<IAccountService>();
            TWNewEgg.Models.DomainModels.Account.AccountDM AccountDM = new TWNewEgg.Models.DomainModels.Account.AccountDM();
            AccountDM.Name = "TEST";
            AccountDM.Email = "TEST@" + ID.ToString()+".com";
            AccountDM.PWD = "TESTPWD";
            AccountDM.PWDtxt = "TESTPWDtxt";
            AccountDM.GuestLogin = 0;
            AccountDM.ReceiveEDM = 0;
            var result = TestAccountInfoDM.Register(AccountDM);
            return View();
        }

        public ActionResult GetMemberBookTest(int accID)
        {
            IMemberRepoAdapter memberRepoAdapter = AutofacConfig.Container.Resolve<IMemberRepoAdapter>();
            IAccountRepoAdapter accountRepoAdapter = AutofacConfig.Container.Resolve<IAccountRepoAdapter>();
            TWNewEgg.AccountServices.GetMemberService getMemberService = new TWNewEgg.AccountServices.GetMemberService(memberRepoAdapter, accountRepoAdapter);
            var result = getMemberService.GetCartMemberInfo(accID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult testItem()
        {
            IRepository<Item> itemRepo = AutofacConfig.Container.Resolve<IRepository<Item>>();
            List<Item> items = itemRepo.GetAll().Take(2).ToList();
            itemRepo.CreateMany(items);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestAutomapper()
        {
            IRepository<Models.DBModels.TWSQLDB.Item> itemRepo = AutofacConfig.Container.Resolve<IRepository<Models.DBModels.TWSQLDB.Item>>();
            var item = itemRepo.Get(x => x.ID == 2);
            var itemBase = ModelConverter.ConvertTo<ItemBase>(item);
            itemBase.Name = "日本山葉原廠Yamaha EPH-50耳道式耳機(簡約黑黑的)";
            ModelConverter.ConvertTo<ItemBase, Models.DBModels.TWSQLDB.Item>(itemBase, item);
            itemRepo.Update(item);
            return Content(item.Name);
        }

        public ActionResult TestCartTemp()
        {
            ICartTempService cartTempService = AutofacConfig.Container.Resolve<ICartTempService>();
            string cartSNO = cartTempService.GenerateSerialNumber(9, 1);
            //CreateCartTemp
            CartTemp cartTemp = new CartTemp();
            CartTempDM insertCartTempDM = new CartTempDM();
            insertCartTempDM.SerialNumber = cartSNO;
            insertCartTempDM.AccountID = 9;
            insertCartTempDM.PayType = 1;
            insertCartTempDM.BankID = 188;
            insertCartTempDM.PayTypeGroupID = 1;
            insertCartTempDM.CartTypeID = 1;
            insertCartTempDM.Status = (int)CartTempDM.StatusEnum.Initial;
            insertCartTempDM.CreateDate = DateTime.UtcNow.AddHours(8);
            insertCartTempDM.CreateUser = "tc59";
            List<int> itemIDList = new List<int> { 119261, 124710 };
            foreach (int itemID in itemIDList)
            {
                CartItemTempDM insertCartItemTempDM = new CartItemTempDM();
                insertCartItemTempDM.ItemID = itemID;
                insertCartItemTempDM.PriceCash = 0;
                insertCartItemTempDM.Qty = 2;
                insertCartItemTempDM.ServicePrice = 0;
                insertCartItemTempDM.ShippingPrice = 0;
                insertCartItemTempDM.Tax = 0;
                insertCartItemTempDM.CreateDate = DateTime.UtcNow.AddHours(8);
                insertCartItemTempDM.CreateUser = "system";
                insertCartItemTempDM.Category = 0;
                insertCartItemTempDM.Updated = 0;
                insertCartTempDM.CartItemTempDMs.Add(insertCartItemTempDM);
            }


            CartTempDM newCartTempDM = cartTempService.CreateCartTemp(insertCartTempDM);

            CartTempDM getCartTempDM = cartTempService.GetCartTempBySN(insertCartTempDM.SerialNumber);

            cartTempService.RemoveTimeoutCartTemps(-3, -3);

            //cartTempService.RemoveCartTemp(9, 3);

            return Json(newCartTempDM, JsonRequestBehavior.AllowGet);
        }

        public void TestFlow()
        {
            IFlowService service = AutofacConfig.Container.Resolve<IFlowService>();
            List<IFlowStep> steps = new List<IFlowStep>();

            XmlSerializer ser = new XmlSerializer(typeof(NEFlows));
            NEFlows flows;
            using (XmlReader reader = XmlReader.Create(@"D:\TWNE\NETWWebsite2.0\01_Branch\Branch_Project16004\TWNewEgg.ECService\TWNewEgg.ECService\Configurations\NEFlows.xml"))
            {
                flows = (NEFlows)ser.Deserialize(reader);
            }
            NEFlow flow = flows.Flows.Where(x => x.name == "TestFlow1").FirstOrDefault();
            if (flow == null) { }
            foreach (NEFlowStep step in flow.Steps)
            {
                IFlowStep flowStep = (IFlowStep)AutofacConfig.Container.ResolveKeyed("TestFlow1", typeof(IFlowStep));
                steps.Add(flowStep);
            }

            service.AddSteps(steps.ToArray());
            service.Context.SetParameter("baseValue", 10);
            service.Execute();
        }

        public class FlowStep1 : FlowStep
        {
            private string _stepName;
            private Action<IFlowStep> _executeAction;

            public override string StepName
            {
                get
                {
                    return _stepName;
                }
            }

            protected override void DoExecute()
            {
                _executeAction(this);
            }

            public FlowStep1()
            {
                _stepName = "step1";
                _executeAction = new Action<IFlowStep>((sender) =>
                {
                    int baseValue = sender.Context.GetParameter<int>("baseValue");
                    sender.Context.SetParameter("baseValue", baseValue + 10);
                    sender.Complete();
                });
            }
        }

        public class FlowStep2 : FlowStep
        {
            private string _stepName;
            private Action<IFlowStep> _executeAction;

            public override string StepName
            {
                get
                {
                    return _stepName;
                }
            }

            protected override void DoExecute()
            {
                _executeAction(this);
            }

            public FlowStep2()
            {
                _stepName = "step2";
                _executeAction = new Action<IFlowStep>((sender) =>
                {
                    int baseValue = sender.Context.GetParameter<int>("baseValue");
                    int sub = 0;
                    int ans = baseValue / sub;
                    sender.Complete();
                });
            }
        }

        public class StaticFlowService : FlowService
        {

            protected override void ExecuteComplate()
            {

            }

            public StaticFlowService(params IFlowStep[] steps)
            {
                this.AddSteps(steps);
            }
        }

        public ActionResult TestFinance()
        {
            TWNewEgg.Models.DomainModels.Message.ResponseMessage<string> result = new Models.DomainModels.Message.ResponseMessage<string>();

            //FinanceServices.CompanyFinanceDataService cmpServ = new FinanceServices.CompanyFinanceDataService();
            //TWNewEgg.Models.DomainModels.Finance.FinanceDataListFinanceData finaData = cmpServ.Get(DateTime.Now);
            
            //產生會計文件
            TWNewEgg.FinanceServices.Interface.IFinanceDocumentService FinanServ = AutofacConfig.Container.Resolve<TWNewEgg.FinanceServices.Interface.IFinanceDocumentService>();
            //result = FinanServ.Create(Models.DomainModels.Finance.FinanDocTypeEnum.XIRMA);            

            result.IsSuccess = true;
            result.Message = "Done.";

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestSapData()
        {
            TWNewEgg.Models.DomainModels.Message.ResponseMessage<List<Models.DomainModels.Finance.SapBapiAccDocumentDM>> result = new Models.DomainModels.Message.ResponseMessage<List<Models.DomainModels.Finance.SapBapiAccDocumentDM>>();
            
            //取得SAP資料
            TWNewEgg.FinanceServices.Interface.ISapDocumentService SapDocServ = AutofacConfig.Container.Resolve<TWNewEgg.FinanceServices.Interface.ISapDocumentService>();
            
            Models.DomainModels.Finance.DocConditionDM condit = new Models.DomainModels.Finance.DocConditionDM();
            condit.DocType = Models.DomainModels.Finance.FinanDocTypeEnum.ALL;
            condit.StartDate = DateTime.Parse("2016/5/10");
            condit.EndDate = DateTime.Parse("2016/5/10");

            result = SapDocServ.GetData(condit);

            result.IsSuccess = true;
            result.Message = "Done.";

            return Json(result, JsonRequestBehavior.AllowGet);

        }
    }
}
