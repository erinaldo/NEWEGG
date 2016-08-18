USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[salesordergroupext]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[salesordergroupext](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SalesorderGroupID] [int] NOT NULL,
	[PscartID] [int] NULL,
	[PsSellerID] [varchar](50) NULL,
	[PsCarryNote] [nvarchar](50) NULL,
	[Pshasact] [int] NULL,
	[PshaspartialAuth] [int] NULL CONSTRAINT [DF_ordergroupext_ordergroupext_pshaspartialauth]  DEFAULT ((1)),
	[Status] [int] NOT NULL CONSTRAINT [DF_ordergroupext_ordergroupext_status]  DEFAULT ((99)),
 CONSTRAINT [PK_salesordergroupext_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
