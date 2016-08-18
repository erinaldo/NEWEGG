USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[purchaseordergroup]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[purchaseordergroup](
	[ID] [int] NOT NULL,
	[Vaccunt] [varchar](16) NOT NULL CONSTRAINT [DF_purchaseordergroup_purchaseordergroup_vaccunt]  DEFAULT (''),
	[PriceSum] [decimal](10, 2) NOT NULL CONSTRAINT [DF_purchaseordergroup_purchaseordergroup_pricesum]  DEFAULT ((0)),
	[OrderNum] [int] NOT NULL,
	[Note] [nvarchar](200) NOT NULL CONSTRAINT [DF_purchaseordergroup_purchaseordergroup_note]  DEFAULT (''),
	[CreateUser] [nvarchar](50) NOT NULL CONSTRAINT [DF_purchaseordergroup_purchaseordergroup_createuser]  DEFAULT (''),
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_purchaseordergroup_purchaseordergroup_createdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_purchaseordergroup_purchaseordergroup_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_purchaseordergroup_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
