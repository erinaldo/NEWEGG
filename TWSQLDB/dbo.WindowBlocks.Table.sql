USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[WindowBlocks]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WindowBlocks](
	[SubCategoryID] [int] NOT NULL,
	[ZoneID] [int] NOT NULL,
	[ZoneStyle] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_WindowBlocks_CateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_WindowBlocks] PRIMARY KEY CLUSTERED 
(
	[SubCategoryID] ASC,
	[ZoneID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
