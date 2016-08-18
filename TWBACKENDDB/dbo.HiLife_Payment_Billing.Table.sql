USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[HiLife_Payment_Billing]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HiLife_Payment_Billing](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TRANS_NO] [nvarchar](8) NULL,
	[SHOP_ID] [nvarchar](4) NULL,
	[ORDER_NO] [nvarchar](20) NULL,
	[SERVICE_ID] [nvarchar](5) NULL,
	[AMOUNT] [decimal](18, 0) NULL,
	[PAY_DATE] [nvarchar](10) NULL,
	[TRN_DATE] [nvarchar](10) NULL,
	[UpdateDate] [datetime] NULL,
	[InUserID] [int] NULL,
	[UpdateUserID] [int] NULL,
 CONSTRAINT [PK_dbo.HiLife_Payment_Billing] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
