USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ProductPropertySketch]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductPropertySketch](
	[ItemSketchID] [int] NOT NULL,
	[ProductValueID] [int] NOT NULL,
	[UserInputValue] [nvarchar](50) NULL,
	[UserInputValueTW] [nvarchar](50) NULL,
	[Show] [int] NULL,
	[Label] [int] NULL,
	[GroupID] [int] NULL,
	[PropertyNameID] [int] NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_ProductPropertySketch_CreateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NOT NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_ProductPropertySketch_Updatedate]  DEFAULT (getdate()),
 CONSTRAINT [PK_ProductPropertySketch_id] PRIMARY KEY CLUSTERED 
(
	[ItemSketchID] ASC,
	[ProductValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
