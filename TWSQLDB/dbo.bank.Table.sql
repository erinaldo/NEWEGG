USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[bank]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[bank](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Code] [varchar](30) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_bank_bank_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_bank_bank_updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[Referred] [nvarchar](50) NULL,
 CONSTRAINT [PK_bank_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
