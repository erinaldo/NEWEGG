USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PMImageObject]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PMImageObject](
	[ImageID] [int] IDENTITY(1,1) NOT NULL,
	[AlbumImageID] [int] NULL,
	[Path] [varchar](200) NULL,
	[Target] [varchar](10) NULL,
	[FileName] [nvarchar](100) NOT NULL,
	[Effect] [char](1) NOT NULL,
	[Speed] [int] NULL,
	[Title] [nvarchar](100) NULL,
	[Description] [nvarchar](300) NULL,
	[EffectGroupID] [int] NULL,
	[EffectGroupOrder] [int] NULL CONSTRAINT [DF_ImageObject_ImageObj_EffectGroupOrder]  DEFAULT ((0)),
	[Hyperlink] [nvarchar](100) NULL,
	[InUser] [nvarchar](100) NOT NULL,
	[InDate] [datetime] NOT NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[LastEditDate] [datetime] NULL,
 CONSTRAINT [PK_ImageObject] PRIMARY KEY CLUSTERED 
(
	[ImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
