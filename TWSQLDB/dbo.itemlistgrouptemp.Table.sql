USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemlistgrouptemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[itemlistgrouptemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Type] [int] NOT NULL,
	[SelectedMax] [int] NOT NULL,
	[SelectedMin] [int] NOT NULL,
	[ItemlistgrouptempRule] [nvarchar](4000) NULL,
	[ItemlistgrouptempOrder] [int] NOT NULL,
	[Note] [nvarchar](50) NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_itemlistgrouptemp_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_itemid]  DEFAULT ((0)) FOR [ItemID]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_name]  DEFAULT ('') FOR [Name]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_selectednum]  DEFAULT ((0)) FOR [SelectedMax]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_selectedmin]  DEFAULT ((0)) FOR [SelectedMin]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemlistgrouptemp_itemlistgrouptemp_order]  DEFAULT ((0)) FOR [ItemlistgrouptempOrder]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_note]  DEFAULT ('') FOR [Note]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_createuser]  DEFAULT ('') FOR [CreateUser]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_createdate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_updated]  DEFAULT ((0)) FOR [Updated]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_updateuser]  DEFAULT ('') FOR [UpdateUser]
GO
ALTER TABLE [dbo].[itemlistgrouptemp] ADD  CONSTRAINT [DF_itemgrouptemp_itemgrouptemp_updatedate]  DEFAULT ('1900/01/01') FOR [UpdateDate]
GO
ALTER TABLE [dbo].[itemlistgrouptemp]  WITH NOCHECK ADD  CONSTRAINT [CK_itemlistgrouptemp_selectedmax_selectedmin] CHECK  (([SelectedMax]>=[SelectedMin]))
GO
ALTER TABLE [dbo].[itemlistgrouptemp] CHECK CONSTRAINT [CK_itemlistgrouptemp_selectedmax_selectedmin]
GO
