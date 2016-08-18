USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PMTextObject]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PMTextObject](
	[TextID] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](3000) NULL,
	[HitCount] [int] NOT NULL CONSTRAINT [DF_TextObject_HitCount]  DEFAULT ((0)),
	[InUser] [nvarchar](100) NOT NULL,
	[InDate] [datetime] NOT NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[LastEditDate] [datetime] NULL,
 CONSTRAINT [PK_ObjectInfo_Text] PRIMARY KEY CLUSTERED 
(
	[TextID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
