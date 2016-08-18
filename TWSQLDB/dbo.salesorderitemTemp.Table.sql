USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[salesorderitemTemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[salesorderitemTemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](15) NOT NULL,
	[SalesorderCode] [varchar](15) NOT NULL,
	[ItemID] [int] NOT NULL,
	[ItemlistID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[ProductlistID] [int] NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[Priceinst] [decimal](10, 2) NULL,
	[Qty] [int] NOT NULL,
	[Pricecoupon] [decimal](10, 2) NULL,
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
	[AdjPrice] [int] NULL,
	[ActID] [nvarchar](50) NULL,
	[ActtkOut] [int] NULL,
	[ProdcutCostID] [int] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[ShippingExpense] [decimal](10, 2) NULL CONSTRAINT [DF_salesorderitemTemp_salesorderitemTemp_shippingexpense]  DEFAULT ((0.00)),
	[ServiceExpense] [decimal](10, 2) NULL,
	[Tax] [decimal](10, 2) NULL,
	[WarehouseID] [int] NULL,
	[Coupons] [nvarchar](20) NULL,
	[ItemPriceSum] [decimal](10, 2) NULL,
	[DisplayPrice] [decimal](10, 2) NULL,
	[DiscountPrice] [decimal](10, 2) NULL,
	[InstallmentFee] [decimal](10, 2) NOT NULL CONSTRAINT [DF__salesorde__Insta__481F9E1A]  DEFAULT ((0)),
	[IsNew] [char](1) NULL,
	[ApportionedAmount] [decimal](10, 2) NOT NULL CONSTRAINT [DF__salesorde__Appor__4913C253]  DEFAULT ((0)),
	[SupplyShippingCharge] [decimal](10, 2) NULL,
	[itemCategory] [int] NULL,
 CONSTRAINT [PK_salesorderitemTemp] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
