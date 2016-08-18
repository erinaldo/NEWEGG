USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemPropertyGroup]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemPropertyGroup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NULL,
	[GroupName] [nvarchar](50) NULL,
	[GroupNameTW] [nchar](10) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[Hide] [char](1) NOT NULL DEFAULT ('F'),
	[ShowOrder] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_ItemPropertyGroup] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
