USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemGroupProperty]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemGroupProperty](
	[GroupID] [int] NOT NULL,
	[PropertyID] [int] NOT NULL,
	[Order] [int] NOT NULL,
	[PropertyName] [nvarchar](500) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemGroupProperty_CreateDate]  DEFAULT (getdate()),
	[InUser] [int] NOT NULL,
	[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemGroupProperty_Updatedate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ItemGroupProperty_1] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
