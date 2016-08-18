USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[prblm]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[prblm](
	[Code] [varchar](15) NOT NULL,
	[AccountID] [int] NOT NULL CONSTRAINT [DF_prblm_prblm_accountid]  DEFAULT ((0)),
	[PrblmCode] [varchar](15) NULL,
	[Blng] [nvarchar](50) NULL,
	[BlngCode] [nvarchar](50) NULL,
	[ItemID] [int] NULL,
	[ProcessID] [int] NULL,
	[SubStation] [int] NULL,
	[Source] [int] NULL,
	[Class] [int] NULL,
	[IntClass] [int] NULL,
	[Email] [nvarchar](256) NULL,
	[Name] [nvarchar](50) NULL,
	[TEL] [varchar](30) NULL,
	[Cont] [nvarchar](1000) NULL,
	[ProcessUser] [nvarchar](50) NULL,
	[Subject] [nvarchar](200) NULL,
	[ServerName] [varchar](50) NULL,
	[PreassignBeginDate] [datetime] NULL,
	[PreassignEndDate] [datetime] NULL,
	[ScmdelvFailid] [int] NULL,
	[Note] [nvarchar](1000) NULL,
	[Status] [int] NULL,
	[StatusNote] [nvarchar](50) NULL,
	[StatusDate] [datetime] NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_prblm_prblm_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_prblm_prblm_updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_prblm_code] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
