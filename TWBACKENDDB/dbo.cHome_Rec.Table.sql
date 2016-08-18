USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cHome_Rec]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cHome_Rec](
	[OrderID] [varchar](13) NOT NULL,
	[HiProductID] [varchar](8) NOT NULL,
	[TakeDate] [varchar](8) NOT NULL,
	[Qty] [varchar](6) NULL,
	[TrackingNum] [varchar](12) NULL,
	[Type] [varchar](1) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_cHome_Rec] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[HiProductID] ASC,
	[TakeDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[cHome_Rec] ADD  CONSTRAINT [DF_cHome_Rec_Indate]  DEFAULT (getdate()) FOR [InDate]
GO
ALTER TABLE [dbo].[cHome_Rec] ADD  CONSTRAINT [DF_cHome_Rec_Updatedate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
