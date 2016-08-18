USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[storefromws]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[storefromws](
	[ID] [int] NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[DEPA] [nvarchar](50) NOT NULL,
	[Showall] [int] NOT NULL,
	[ShowOrder] [int] NULL,
 CONSTRAINT [PK_storefromws_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
