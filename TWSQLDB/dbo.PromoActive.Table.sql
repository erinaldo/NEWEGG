USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PromoActive]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PromoActive](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](10) NOT NULL,
	[CategoryLink] [nvarchar](300) NULL,
	[FuncType] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[NameLink] [nvarchar](300) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Note] [nvarchar](1000) NULL,
	[Type] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[TakeType] [int] NULL,
	[DeclareDate] [datetime] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_PromoActive_createdate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_PromoActive_UpdateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_PromoActive] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
