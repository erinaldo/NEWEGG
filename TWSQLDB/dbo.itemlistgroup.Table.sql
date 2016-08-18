USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemlistgroup]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[itemlistgroup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TempID] [int] NULL,
	[ItemID] [int] NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_itemid]  DEFAULT ((0)),
	[Name] [nvarchar](50) NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_name]  DEFAULT (''),
	[Type] [int] NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_type]  DEFAULT ((0)),
	[SelectedMax] [int] NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_selectednum]  DEFAULT ((0)),
	[SelectedMin] [int] NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_selectedmin]  DEFAULT ((0)),
	[ItemlistgroupRule] [nvarchar](4000) NULL,
	[ItemlistgroupOrder] [int] NOT NULL CONSTRAINT [DF_itemlistgroup_itemlistgroup_order]  DEFAULT ((0)),
	[Note] [nvarchar](50) NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_note]  DEFAULT (''),
	[CreateUser] [nvarchar](50) NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_createuser]  DEFAULT (''),
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_createdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_updateuser]  DEFAULT (''),
	[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_itemgroup_itemgroup_updatedate]  DEFAULT ('1900/01/01'),
 CONSTRAINT [PK_itemlistgroup_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[itemlistgroup]  WITH NOCHECK ADD  CONSTRAINT [CK_itemlistgroup_selectedmax_selectedmin] CHECK  (([SelectedMax]>=[SelectedMin]))
GO
ALTER TABLE [dbo].[itemlistgroup] CHECK CONSTRAINT [CK_itemlistgroup_selectedmax_selectedmin]
GO
