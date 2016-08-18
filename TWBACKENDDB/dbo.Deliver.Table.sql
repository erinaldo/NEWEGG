USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[Deliver]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Deliver](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[WebSite] [nvarchar](200) NULL,
	[DeliverType] [int] NULL,
	[IDForTrackNo2] [int] NULL,
 CONSTRAINT [PK_Deliver] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
