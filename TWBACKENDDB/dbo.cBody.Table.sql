USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cBody]    Script Date: 2016/08/18 14:12:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cBody](
	[OrderID] [varchar](13) NOT NULL,
	[PrdID] [varchar](10) NULL,
	[HiProductID] [varchar](8) NOT NULL,
	[Qty] [varchar](6) NULL,
	[TakeStoreID] [varchar](4) NULL,
	[TakeDate] [varchar](8) NOT NULL,
	[TranTime] [varchar](20) NULL,
	[PromoCode] [varchar](8) NULL,
	[TodayPrice] [varchar](6) NULL,
	[Discount] [varchar](6) NULL,
	[ProSn] [varchar](11) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL CONSTRAINT [DF_cBody_Indate]  DEFAULT (getdate()),
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_cBody_Updatedate]  DEFAULT (getdate()),
	[ExportDate] [datetime] NULL,
 CONSTRAINT [PK_cBody] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[HiProductID] ASC,
	[TakeDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
