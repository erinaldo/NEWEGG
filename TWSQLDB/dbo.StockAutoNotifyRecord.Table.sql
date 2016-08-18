USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[StockAutoNotifyRecord]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StockAutoNotifyRecord](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](256) NOT NULL,
	[ProductID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
