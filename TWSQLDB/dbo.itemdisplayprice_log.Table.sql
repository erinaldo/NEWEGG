USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemdisplayprice_log]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[itemdisplayprice_log](
	[UserAction] [varchar](15) NULL,
	[EditDate] [datetime] NULL,
	[ID] [int] NULL,
	[ItemID] [int] NULL,
	[PriceType] [int] NULL,
	[MinNumber] [int] NULL,
	[MaxNumber] [int] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[DisplayPrice] [decimal](12, 4) NULL,
	[DisplayTax] [decimal](12, 4) NULL,
	[DisplayShipping] [decimal](12, 4) NULL,
	[ItemCost] [decimal](12, 4) NULL,
	[ItemCostTW] [decimal](12, 4) NULL,
	[ItemProfitPercent] [decimal](5, 2) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
