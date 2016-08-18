USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[twtradetax]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[twtradetax](
	[ID] [char](11) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[ENGName] [nvarchar](50) NULL,
	[Rate1] [nvarchar](50) NULL,
	[Rate2] [nvarchar](50) NULL,
	[Rate3] [nvarchar](50) NULL,
	[StaticUnit] [nvarchar](20) NULL,
	[WeightUnit] [nvarchar](20) NULL,
	[TWTradeTaxRule] [nvarchar](20) NULL,
	[ImportRule] [nvarchar](50) NULL,
	[ExportRule] [nvarchar](50) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_twtradetax_twtradetax_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_twtradetax_twtradetax_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_twtradetax_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
