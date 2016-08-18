USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[CategoryTopItem]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CategoryTopItem](
	[CategoryID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[ItemType] [int] NOT NULL,
	[Showorder] [int] NOT NULL,
	[ShowAll] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_CategoryTopItem_CateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_CategoryTopItem_UpdateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_CategoryTopItem] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC,
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
