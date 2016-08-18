USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cShop_DC_TakeDay]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cShop_DC_TakeDay](
	[PreID] [varchar](10) NOT NULL,
	[TakeStoreID] [varchar](4) NOT NULL,
	[BuyDate] [varchar](8) NOT NULL,
	[TakeDate1] [varchar](8) NULL,
	[TakeDate2] [varchar](8) NULL,
	[TakeDate3] [varchar](8) NULL,
	[TakeDate4] [varchar](8) NULL,
	[GoodStoreID] [varchar](4) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_cShop_DC_TakeDay] PRIMARY KEY CLUSTERED 
(
	[PreID] ASC,
	[TakeStoreID] ASC,
	[BuyDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[cShop_DC_TakeDay] ADD  CONSTRAINT [DF_cShop_DC_TakeDay_Indate]  DEFAULT (getdate()) FOR [InDate]
GO
ALTER TABLE [dbo].[cShop_DC_TakeDay] ADD  CONSTRAINT [DF_cShop_DC_TakeDay_Updatedate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
