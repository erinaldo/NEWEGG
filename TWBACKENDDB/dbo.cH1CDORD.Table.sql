USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cH1CDORD]    Script Date: 2016/08/18 14:12:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cH1CDORD](
	[TakeStoreID] [varchar](4) NOT NULL,
	[GoodStoreID] [varchar](4) NOT NULL,
	[OrderCycle] [varchar](7) NULL,
	[StartDate] [varchar](8) NOT NULL,
	[EndDate] [varchar](8) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_cH1CDORD] PRIMARY KEY CLUSTERED 
(
	[TakeStoreID] ASC,
	[GoodStoreID] ASC,
	[StartDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[cH1CDORD] ADD  CONSTRAINT [DF_cH1CDORD_Indate]  DEFAULT (getdate()) FOR [InDate]
GO
ALTER TABLE [dbo].[cH1CDORD] ADD  CONSTRAINT [DF_cH1CDORD_Updatedate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
