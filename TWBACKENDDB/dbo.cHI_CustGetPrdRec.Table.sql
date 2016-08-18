USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cHI_CustGetPrdRec]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cHI_CustGetPrdRec](
	[OrderID] [varchar](13) NOT NULL,
	[HiProductID] [varchar](8) NOT NULL,
	[Type] [varchar](1) NOT NULL,
	[TakeDate] [varchar](8) NULL,
	[TakeStoreID] [varchar](4) NULL,
	[RealDate] [varchar](8) NULL,
	[Qty] [varchar](6) NULL,
	[TrackingNum] [varchar](12) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_cHI_CustGetPrdRec] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[HiProductID] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[cHI_CustGetPrdRec] ADD  CONSTRAINT [DF_cHI_CustGetPrdRec_Indate]  DEFAULT (getdate()) FOR [InDate]
GO
ALTER TABLE [dbo].[cHI_CustGetPrdRec] ADD  CONSTRAINT [DF_cHI_CustGetPrdRec_Updatedate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
