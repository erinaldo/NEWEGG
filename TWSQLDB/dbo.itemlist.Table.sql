USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemlist]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[itemlist](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemlisttempID] [int] NULL,
	[ItemID] [int] NOT NULL,
	[ItemlistGroupID] [int] NOT NULL,
	[ItemlistProductID] [int] NOT NULL,
	[ItemlistID] [int] NOT NULL,
	[ProdcutcostID] [int] NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Sdesc] [nvarchar](100) NOT NULL,
	[Cost] [decimal](10, 2) NOT NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[ServicePrice] [decimal](10, 2) NOT NULL,
	[Pricehpinst1] [decimal](10, 2) NOT NULL,
	[Pricehpinst2] [decimal](10, 2) NOT NULL,
	[Priceship] [decimal](10, 2) NOT NULL,
	[Qty] [int] NOT NULL,
	[SafeQty] [int] NOT NULL,
	[QtyReg] [int] NOT NULL,
	[QtyLimit] [int] NOT NULL,
	[Photo] [nvarchar](50) NOT NULL,
	[ItemlistOrder] [int] NOT NULL,
	[Note] [nvarchar](50) NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_itemlist_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_item_itemlist_itemid]  DEFAULT ((0)) FOR [ItemID]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_listgroupid]  DEFAULT ((0)) FOR [ItemlistGroupID]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_prdctid]  DEFAULT ((0)) FOR [ItemlistProductID]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_itemlistid]  DEFAULT ((0)) FOR [ItemlistID]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_name]  DEFAULT ('') FOR [Name]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_sdesc]  DEFAULT ('') FOR [Sdesc]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_cost]  DEFAULT ((0)) FOR [Cost]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_price]  DEFAULT ((0)) FOR [Price]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_serviceprice]  DEFAULT ((0)) FOR [ServicePrice]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_pricehpinst1]  DEFAULT ((0)) FOR [Pricehpinst1]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_pricehpinst2]  DEFAULT ((0)) FOR [Pricehpinst2]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_priceship]  DEFAULT ((0)) FOR [Priceship]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_qty]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_safeqty]  DEFAULT ((0)) FOR [SafeQty]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_qtyreg]  DEFAULT ((0)) FOR [QtyReg]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_qtylimit]  DEFAULT ((0)) FOR [QtyLimit]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_photo]  DEFAULT ('') FOR [Photo]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_order]  DEFAULT ((0)) FOR [ItemlistOrder]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_note]  DEFAULT ('') FOR [Note]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_createdate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_updateduser]  DEFAULT ('') FOR [CreateUser]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_updated]  DEFAULT ((0)) FOR [Updated]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_itemlist_itemlist_updatedate]  DEFAULT ('1900/01/01') FOR [UpdateDate]
GO
ALTER TABLE [dbo].[itemlist] ADD  CONSTRAINT [DF_gditem_gditem_updateuser]  DEFAULT ('') FOR [UpdateUser]
GO
ALTER TABLE [dbo].[itemlist]  WITH NOCHECK ADD  CONSTRAINT [CK_itemlist_Reg] CHECK  ((([Qty]-[QtyReg])>=(0) AND [Qty]>(0) OR [Qty]=(0)))
GO
ALTER TABLE [dbo].[itemlist] CHECK CONSTRAINT [CK_itemlist_Reg]
GO
