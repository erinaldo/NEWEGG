USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemForChoice]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemForChoice](
	[CategoryID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[Showorder] [int] NOT NULL,
	[ShowAll] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_ItemForChoice] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC,
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
