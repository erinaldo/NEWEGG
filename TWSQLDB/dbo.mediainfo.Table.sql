USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[mediainfo]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mediainfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LaunchDate] [date] NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Snapshotpath] [nvarchar](300) NOT NULL,
	[Clickpath] [nvarchar](300) NOT NULL,
	[Displaypath] [nvarchar](300) NOT NULL,
	[Displaytype] [int] NOT NULL,
	[ShowAll] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_mediainfo_CateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_mediainfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
