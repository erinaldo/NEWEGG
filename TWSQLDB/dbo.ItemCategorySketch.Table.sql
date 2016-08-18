USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemCategorySketch]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemCategorySketch](
	[ItemSketchID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[FromSystem] [char](1) NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_ItemCategorySketch_CreateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NOT NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_ItemCategorySketch_Updatedate]  DEFAULT (getdate()),
 CONSTRAINT [PK_ItemCategorySketch_id] PRIMARY KEY CLUSTERED 
(
	[ItemSketchID] ASC,
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
