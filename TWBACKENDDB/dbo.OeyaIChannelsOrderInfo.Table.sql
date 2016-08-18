USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[OeyaIChannelsOrderInfo]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OeyaIChannelsOrderInfo](
	[SalesOrderItemCode] [varchar](20) NOT NULL,
	[ProductID] [varchar](32) NOT NULL,
	[ProductName] [nvarchar](1500) NOT NULL,
	[ProductPrice] [decimal](10, 2) NOT NULL,
	[ProductQty] [int] NOT NULL,
	[TotalPrice] [decimal](10, 2) NOT NULL,
	[OrderStatus] [int] NOT NULL,
	[OrderStatusDate] [datetime] NOT NULL,
	[OrderCreateDate] [datetime] NOT NULL,
	[OeyaInfo] [nvarchar](500) NOT NULL,
	[BackCode] [varchar](500) NOT NULL,
	[InvalidReason] [nvarchar](32) NOT NULL,
	[Other] [nvarchar](32) NOT NULL,
	[SendStatus] [varchar](1500) NOT NULL,
	[SendDate] [datetime] NULL,
	[OeyaLastQueryDate] [datetime] NULL,
 CONSTRAINT [PK_ichannels] PRIMARY KEY CLUSTERED 
(
	[SalesOrderItemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
