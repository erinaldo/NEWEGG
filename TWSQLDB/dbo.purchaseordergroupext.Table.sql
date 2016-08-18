USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[purchaseordergroupext]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[purchaseordergroupext](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseorderGroupID] [int] NOT NULL,
	[PsCartID] [int] NULL,
	[PsSellerID] [varchar](50) NULL,
	[PsCarryNote] [nvarchar](50) NULL,
	[Pshasact] [int] NULL,
	[PshasPartialAuth] [int] NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_purchaseordergroupext_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[purchaseordergroupext] ADD  CONSTRAINT [DF_purchaseordergroupext_purchaseordergroupext_pshaspartialauth]  DEFAULT ((1)) FOR [PshasPartialAuth]
GO
ALTER TABLE [dbo].[purchaseordergroupext] ADD  CONSTRAINT [DF_purchaseordergroupext_purchaseordergroupext_status]  DEFAULT ((99)) FOR [Status]
GO
