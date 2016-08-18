USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[Seller_CorrectionPrice]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Seller_CorrectionPrice](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[SellerID] [int] NULL DEFAULT ((0)),
	[Subject] [nvarchar](100) NULL DEFAULT (''),
	[Description] [nvarchar](500) NULL DEFAULT (''),
	[FinanStatus] [nvarchar](50) NOT NULL DEFAULT (''),
	[TotalAmount] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_CorrectionPrice_TotalAmount]  DEFAULT ((0.00)),
	[PurePrice] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_CorrectionPrice_PurePrice]  DEFAULT ((0.00)),
	[Tax] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_CorrectionPrice_Tax]  DEFAULT ((0.00)),
	[SettlementID] [nvarchar](50) NOT NULL DEFAULT (''),
	[CreateUser] [nvarchar](50) NOT NULL DEFAULT (''),
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_Seller_CorrectionPrice] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商家編號;供應商代號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Seller_CorrectionPrice', @level2type=N'COLUMN',@level2name=N'SellerID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Seller_CorrectionPrice', @level2type=N'COLUMN',@level2name=N'FinanStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'調整金額(含稅)：數值，可填入負值，四捨五入至整數位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Seller_CorrectionPrice', @level2type=N'COLUMN',@level2name=N'TotalAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'調整金額(未稅)：數值，可填入負值，四捨五入至整數位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Seller_CorrectionPrice', @level2type=N'COLUMN',@level2name=N'PurePrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'稅額：數值，可填入負值，四捨五入至整數位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Seller_CorrectionPrice', @level2type=N'COLUMN',@level2name=N'Tax'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'對帳單編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Seller_CorrectionPrice', @level2type=N'COLUMN',@level2name=N'SettlementID'
GO
