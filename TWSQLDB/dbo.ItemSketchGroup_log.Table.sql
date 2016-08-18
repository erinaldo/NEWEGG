USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemSketchGroup_log]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemSketchGroup_log](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[UserAction] [varchar](15) NULL,
	[EditDate] [datetime] NULL,
	[ID] [int] NULL,
	[ItemSketchID] [int] NULL,
	[PropertyID] [int] NULL,
	[Order] [int] NULL,
	[PropertyName] [nvarchar](500) NULL,
	[CreateDate] [datetime] NULL,
	[InUser] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [int] NULL,
 CONSTRAINT [PK_ItemSketchGroup_log] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
