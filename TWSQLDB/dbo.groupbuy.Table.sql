USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[groupbuy]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[groupbuy](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[ItemName] [nvarchar](500) NOT NULL CONSTRAINT [DF_groupbuy_ItemName]  DEFAULT (''),
	[OriginalPrice] [decimal](10, 5) NOT NULL,
	[GroupBuyPrice] [decimal](10, 5) NOT NULL,
	[ProductCost] [decimal](10, 5) NOT NULL,
	[ShippingCost] [decimal](10, 5) NOT NULL,
	[SalesOrderLimit] [int] NOT NULL,
	[SalesOrderBase] [int] NOT NULL,
	[IsExclusive] [bit] NOT NULL,
	[IsNeweggUSASync] [bit] NOT NULL,
	[PromoText] [nvarchar](max) NOT NULL,
	[ImgUrl] [varchar](512) NOT NULL,
	[BeginDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IsWaitingForApprove] [bit] NOT NULL CONSTRAINT [DF_groupbuy_IsWaitingForApprove]  DEFAULT ((0)),
	[IsApprove] [bit] NOT NULL,
	[IsReject] [bit] NOT NULL CONSTRAINT [DF_groupbuy_IsReject]  DEFAULT ((0)),
	[RejectCause] [nvarchar](500) NOT NULL,
	[IsHide] [bit] NOT NULL CONSTRAINT [DF_groupbuy_IsHide]  DEFAULT ((0)),
	[InUser] [nvarchar](10) NOT NULL,
	[InDate] [datetime] NOT NULL,
	[EditUser] [nvarchar](10) NULL,
	[EditDate] [datetime] NULL,
 CONSTRAINT [PK_groupbuy] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
