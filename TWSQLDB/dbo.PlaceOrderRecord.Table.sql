USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PlaceOrderRecord]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PlaceOrderRecord](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderCode] [char](15) NOT NULL,
	[PurchaseOrderItemCode] [char](15) NOT NULL,
	[SellerOrderCode] [nvarchar](30) NULL,
	[TradeMode] [int] NOT NULL,
	[SellerProductID] [nvarchar](150) NOT NULL,
	[Qty] [int] NOT NULL,
	[OriginalUnitPrice] [decimal](10, 5) NULL,
	[CurrentUnitPrice] [decimal](10, 5) NULL,
	[OriginalShippingCharge] [decimal](10, 5) NULL,
	[CurrentShippingCharge] [decimal](10, 5) NULL,
	[OriginalAmount] [decimal](10, 5) NULL,
	[CurrentAmount] [decimal](10, 5) NULL,
	[ShipViaCode] [varchar](15) NOT NULL,
	[Warehouse] [varchar](15) NOT NULL,
	[IsSuccess] [bit] NULL,
	[LastSendDate] [datetime] NULL,
	[Message] [nvarchar](2000) NULL,
	[Exception] [nvarchar](2000) NULL,
	[CreateDate] [datetime] NULL,
 CONSTRAINT [PK_PlaceOrderRecord] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
