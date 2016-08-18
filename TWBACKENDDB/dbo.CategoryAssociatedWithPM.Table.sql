USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[CategoryAssociatedWithPM]    Script Date: 2016/08/18 14:12:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CategoryAssociatedWithPM](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[ProductMananeger] [nvarchar](20) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [df_CreateDate]  DEFAULT (getdate()),
	[CreateUser] [nvarchar](50) NOT NULL,
	[Updated] [int] NOT NULL CONSTRAINT [df_Updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[Delvtype] [nvarchar](20) NOT NULL CONSTRAINT [DF__CategoryA__Delvt__3B0BC30C]  DEFAULT (N'f'),
	[ManagerName] [varchar](50) NOT NULL,
	[ActiveStatus] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [pk_CategoryAssociatedWithPM] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC,
	[ManagerName] ASC,
	[Delvtype] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
