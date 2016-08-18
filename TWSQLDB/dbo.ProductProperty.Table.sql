USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ProductProperty]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductProperty](
	[ProductID] [int] NOT NULL,
	[ProductValueID] [int] NOT NULL,
	[UserInputValue] [nvarchar](500) NULL,
	[UserInputValueTW] [nvarchar](500) NULL,
	[Show] [int] NULL CONSTRAINT [DF_ProductProperty_Show]  DEFAULT ((0)),
	[Label] [int] NULL CONSTRAINT [DF_ProductProperty_Label]  DEFAULT ((0)),
	[GroupID] [int] NOT NULL,
	[PropertyNameID] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_ProductProperty_CreateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_ProductProperty_UpdateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_ProductProperty] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC,
	[ProductValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
