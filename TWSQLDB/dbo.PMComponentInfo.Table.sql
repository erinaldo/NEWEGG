USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PMComponentInfo]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PMComponentInfo](
	[ComponentID] [int] IDENTITY(1,1) NOT NULL,
	[PageID] [int] NOT NULL,
	[ObjectType] [varchar](50) NOT NULL,
	[ObjectID] [int] NOT NULL,
	[HitCount] [int] NOT NULL CONSTRAINT [DF_ComponentInfo_CompInfo_HitCount]  DEFAULT ((0)),
	[Status] [char](1) NOT NULL,
	[Width] [int] NOT NULL CONSTRAINT [DF_ComponentInfo_CompInfo_Width]  DEFAULT ((100)),
	[Height] [int] NOT NULL CONSTRAINT [DF_ComponentInfo_CompInfo_Height]  DEFAULT ((100)),
	[ZIndex] [int] NOT NULL CONSTRAINT [DF_ComponentInfo_CompInfo_ZIndex]  DEFAULT ((0)),
	[XIndex] [int] NOT NULL CONSTRAINT [DF_ComponentInfo_CompInfo_Top]  DEFAULT ((0)),
	[YIndex] [int] NOT NULL CONSTRAINT [DF_ComponentInfo_CompInfo_Left]  DEFAULT ((0)),
	[InUser] [nvarchar](100) NOT NULL,
	[InDate] [datetime] NOT NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[LastEditDate] [datetime] NULL,
	[Index] [varchar](50) NULL,
 CONSTRAINT [PK_ComponentInfo] PRIMARY KEY CLUSTERED 
(
	[ComponentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
