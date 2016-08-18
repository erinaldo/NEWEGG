USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PMDynamicObject]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PMDynamicObject](
	[DynamicID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[Api] [nvarchar](50) NOT NULL,
	[Parameters] [varchar](50) NULL,
	[InUser] [varchar](50) NOT NULL,
	[InDate] [datetime] NOT NULL,
	[LastEditUser] [varchar](50) NULL,
	[LastEditDate] [datetime] NULL,
 CONSTRAINT [PK_DataObject] PRIMARY KEY CLUSTERED 
(
	[DynamicID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
