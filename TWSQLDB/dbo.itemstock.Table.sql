USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemstock]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[itemstock](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[Qty] [int] NOT NULL,
	[QtyReg] [int] NOT NULL CONSTRAINT [DF_itemstock_QtyReg]  DEFAULT ((0)),
	[SafeQty] [int] NOT NULL CONSTRAINT [DF_itemstock_SafeQty]  DEFAULT ((0)),
	[Fdbcklmt] [int] NOT NULL CONSTRAINT [DF_itemstock_Fdbcklmt]  DEFAULT ((0)),
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_itemstock_itemstock_createdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_itemstock_itemstock_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL CONSTRAINT [DF_Table_1_itemstock_updatedate]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_itemstock_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[itemstock]  WITH NOCHECK ADD  CONSTRAINT [CK_itemstock_qty_reg] CHECK  ((([Qty]-[QtyReg])>=(0)))
GO
ALTER TABLE [dbo].[itemstock] CHECK CONSTRAINT [CK_itemstock_qty_reg]
GO
