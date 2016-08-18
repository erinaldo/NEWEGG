USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[DeviceAdContent]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceAdContent](
	[ID] [int] IDENTITY(5000,1) NOT NULL,
	[DeviceAdSetID] [int] NOT NULL DEFAULT ((0)),
	[Name] [nvarchar](50) NULL DEFAULT (''),
	[Name2] [nvarchar](50) NULL DEFAULT (''),
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Flag] [nvarchar](50) NOT NULL DEFAULT (''),
	[Showorder] [int] NOT NULL,
	[ShowAll] [nvarchar](50) NOT NULL DEFAULT (''),
	[Clickpath] [nvarchar](500) NULL DEFAULT (''),
	[ImageUrl] [nvarchar](100) NOT NULL DEFAULT (''),
	[CategoryID] [int] NOT NULL DEFAULT ((0)),
	[CreateUser] [nvarchar](50) NOT NULL DEFAULT (''),
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_DeviceAdContent] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'屬於哪一個行動設備的廣告設定, DeviceAdSet.ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DeviceAdContent', @level2type=N'COLUMN',@level2name=N'DeviceAdSetID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名稱2, 生活提案時叫:小標題' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DeviceAdContent', @level2type=N'COLUMN',@level2name=N'Name2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'del=已刪除了, 若CategoryID有值這裡代表是屬於這個CategoryID的index' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DeviceAdContent', @level2type=N'COLUMN',@level2name=N'Flag'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'顯示:show|1, 不顯示:hide|0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DeviceAdContent', @level2type=N'COLUMN',@level2name=N'ShowAll'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'來自SubCategory_NormalStore與Category的ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DeviceAdContent', @level2type=N'COLUMN',@level2name=N'CategoryID'
GO
