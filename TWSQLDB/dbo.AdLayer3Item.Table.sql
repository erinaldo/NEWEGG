USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[AdLayer3Item]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdLayer3Item](
	[AdLayer3ID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[Showorder] [int] NOT NULL,
	[ShowAll] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_AdLayer3Item_CateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_AdLayer3Item_UpdateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_AdLayer3Item] PRIMARY KEY CLUSTERED 
(
	[AdLayer3ID] ASC,
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
