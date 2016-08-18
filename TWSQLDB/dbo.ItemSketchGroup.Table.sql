USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemSketchGroup]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemSketchGroup](
	[ID] [int] NOT NULL,
	[ItemSketchID] [int] NOT NULL,
	[PropertyID] [int] NULL,
	[Order] [int] NOT NULL,
	[PropertyName] [nvarchar](500) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemSketchGroup_CreateDate]  DEFAULT (getdate()),
	[InUser] [int] NOT NULL,
	[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemSketchGroup_Updatedate]  DEFAULT (getdate()),
	[UpdateUser] [int] NOT NULL,
 CONSTRAINT [PK_ItemSketchGroup_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[ItemSketchID] ASC,
	[Order] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
