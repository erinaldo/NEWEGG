USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[invoiceprizeitem]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[invoiceprizeitem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[InvoicePrizeFooterID] [int] NOT NULL,
	[TCompanyBan] [varchar](20) NOT NULL,
	[InvoiceAxle] [varchar](10) NOT NULL,
	[InvoiceNumber] [varchar](20) NOT NULL,
	[BuyerName] [nvarchar](50) NOT NULL,
	[BuyerBAN] [varchar](20) NOT NULL,
	[BuyerAddress] [nvarchar](200) NOT NULL,
	[BuyerInvoiceDate] [varchar](20) NOT NULL,
	[BuyerInvoiceTime] [varchar](20) NULL,
	[TotalAmount] [decimal](15, 3) NOT NULL,
	[CarrierType] [varchar](10) NULL,
	[CarrierName] [nvarchar](120) NULL,
	[CarrierIdClear] [nvarchar](128) NULL,
	[CarrierIdHide] [nvarchar](128) NULL,
	[RandomNumber] [varchar](10) NOT NULL,
	[PrizeType] [varchar](10) NOT NULL,
	[PrizeAmt] [decimal](15, 3) NOT NULL,
	[BBAN] [varchar](20) NOT NULL,
	[DepositMK] [varchar](10) NULL,
	[DataType] [varchar](10) NOT NULL,
	[ExceptionCode] [varchar](10) NOT NULL,
	[PrintFormat] [varchar](10) NOT NULL,
	[UniqueCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_invoiceprizeitem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
