USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[HiTrust]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HiTrust](
	[MerConfigName] [nvarchar](15) NOT NULL,
	[BnkID] [varchar](7) NOT NULL,
	[DateStart] [datetime] NOT NULL,
	[DateEnd] [datetime] NULL,
	[IsOnce] [int] NOT NULL,
	[UpdateUrl] [nvarchar](256) NOT NULL,
	[merupdateURL] [nvarchar](256) NOT NULL,
	[returnURL] [nvarchar](256) NOT NULL,
	[HiServer] [nvarchar](300) NOT NULL,
	[MerConfig] [nvarchar](300) NOT NULL,
	[QueryFlag] [varchar](1) NOT NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_HiTrust_createdate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_HiTrust_updatedate]  DEFAULT (getdate()),
 CONSTRAINT [PK_HiTrust_id] PRIMARY KEY CLUSTERED 
(
	[BnkID] ASC,
	[DateStart] ASC,
	[IsOnce] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
