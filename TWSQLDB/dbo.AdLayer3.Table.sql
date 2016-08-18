USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[AdLayer3]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdLayer3](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](8) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[AdType] [int] NOT NULL,
	[Showorder] [int] NOT NULL,
	[ShowAll] [int] NOT NULL,
	[ImagePath] [nvarchar](500) NULL,
	[ImageLink] [nvarchar](500) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_AdLayer3_CateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_AdLayer3_UpdateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_AdLayer3] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
