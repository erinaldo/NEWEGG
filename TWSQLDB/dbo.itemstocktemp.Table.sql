USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemstocktemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[itemstocktemp](
	[producttempID] [int] NOT NULL,
	[ProductID] [int] NULL,
	[Qty] [int] NOT NULL,
	[QtyReg] [int] NOT NULL,
	[SafeQty] [int] NOT NULL,
	[Fdbcklmt] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_itemstocktemp_createdate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_itemstocktemp_UpdateDate]  DEFAULT (getdate()),
	[Updated] [int] NULL,
 CONSTRAINT [PK_itemstocktemp_id] PRIMARY KEY CLUSTERED 
(
	[producttempID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
