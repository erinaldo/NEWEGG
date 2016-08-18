USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[drawinglist]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[drawinglist](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DrawingCode] [varchar](25) NOT NULL,
	[DrawingAccID] [int] NOT NULL,
	[DrawingTimes] [int] NOT NULL,
	[DrawingName] [nvarchar](30) NOT NULL,
	[DrawingPhone] [varchar](30) NOT NULL,
	[DrawingBirthday] [datetime] NOT NULL,
	[DrawingGender] [nvarchar](10) NOT NULL,
	[DrawingAddress] [nvarchar](200) NOT NULL,
	[DrawingIDNO] [nvarchar](20) NOT NULL,
	[DrawingPrizeID] [int] NOT NULL,
	[DrawingPrizeName] [nvarchar](50) NOT NULL,
	[DrawingPrizeItem] [nvarchar](50) NOT NULL,
	[DrawingPrizePrice] [decimal](10, 2) NOT NULL,
	[DrawingPrizeIsGive] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](10) NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](10) NULL,
 CONSTRAINT [PK_drawinglist] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
