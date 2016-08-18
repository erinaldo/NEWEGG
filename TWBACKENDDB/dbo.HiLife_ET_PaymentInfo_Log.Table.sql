USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[HiLife_ET_PaymentInfo_Log]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HiLife_ET_PaymentInfo_Log](
	[TRANS_NO] [nvarchar](8) NOT NULL,
	[SHOP_ID] [nvarchar](max) NOT NULL,
	[ORDER_NO] [nvarchar](20) NULL,
 CONSTRAINT [PK_dbo.HiLife_ET_PaymentInfo_Log] PRIMARY KEY CLUSTERED 
(
	[TRANS_NO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
