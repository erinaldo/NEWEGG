USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[warehouse]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[warehouse](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[WarehouseID] [nvarchar](10) NOT NULL,
	[WarehouseName] [nvarchar](50) NOT NULL,
	[WMSWarehouseLocationName] [nvarchar](50) NOT NULL,
	[CreateUser] [nvarchar](10) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[Address] [nvarchar](150) NULL,
	[Zipcode] [varchar](15) NULL,
	[City] [nvarchar](20) NULL,
	[Dist] [nvarchar](10) NULL,
	[PhoneRegion] [varchar](10) NULL,
	[Phone] [varchar](30) NULL,
	[PhoneExt] [varchar](10) NULL,
 CONSTRAINT [PK_warehouse] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
