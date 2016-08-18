USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[addressbook]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[addressbook](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[RecvName] [nvarchar](30) NOT NULL,
	[RecvTelDay] [varchar](30) NULL,
	[RecvMobile] [varchar](30) NOT NULL,
	[DelivLOC] [nvarchar](10) NOT NULL,
	[DelivZip] [char](5) NOT NULL,
	[DelivAddr] [nvarchar](150) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL CONSTRAINT [DF_addressbook_addressbook_updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[AccountEmail] [varchar](256) NULL,
	[RecvSex] [int] NOT NULL CONSTRAINT [RecvSex]  DEFAULT ((2)),
	[RecvLastName] [nvarchar](20) NULL,
	[RecvFirstName] [nvarchar](20) NULL,
	[DefaultSetting] [int] NOT NULL DEFAULT ((0)),
	[DelivZipName] [nvarchar](10) NULL,
	[DelivAddress] [nvarchar](150) NULL,
	[IsMemberBook] [int] NULL,
	[TelZip] [varchar](4) NULL,
	[TelDay] [varchar](30) NULL,
	[TelExtension] [varchar](10) NULL,
 CONSTRAINT [PK_addressbook_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
