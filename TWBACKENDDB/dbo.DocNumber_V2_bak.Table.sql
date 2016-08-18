USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[DocNumber_V2_bak]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocNumber_V2_bak](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DocType] [nvarchar](6) NULL,
	[CurrentNumber] [int] NOT NULL,
	[StartNumber] [nvarchar](18) NULL,
	[EndNumber] [nvarchar](18) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[StartUsingDate] [datetime] NULL,
	[EndUsingDate] [datetime] NULL,
	[CurrentNumberV2] [nvarchar](18) NULL,
	[IsChange] [int] NULL
) ON [PRIMARY]

GO
