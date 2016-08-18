USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[HiLife_ET_PaymentInfo]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HiLife_ET_PaymentInfo](
	[ORDER_NO] [nvarchar](20) NOT NULL,
	[TRANS_NO] [nvarchar](8) NOT NULL,
	[SHOP_ID] [nvarchar](4) NOT NULL,
	[MMK_ID] [nvarchar](5) NOT NULL,
	[RECEIPT] [nvarchar](1) NULL,
	[PRD_ITEM] [nvarchar](50) NULL,
	[PRD_NAME] [nvarchar](50) NULL,
	[AMOUNT] [decimal](18, 0) NULL,
	[ACCOUNT] [nvarchar](20) NULL,
	[C002_URL] [nvarchar](255) NULL,
	[InUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
 CONSTRAINT [PK_dbo.HiLife_ET_PaymentInfo] PRIMARY KEY CLUSTERED 
(
	[ORDER_NO] ASC,
	[TRANS_NO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
