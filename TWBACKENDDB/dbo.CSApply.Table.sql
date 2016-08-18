USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[CSApply]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CSApply](
	[Code] [char](13) NOT NULL,
	[ApplicantCompanyID] [int] NOT NULL,
	[ApplicantCompanyName] [nvarchar](50) NOT NULL,
	[Contact] [nvarchar](10) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Country] [nvarchar](10) NOT NULL,
	[State] [nvarchar](10) NOT NULL,
	[City] [nvarchar](10) NOT NULL,
	[Address] [nvarchar](500) NOT NULL,
	[ZipCode] [varchar](10) NOT NULL,
	[EnumCSSellerVendor] [varchar](10) NOT NULL,
	[ShipLocationName] [nvarchar](50) NOT NULL,
	[ShipLocationApplicantCompanyID] [int] NOT NULL,
	[ShipLocationContact] [nvarchar](50) NOT NULL,
	[ShipLocationPhone] [nvarchar](20) NOT NULL,
	[ShipLocationCountry] [nvarchar](10) NOT NULL,
	[ShipLocationState] [nvarchar](10) NOT NULL,
	[ShipLocationCity] [nvarchar](10) NOT NULL,
	[ShipLocationAddress] [nvarchar](50) NOT NULL,
	[ShipLocationZipCode] [varchar](10) NOT NULL,
	[DeliveryID] [varchar](50) NOT NULL,
	[DeliveryCompanyID] [varchar](50) NOT NULL,
	[DeliveryBoxQuantity] [int] NOT NULL,
	[DeliveryCompanyOther] [nvarchar](50) NOT NULL,
	[ConsignmentStockLocationID] [nvarchar](50) NOT NULL,
	[EnumCSSellTWNewEgg] [nchar](10) NOT NULL,
	[InventoryStatus] [int] NULL,
	[InventoryStatusDate] [datetime] NULL,
	[InventoryStatusUser] [nvarchar](50) NULL,
	[CheckInDate] [datetime] NULL,
 CONSTRAINT [PK_CSApply] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
