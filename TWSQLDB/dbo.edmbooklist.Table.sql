USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[edmbooklist]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[edmbooklist](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EDMBookID] [int] NOT NULL,
	[EDMTypeName] [nvarchar](200) NULL,
	[EDMOrder] [int] NOT NULL,
	[AdvEventID] [int] NOT NULL,
	[RowSpan] [int] NOT NULL,
	[ColSpan] [int] NOT NULL,
	[FirstStyle] [nvarchar](300) NULL,
	[SecondStyle] [nvarchar](300) NULL,
	[ThirdStyle] [nvarchar](300) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](100) NOT NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](100) NULL,
 CONSTRAINT [PK_edmbooklist] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
