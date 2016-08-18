USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[logistic]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[logistic](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FulCharge] [decimal](10, 4) NULL,
	[Weight] [decimal](10, 2) NULL,
	[Expense] [decimal](10, 2) NOT NULL,
	[SellerID] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL CONSTRAINT [DF_logistic_logistic_createuser]  DEFAULT ('auto_insert'),
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_logistic_logistic_createdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_logistic_logistic_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_logistic_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
