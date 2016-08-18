USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[reversepurchaseorderitem]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[reversepurchaseorderitem](
	[Code] [varchar](15) NOT NULL,
	[ReversePurchaseorderCode] [varchar](15) NOT NULL,
	[PurchaseOrderItemCode] [varchar](15) NULL,
	[SellerorderCode] [varchar](30) NULL,
	[ItemID] [int] NOT NULL,
	[ItemlistID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[ProductlistID] [int] NOT NULL,
	[Name] [nvarchar](200) NULL,
	[SourceCurrencyID] [int] NULL,
	[SourcePrice] [decimal](10, 5) NOT NULL,
	[LocalCurrencyID] [int] NULL,
	[LocalPrice] [decimal](10, 5) NOT NULL,
	[LocalPriceinst] [decimal](10, 2) NULL,
	[Qty] [int] NOT NULL,
	[LocalPriceCoupon] [decimal](10, 2) NULL,
	[RedmtkOut] [int] NULL,
	[RedmBLN] [int] NULL,
	[Redmfdbck] [int] NULL,
	[Status] [int] NULL,
	[StatusNote] [nvarchar](50) NULL,
	[Date] [datetime] NULL,
	[Attribs] [nvarchar](50) NULL,
	[Note] [nvarchar](50) NULL,
	[WftkOut] [int] NULL,
	[WfBLN] [int] NULL,
	[ADJPrice] [int] NULL,
	[ACTID] [nvarchar](50) NULL,
	[ActtkOut] [int] NULL,
	[ProdcutCostID] [int] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_reversepurchaseorderitem_purchaseorderitem_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_reversepurchaseorderitem_purchaseorderitem_updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[InvoiceNO] [nvarchar](50) NULL,
	[BoxNO] [nvarchar](50) NULL,
	[BoxWeight] [decimal](10, 2) NULL,
	[BoxCurrency] [int] NULL,
	[DimLenght] [decimal](10, 2) NULL,
	[DimWidth] [decimal](10, 2) NULL,
	[DimHieght] [decimal](10, 2) NULL,
	[DimCurrency] [int] NULL,
	[ImportCost] [decimal](10, 2) NULL,
	[CCCCode] [varchar](15) NULL,
	[DutyRate] [decimal](10, 5) NULL,
	[ProductTax] [decimal](10, 5) NULL,
	[FullTaxValue] [decimal](10, 2) NULL,
	[ShippingFee] [decimal](10, 2) NULL,
	[TaxandDuty] [decimal](10, 2) NULL,
	[WarehouseID] [int] NULL,
	[Customs_Charge] [decimal](10, 2) NULL,
	[TradeServiceCharges] [decimal](10, 2) NULL,
	[SellerID] [int] NULL,
	[Coupons] [nvarchar](20) NULL,
 CONSTRAINT [PK_reversepurchaseorderitem_id] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
