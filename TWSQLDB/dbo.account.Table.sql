USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[account]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[account](
	[ID] [int] IDENTITY(5,1) NOT NULL,
	[Name] [nvarchar](30) NULL,
	[PWD] [varchar](50) NULL,
	[PWDtxt] [nvarchar](50) NULL,
	[PWDenId] [nvarchar](20) NULL,
	[Nickname] [nvarchar](30) NULL,
	[NO] [varchar](20) NULL,
	[Sex] [int] NULL,
	[Type] [int] NULL,
	[Birthday] [char](10) NULL,
	[Email] [varchar](256) NOT NULL,
	[Email2] [varchar](256) NULL,
	[Loc] [nvarchar](10) NULL,
	[Zip] [char](5) NULL,
	[Address] [nvarchar](150) NULL,
	[TelDay] [varchar](30) NULL,
	[TelNight] [varchar](30) NULL,
	[Mobile] [varchar](30) NULL,
	[Fax] [varchar](30) NULL,
	[ConfirmDate] [datetime] NULL,
	[ConfirmCode] [char](8) NULL,
	[Subscribe] [int] NULL,
	[ACTName] [nvarchar](50) NULL,
	[Degree] [int] NULL,
	[Income] [int] NULL,
	[Job] [int] NULL,
	[Marrige] [int] NULL,
	[ServerName] [varchar](50) NULL,
	[Chkfailcnt] [int] NULL,
	[LockedDate] [datetime] NULL,
	[Status] [int] NULL,
	[StatusDate] [datetime] NULL,
	[StatusNote] [nvarchar](50) NULL,
	[Note] [nvarchar](50) NULL,
	[NewLinks] [nvarchar](256) NULL,
	[LoginStatus] [int] NULL,
	[ValidateCode] [varchar](50) NULL,
	[Registeron] [datetime] NULL,
	[Loginon] [datetime] NULL,
	[RememberMe] [int] NULL,
	[AgreePaper] [int] NULL,
	[MessagePaper] [int] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_account_account_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_account_account_updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[FacebookUID] [varchar](20) NULL,
	[Istosap] [int] NULL CONSTRAINT [DF_account_account_tosap]  DEFAULT ((0)),
	[MemberAgreement] [int] NULL,
	[ActionCode] [char](5) NULL,
	[GuestLogin] [int] NOT NULL DEFAULT ((0)),
	[ReceiveEDM] [int] NOT NULL DEFAULT ((1)),
	[InvoiceCarrierReturn] [char](1) NULL,
 CONSTRAINT [PK_account_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
