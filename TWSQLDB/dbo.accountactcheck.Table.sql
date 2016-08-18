USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[accountactcheck]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[accountactcheck](
	[Phone] [char](10) NOT NULL,
	[User_id] [char](30) NULL,
	[CrearDate] [datetime] NULL,
	[StatusDate] [datetime] NULL,
	[Status] [int] NULL,
	[FailCount] [int] NULL,
	[Authenticate] [char](6) NULL,
	[Firstname] [nchar](30) NULL,
	[Lastname] [nchar](30) NULL,
	[SMSReturnID] [int] NULL,
 CONSTRAINT [PK_accountactcheck] PRIMARY KEY CLUSTERED 
(
	[Phone] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
