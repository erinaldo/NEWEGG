USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[adveventtype]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[adveventtype](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AdvTypeCode] [int] NOT NULL,
	[AdvTypeName] [nvarchar](200) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](100) NOT NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](100) NULL,
	[MaxAd] [int] NULL,
	[CacheMins] [int] NULL,
	[CSS] [varchar](30) NULL,
	[Country] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_adveventtype] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
