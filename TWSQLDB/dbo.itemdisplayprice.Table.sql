USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemdisplayprice]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[itemdisplayprice](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[PriceType] [int] NOT NULL,
	[MinNumber] [int] NOT NULL,
	[MaxNumber] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[DisplayPrice] [decimal](12, 4) NOT NULL,
	[DisplayTax] [decimal](12, 4) NOT NULL,
	[DisplayShipping] [decimal](12, 4) NOT NULL,
	[ItemCost] [decimal](12, 4) NOT NULL,
	[ItemCostTW] [decimal](12, 4) NOT NULL,
	[ItemProfitPercent] [decimal](5, 2) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_itemdisplayprice] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
