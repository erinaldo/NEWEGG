USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[HiLife_IDC_ConfirmInfoRequest]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HiLife_IDC_ConfirmInfoRequest](
	[ORDER_NO] [nvarchar](20) NOT NULL,
	[TRANS_NO] [nvarchar](8) NOT NULL,
	[SHOP_ID] [nvarchar](4) NOT NULL,
	[MMK_ID] [nvarchar](5) NULL,
	[InUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
 CONSTRAINT [PK_dbo.HiLife_IDC_ConfirmInfoRequest] PRIMARY KEY CLUSTERED 
(
	[ORDER_NO] ASC,
	[TRANS_NO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
