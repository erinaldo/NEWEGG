USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[advevent]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[advevent](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[HashCode] [nvarchar](4000) NULL,
	[ClickNumber] [int] NOT NULL,
	[AdvType] [int] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[DelDate] [datetime] NULL,
	[StyleClassName1] [nvarchar](300) NULL,
	[StyleClassName2] [nvarchar](300) NULL,
	[SoldoutClassName] [nvarchar](300) NULL,
	[ImgFilterClassName1] [nvarchar](300) NULL,
	[ImgFilterClassName2] [nvarchar](300) NULL,
	[BeforeTitle] [nvarchar](1500) NULL,
	[BeforeSlogan] [nvarchar](1500) NULL,
	[BeforeLinkUrl] [nvarchar](1500) NULL,
	[BeforeImgUrl] [nvarchar](1500) NULL,
	[BeforeImgAlt] [nvarchar](1500) NULL,
	[StartTitle] [nvarchar](1500) NULL,
	[StartSlogan] [nvarchar](1500) NULL,
	[StartLinkUrl] [nvarchar](1500) NULL,
	[StartImgUrl] [nvarchar](1500) NULL,
	[StartImgAlt] [nvarchar](1500) NULL,
	[EndTitle] [nvarchar](1500) NULL,
	[EndSlogan] [nvarchar](1500) NULL,
	[EndLinkUrl] [nvarchar](1500) NULL,
	[EndImgUrl] [nvarchar](1500) NULL,
	[EndImgAlt] [nvarchar](1500) NULL,
	[ItemID] [int] NULL,
	[RecommendItemIDs] [nvarchar](4000) NULL,
	[ExtraApi1] [nvarchar](1000) NULL,
	[ExtraApiAction1] [nvarchar](50) NULL,
	[ExtraApiParameters1] [nvarchar](1500) NULL,
	[ExtraApi2] [nvarchar](1000) NULL,
	[ExtraApiAction2] [nvarchar](50) NULL,
	[ExtraApiParameters2] [nvarchar](1500) NULL,
	[ExtraApi3] [nvarchar](1000) NULL,
	[ExtraApiAction3] [nvarchar](50) NULL,
	[ExtraApiParameters3] [nvarchar](1500) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](100) NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](100) NULL,
	[AdvEventTypeId] [int] NULL,
	[Memo] [nvarchar](40) NULL,
	[OnlineStatus] [int] NOT NULL DEFAULT ((1)),
	[ShowOrder] [int] NOT NULL DEFAULT ((1)),
 CONSTRAINT [PK_advevent] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流水號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Hash Code
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'HashCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'點擊次數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ClickNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'廣告型態' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'AdvType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'起始日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'StartDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'結束日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'EndDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'刪除日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'DelDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CSS class名稱1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'StyleClassName1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CSS class名稱2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'StyleClassName2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'完售圖片遮罩Css class名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'SoldoutClassName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'圖片遮罩Css class名稱1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ImgFilterClassName1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'圖片遮罩Css class名稱2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ImgFilterClassName2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'起始前標題' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'BeforeTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'起始前標語
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'BeforeSlogan'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'起始前連結位置' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'BeforeLinkUrl'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'起始前圖片位置' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'BeforeImgUrl'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'起始前圖片說明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'BeforeImgAlt'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'進行中標題' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'StartTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'進行中標語' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'StartSlogan'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'進行中連結位置' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'StartLinkUrl'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'進行中圖片位置' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'StartImgUrl'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'進行中圖片說明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'StartImgAlt'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'結束後標題' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'EndTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'結束後標語' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'EndSlogan'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'結束後連結位置' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'EndLinkUrl'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'結束後圖片位置' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'EndImgUrl'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'結束後圖片說明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'EndImgAlt'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'本廣告ItemID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ItemID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'推薦ItemIDs，逗號分隔' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'RecommendItemIDs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Api名稱1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ExtraApi1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Api呼叫方式1(REST/Get/Post...)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ExtraApiAction1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Api參數1(Uri格式)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ExtraApiParameters1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Api名稱2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ExtraApi2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Api呼叫方式2(REST/Get/Post...)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ExtraApiAction2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Api參數2(Uri格式)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ExtraApiParameters2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Api名稱3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ExtraApi3'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Api呼叫方式3(REST/Get/Post...)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ExtraApiAction3'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Api參數3(Uri格式)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent', @level2type=N'COLUMN',@level2name=N'ExtraApiParameters3'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'advevent'
GO
