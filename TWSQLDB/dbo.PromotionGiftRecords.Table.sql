USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PromotionGiftRecords]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PromotionGiftRecords](
	[PromotionGiftBasicID] [int] NOT NULL,
	[SalesOrderItemCode] [varchar](15) NOT NULL,
	[PromotionGiftIntervalID] [int] NOT NULL,
	[DiscountAmount] [decimal](10, 2) NOT NULL,
	[UsedStatus] [int] NOT NULL CONSTRAINT [DF_PromotionGiftRecords_UsedStatus]  DEFAULT ((3)),
	[ApportionedAmount] [decimal](10, 2) NOT NULL,
	[SAIn] [int] NULL,
	[SAOut] [int] NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_PromotionGiftRecords_CreateDate]  DEFAULT (getdate()),
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_PromotionGiftRecords] PRIMARY KEY CLUSTERED 
(
	[PromotionGiftBasicID] ASC,
	[SalesOrderItemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'滿額贈ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftRecords', @level2type=N'COLUMN',@level2name=N'PromotionGiftBasicID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂單子單編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftRecords', @level2type=N'COLUMN',@level2name=N'SalesOrderItemCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'級距ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftRecords', @level2type=N'COLUMN',@level2name=N'PromotionGiftIntervalID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'級距的折扣金額' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftRecords', @level2type=N'COLUMN',@level2name=N'DiscountAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用狀態( Used = 1, NotUsed = 2, TempUsed = 3, CancelUsed = 4, 初始狀態為TempUsed = 3)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftRecords', @level2type=N'COLUMN',@level2name=N'UsedStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂單子單分攤金額' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftRecords', @level2type=N'COLUMN',@level2name=N'ApportionedAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'產生入會計' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftRecords', @level2type=N'COLUMN',@level2name=N'SAIn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'報廢入會計' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftRecords', @level2type=N'COLUMN',@level2name=N'SAOut'
GO
