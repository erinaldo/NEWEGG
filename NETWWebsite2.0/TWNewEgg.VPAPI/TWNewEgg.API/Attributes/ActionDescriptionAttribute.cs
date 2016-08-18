using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.Attributes
{
    public class ActionDescriptionAttribute : System.Attribute
    {
        private string _description;
        public string Description
        {
            get { return this._description; }
        }

        public ActionDescriptionAttribute(string description)
        {
            this._description = description;
        }
    }

    public class FunctionCategoryNameAttribute : System.Attribute
    {
        private string _functionCategoryName;
        public string FunctionCategoryName
        {
            get { return this._functionCategoryName; }
        }

        public FunctionCategoryNameAttribute(string functionCategoryName)
        {
            this._functionCategoryName = functionCategoryName;
        }
    }

    public class FunctionNameAttribute : System.Attribute
    {
        private string _functionName;
        public string FunctionName
        {
            get { return this._functionName; }
        }

        public FunctionNameAttribute(string functionName)
        {
            this._functionName = functionName;
        }
    }

    public class ActiveKeyAttribute : System.Attribute
    {
        private string _activeKey;
        public string ActiveKey
        {
            get { return this._activeKey; }
        }

        public ActiveKeyAttribute(string activeKey)
        {
            this._activeKey = activeKey;
        }
    }

    public class FunctionNameAttributeValues
    {
        public const string AccountSettings = "Account Settings";
        public const string AccountRegister = "Account Register";
        public const string ItemCreation = "Item Creation";
        public const string ItemList = "Item List";
        public const string BatchUploadInventory = "Batch Upload Inventory";
        public const string BatchItemCreationUpdate = "Batch Item Creation/Update";
        public const string OrderList = "Order List";
        public const string CustomerRatingReport = "Customer Rating Report";
        public const string PaymentReports = "Payment Reports";
        public const string QueryLog = "Query Log";
        public const string LogSettings = "Log Settings";
        public const string PromotionScheduler = "Promotion Scheduler";
        public const string ShippingSettings = "Shipping Settings";
        public const string ShipmentList_SBNManagementOnly = "Shipment List  (SBN Management Only)";
        public const string InventoryAlert = "Inventory Alert";
        public const string ReturnList = "Return List";
        public const string CreateMultiChannelOrder = "Create Multi-Channel Order";
        public const string RefundReports = "Refund Reports";
        public const string ReplacementReports = "Replacement Reports";
        public const string DailyInventoryHistory = "Daily Inventory History";
        public const string ReceivedHistory = "Received History";
        public const string OrderListInternal = "Order List Internal";
        public const string UserList = "User List";
        public const string RoleList = "Role List";
        public const string UserGroupList = "User Group List";
        public const string SellerInvitation = "Seller Invitation";
        public const string GeneralPromotion = "General Promotion";
        public const string GeneralPromotionApproveAndForceClose = "General Promotion Approve&Force Close";
        public const string AutoAddtoCart = "Auto Add to Cart";
        public const string AutoAddtoCartApproveAndForceClose = "Auto Add to Cart Approve&Force Close";
        public const string Combo = "Combo";
        public const string ComboApproveAndForceClose = "Combo Approve&Force Close";
        public const string PromotionCode = "Promotion Code";
        public const string PromotionCodeApproveAndForceClose = "Promotion Code Approve&Force Close";
        public const string WordFilter = "Word Filter";
        public const string DataFeedSettings = "Data Feed Settings";
        public const string ManufactureMappingMaintain = "Manufacture Mapping Maintain";
        public const string SellerRelationshipManagement = "Seller Relationship Management";
        public const string CustomerMessage = "Customer Message";
        public const string FeedConverter = "Feed Converter";
        public const string SellerSubcategoryControl = "Seller Subcategory Control";
        public const string ItemCorrectionTools = "Item Correction Tools";
        public const string StoreFront = "Store Front";
        public const string SellerPerformance = "Seller Performance";
        public const string Manufacturer = "Manufacturer";
        public const string ManufacturerRequestApprove = "Manufacturer Request Approve";
        public const string PrintInvoice = "Print Invoice";
        public const string BatchPropertyUpdate = "Batch Property Update";
        public const string SellerAnnouncement = "Seller Announcement";
        public const string ItemImageManagement = "Item Image Management";
        public const string PrintOrderPackingList = "Print Order Packing List";
        public const string HelpGuideManagement = "Help Guide Management";
        public const string ItemQASettings = "Item QA Settings";
        public const string ItemQAList = "Item QA List";
        public const string UPloadFileDemo = "UPload File Demo";
        public const string BatchItemOwnershipChange = "Batch Item Ownership Change";
        public const string PropertyValue = "Property Value";
        public const string ApiKeyManagement = "Api Key Management";
        public const string NeweggSBNVoidFunction = "Newegg SBN Void Function";
        public const string NavigationNodeMaintain = "Navigation Node Maintain";
        public const string NavigationTreeMaintain = "Navigation Tree Maintain";
        public const string NavigationGroupMaintain = "Navigation Group Maintain";
        public const string BannerMaintain = "Banner Maintain";
        public const string HomepagePromotionMaintain = "Homepage Promotion Maintain";
        public const string StoreItemMaintain = "Store Item Maintain";
        public const string ComboDealsStoreMaintain = "Combo Deals Store Maintain";
        public const string RefurbishedStoreMaintain = "Refurbished Store Maintain";
        public const string DealDirectoryStoreMaintain = "Deal Directory Store Maintain";
        public const string Summary = "Summary";
        public const string SalesRevenue = "Sales Revenue";
        public const string OrderAutoVoidTimeFrame = "Order Auto Void Time Frame";
        public const string NeweggFlash = "Newegg Flash";
        public const string SellerSelectionDemo = "SellerSelectionDemo";
        public const string VoidOrders = "Void Orders";
        public const string Returns = "Returns";
        public const string ItemReport = "Item Report";
        public const string OrderLocation = "Order Location";
        public const string BatchContentUpdate = "Batch Content Update";
        public const string SellerSubcategoryControl_IndividualSellers = "Seller Subcategory Control(Individual Sellers)";
        public const string Deals = "Deals";
        public const string DealsPageSubmission = "Deals Page Submission";
    }

    public class FunctionCategoryNameAttributeValues
    {
        public const string ManageAccount = "ManageAccount";
        public const string ManageItems = "ManageItems";
        public const string ManageOrder = "ManageOrder";
        public const string OtherReports = "OtherReports";
        public const string SystemHealth = "SystemHealth";
        public const string ManagePromotion = "ManagePromotion";
        public const string ManageMessage = "ManageMessage";
        public const string ManageStore = "ManageStore";
        public const string ContentManage = "ContentManage";
        public const string SellerManage = "SellerManage";
        public const string UserManage = "UserManage";
        public const string DemoManage = "DemoManage";
        public const string WebStoreManagement = "WebStoreManagement";
        public const string BusinessReport = "BusinessReport";

    }

    public class ActiveKeyAttributeValues
    {
        public const string None = "None";
        public const string View = "View";
        public const string Edit = "Edit";
        public const string Menu = "Menu";
    }

    /*
    public class AttributeValues
    {

        public static Dictionary<int, string> FunctionNames = new Dictionary<int, string>()
        {
            #region FunctionName
            {1,"Account Settings"},
            {2,"Account Register"},
            {4,"Item Creation"},
            {5,"Item List"},
            {6,"Batch Upload Inventory"},
            {7,"Batch Item Creation/Update"},
            {8,"Order List"},
            {9,"Customer Rating Report"},
            {10,"Payment Reports"},
            {11,"Query Log"},
            {12,"Log Settings"},
            {13,"Promotion Scheduler"},
            {14,"Shipping Settings"},
            {15,"Shipment List  (SBN Management Only)"},
            {16,"Inventory Alert"},
            {17,"Return List"},
            {18,"Create Multi-Channel Order"},
            {19,"Refund Reports"},
            {20,"Replacement Reports"},
            {21,"Daily Inventory History"},
            {22,"Received History"},
            {23,"Order List"},
            {24,"User List"},
            {25,"Role List"},
            {26,"User Group List"},
            {27,"Seller Invitation"},
            {28,"General Promotion"},
            {29,"General Promotion Approve&Force Close"},
            {30,"Auto Add to Cart"},
            {31,"Auto Add to Cart Approve&Force Close"},
            {32,"Combo"},
            {33,"Combo Approve&Force Close"},
            {34,"Promotion Code"},
            {35,"Promotion Code Approve&Force Close"},
            {36,"Word Filter"},
            {38,"Data Feed Settings"},
            {39,"Manufacture Mapping Maintain"},
            {40,"Seller Relationship Management"},
            {42,"Customer Message"},
            {43,"Feed Converter"},
            {44,"Seller Subcategory Control"},
            {45,"Item Correction Tools"},
            {46,"Store Front"},
            {47,"Seller Performance"},
            {48,"Manufacturer"},
            {49,"Manufacturer Request Approve"},
            {50,"Print Invoice"},
            {51,"Batch Property Update"},
            {52,"Seller Announcement"},
            {53,"Item Image Management"},
            {54,"Print Order Packing List"},
            {55,"Help Guide Management"},
            {56,"Item QA Settings"},
            {57,"Item QA List"},
            {58,"UPload File Demo"},
            {59,"Batch Item Ownership Change"},
            {60,"Property Value"},
            {61,"Api Key Management"},
            {74,"Newegg SBN Void Function"},
            {76,"Navigation Node Maintain"},
            {77,"Navigation Tree Maintain"},
            {78,"Navigation Group Maintain"},
            {79,"Banner Maintain"},
            {80,"Homepage Promotion Maintain"},
            {81,"Store Item Maintain"},
            {82,"Combo Deals Store Maintain"},
            {83,"Refurbished Store Maintain"},
            {84,"Deal Directory Store Maintain"},
            {85,"Summary"},
            {86,"Sales Revenue"},
            {87,"Order Auto Void Time Frame"},
            {88,"Newegg Flash"},
            {89,"SellerSelectionDemo"},
            {90,"Void Orders"},
            {91,"Returns"},
            {92,"Item Report"},
            {93,"Order Location"},
            {94,"Batch Content Update"},
            {95,"Seller Subcategory Control(Individual Sellers)"},
            {96,"Deals"},
            {97,"Deals Page Submission"}
            #endregion
        };

        public static Dictionary<int, string> FunctionCategoryNames = new Dictionary<int, string>()
        {
            #region FunctionCategoryName
            {1,"ManageAccount"},
            {2,"ManageItems"},
            {3,"ManageOrder"},
            {4,"OtherReports"},
            {5,"SystemHealth"},
            {6,"ManagePromotion"},
            {7,"ManageMessage"},
            {8,"ManageStore"},
            {9,"ContentManage"},
            {10,"SellerManage"},
            {11,"UserManage"},
            {12,"DemoManage"},
            {13,"WebStoreManagement"},
            {14,"BusinessReport"}
            #endregion
        };

        public static Dictionary<int, string> ActiveKeys = new Dictionary<int, string>()
        {
            #region ActiveKey
            {1,"None"},
            {2,"View"},
            {3,"Edit"}
            #endregion
        };
    }
    */
}