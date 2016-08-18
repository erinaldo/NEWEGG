USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PromotionGiftBasic]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PromotionGiftBasic](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Categories] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_PromotionGiftBasic_Status]  DEFAULT ((0)),
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Priority] [int] NOT NULL DEFAULT ((0)),
	[ShowDesc] [nvarchar](50) NULL,
	[ReferencesList] [nvarchar](50) NULL,
	[CSS] [varchar](50) NULL,
	[HighLight] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	[CreateUser] [varchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [varchar](50) NULL,
 CONSTRAINT [PK_PromotionGiftBasic] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'滿額贈ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftBasic', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'類別ID String' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftBasic', @level2type=N'COLUMN',@level2name=N'Categories'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'說明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftBasic', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'狀態' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftBasic', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'起始時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftBasic', @level2type=N'COLUMN',@level2name=N'StartDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'結束時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftBasic', @level2type=N'COLUMN',@level2name=N'EndDate'
GO
