USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[product_log]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product_log](
	[UserAction] [varchar](15) NULL,
	[EditDate] [datetime] NULL,
	[ID] [int] NULL,
	[SourceTable] [nvarchar](150) NULL,
	[SellerProductID] [varchar](150) NULL,
	[ManufactureID] [int] NULL,
	[SellerID] [int] NULL,
	[DelvType] [int] NULL,
	[PicStart] [int] NULL,
	[PicEnd] [int] NULL,
	[Cost] [decimal](10, 2) NULL,
	[Length] [decimal](10, 2) NULL,
	[Width] [decimal](10, 2) NULL,
	[Height] [decimal](10, 2) NULL,
	[Weight] [decimal](12, 4) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[Warranty] [int] NULL,
	[UPC] [varchar](15) NULL,
	[Note] [nvarchar](500) NULL,
	[IsMarket] [char](1) NULL,
	[Is18] [char](1) NULL,
	[IsShipDanger] [char](1) NULL,
	[IsChokingDanger] [char](1) NULL,
	[MenufacturePartNum] [varchar](15) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
