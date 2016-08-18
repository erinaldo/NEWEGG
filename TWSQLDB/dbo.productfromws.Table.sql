USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[productfromws]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[productfromws](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemNumber] [varchar](30) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[NameTW] [nvarchar](200) NULL,
	[Description] [nvarchar](500) NULL,
	[DescriptionTW] [nvarchar](500) NULL,
	[SPEC] [nvarchar](4000) NULL,
	[ManufactureID] [int] NOT NULL,
	[Model] [varchar](30) NULL,
	[BarCode] [varchar](50) NULL,
	[SellerID] [int] NULL,
	[DelvType] [int] NULL,
	[PicStart] [int] NULL,
	[PicEnd] [int] NULL,
	[Cost] [decimal](10, 2) NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_productfromws_status]  DEFAULT ((0)),
	[NValue] [varchar](128) NULL,
	[InvoiceType] [int] NULL,
	[SaleType] [int] NULL,
	[CCC] [varchar](15) NULL,
	[HS] [varchar](15) NULL,
	[Rating] [int] NULL,
	[TotalReviews] [int] NULL,
	[ImagePath] [varchar](256) NULL,
	[Length] [decimal](10, 2) NULL,
	[Width] [decimal](10, 2) NULL,
	[Height] [decimal](10, 2) NULL,
	[Weight] [decimal](10, 2) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[Qty] [int] NULL,
	[IsFirstFromAsia] [char](1) NULL,
	[SellerName] [nvarchar](50) NULL,
	[Brand] [nvarchar](500) NULL,
	[CategoryTW] [int] NULL,
	[ShippingCharge] [decimal](10, 2) NULL,
	[Available] [char](5) NULL,
 CONSTRAINT [PK_productfromws_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
