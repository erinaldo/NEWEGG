USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[landingpage]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[landingpage](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[HtmlContext] [nvarchar](max) NULL,
	[MainItem] [nvarchar](100) NULL,
	[Title] [nvarchar](300) NULL,
	[ItemTitle] [nvarchar](300) NULL,
	[ADCopy] [nvarchar](300) NULL,
	[Slogen] [nvarchar](300) NULL,
	[ItemDesc] [nvarchar](max) NULL,
	[ShowType] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[ActionType] [int] NULL,
	[MetaTitle] [nvarchar](1024) NULL,
	[MetaKeyword] [nvarchar](1024) NULL,
	[MetaDescription] [nvarchar](1024) NULL,
 CONSTRAINT [PK_landingpage_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'頁面名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'landingpage', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主商品ItemID清單' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'landingpage', @level2type=N'COLUMN',@level2name=N'MainItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主標題清單' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'landingpage', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'landingpage', @level2type=N'COLUMN',@level2name=N'ItemTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'廣告文案' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'landingpage', @level2type=N'COLUMN',@level2name=N'ADCopy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品副標清單' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'landingpage', @level2type=N'COLUMN',@level2name=N'Slogen'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品特點說明清單' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'landingpage', @level2type=N'COLUMN',@level2name=N'ItemDesc'
GO
