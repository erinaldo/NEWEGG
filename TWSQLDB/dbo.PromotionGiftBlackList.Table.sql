USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PromotionGiftBlackList]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PromotionGiftBlackList](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PromotionGiftBasicID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_PromotionGiftBlackList_Status]  DEFAULT ((0)),
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	[CreateUser] [varchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [varchar](50) NULL,
 CONSTRAINT [PK_PromotionGiftBlackList] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'滿額贈ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftBlackList', @level2type=N'COLUMN',@level2name=N'PromotionGiftBasicID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'黑名單ItemID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftBlackList', @level2type=N'COLUMN',@level2name=N'ItemID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'黑名單ItemID是否啟用的設定狀態(TempUsed = 1, Used = 2, NotUsed = 3)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftBlackList', @level2type=N'COLUMN',@level2name=N'Status'
GO
