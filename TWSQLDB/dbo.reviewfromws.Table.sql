USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[reviewfromws]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[reviewfromws](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProductfromwsID] [int] NOT NULL,
	[Title] [nvarchar](200) NULL,
	[Rating] [int] NULL,
	[PublishDate] [datetime] NULL,
	[Nickname] [nvarchar](50) NULL,
	[PROS] [nvarchar](4000) NULL,
	[CONS] [nvarchar](4000) NULL,
	[Comments] [nvarchar](4000) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_reviewfromws_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
