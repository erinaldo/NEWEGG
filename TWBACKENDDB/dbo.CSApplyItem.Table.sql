USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[CSApplyItem]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CSApplyItem](
	[Code] [char](14) NOT NULL,
	[CSApplyCode] [char](13) NOT NULL,
	[ProductID] [int] NOT NULL,
	[SellerID] [int] NOT NULL,
	[ProductName] [nvarchar](max) NOT NULL,
	[UPC] [varchar](100) NOT NULL,
	[Qty] [int] NOT NULL,
	[Manufacturers] [nvarchar](max) NOT NULL,
	[Model] [nvarchar](100) NOT NULL,
	[EnumMarketplace] [nvarchar](100) NOT NULL,
	[EnumCSUint] [varchar](100) NOT NULL,
	[Length] [decimal](5, 2) NOT NULL,
	[Width] [decimal](5, 2) NOT NULL,
	[Height] [decimal](5, 2) NOT NULL,
	[Weight] [decimal](5, 2) NOT NULL,
	[FullImageName] [varchar](max) NOT NULL,
	[TrackingNumber] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CSApplyItem] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
