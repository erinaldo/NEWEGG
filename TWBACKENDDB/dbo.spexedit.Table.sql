USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[spexedit]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[spexedit](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SO] [varchar](30) NULL,
	[SellerProductID] [nchar](30) NULL,
	[PurchaseOrderSalesOrdercode] [nchar](30) NULL,
	[SalesDEP] [char](5) NOT NULL,
	[AccountNumber] [varchar](11) NOT NULL,
	[SimpleName] [varchar](20) NOT NULL,
	[Sales] [varchar](16) NULL,
	[CompanyNameSent] [varchar](200) NOT NULL,
	[ContactPersonSent] [varchar](200) NULL,
	[Address1Sent] [varchar](150) NOT NULL,
	[Address2Sent] [varchar](150) NULL,
	[State] [char](2) NOT NULL,
	[SentZipCode] [char](10) NOT NULL,
	[SentCityName] [varchar](100) NOT NULL,
	[SentPhoneNO] [varchar](30) NULL,
	[ReceiveCompanyName] [nvarchar](150) NOT NULL,
	[ReceiveContactPerson] [varchar](50) NULL,
	[ReceiveAddress1] [nvarchar](150) NOT NULL,
	[ReceiveAddress2] [varchar](150) NULL,
	[ConsigneeState] [char](10) NOT NULL,
	[ReceiveZipCode] [char](5) NULL,
	[CountryName] [varchar](100) NOT NULL,
	[ReceiveCityName] [nvarchar](100) NULL,
	[ReceivePhoneNO] [varchar](30) NOT NULL,
	[Pieces] [int] NULL,
	[PackageType] [varchar](20) NULL,
	[DimWeight] [decimal](10, 2) NULL,
	[GrossWeight] [decimal](10, 2) NULL,
	[InvoiceValue] [int] NULL,
	[Description] [nvarchar](200) NULL,
	[FuelSurcharge] [decimal](10, 2) NULL,
	[Flag] [int] NOT NULL,
	[FileName] [varchar](50) NULL,
	[DELIVType] [int] NULL,
	[CreateDate] [datetime] NULL,
	[NO] [int] NOT NULL,
	[Long] [decimal](10, 2) NULL,
	[Width] [decimal](10, 2) NULL,
	[High] [decimal](10, 2) NULL,
	[ForwardNo] [nvarchar](50) NULL,
	[Qty] [int] NULL,
 CONSTRAINT [PK_spexedit_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
