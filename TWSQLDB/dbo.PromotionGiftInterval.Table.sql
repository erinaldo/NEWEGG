USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PromotionGiftInterval]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PromotionGiftInterval](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PromotionGiftBasicID] [int] NOT NULL,
	[LowerLimit] [decimal](10, 2) NOT NULL,
	[UpperLimit] [decimal](10, 2) NOT NULL,
	[DiscountAmount] [decimal](10, 2) NOT NULL,
	[DiscountPercent] [decimal](10, 2) NOT NULL CONSTRAINT [DF_PromotionGiftInterval_DiscountPercent]  DEFAULT ((0)),
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	[CreateUser] [varchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [varchar](50) NULL,
 CONSTRAINT [PK_PromotionGiftInterval] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'滿額贈ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftInterval', @level2type=N'COLUMN',@level2name=N'PromotionGiftBasicID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'下限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftInterval', @level2type=N'COLUMN',@level2name=N'LowerLimit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftInterval', @level2type=N'COLUMN',@level2name=N'UpperLimit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'購物車折價金額' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftInterval', @level2type=N'COLUMN',@level2name=N'DiscountAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'折扣百分比 ex:95折請填入5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PromotionGiftInterval', @level2type=N'COLUMN',@level2name=N'DiscountPercent'
GO
