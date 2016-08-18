USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemSketchProperty]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemSketchProperty](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemSketchID] [int] NOT NULL,
	[PropertyID] [int] NOT NULL,
	[GroupID] [int] NOT NULL,
	[GroupValueID] [int] NOT NULL,
	[ValueID] [int] NOT NULL,
	[ValueName] [nvarchar](500) NOT NULL,
	[InputValue] [nvarchar](100) NULL,
	[Qty] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemSketchProperty_CreateDate]  DEFAULT (getdate()),
	[InUser] [int] NOT NULL,
	[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemSketchProperty_Updatedate]  DEFAULT (getdate()),
	[UpdateUser] [int] NOT NULL,
 CONSTRAINT [PK_ItemSketchProperty] PRIMARY KEY CLUSTERED 
(
	[PropertyID] ASC,
	[GroupID] ASC,
	[GroupValueID] ASC,
	[ValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
