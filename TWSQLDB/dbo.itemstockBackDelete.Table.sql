USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemstockBackDelete]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[itemstockBackDelete](
	[ID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Qty] [int] NOT NULL,
	[QtyReg] [int] NOT NULL,
	[SafeQty] [int] NOT NULL,
	[Fdbcklmt] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL
) ON [PRIMARY]

GO
