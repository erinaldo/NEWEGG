USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[generatenumber]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[generatenumber](
	[LogiticsName] [nvarchar](30) NOT NULL,
	[LogiticsCode] [char](1) NOT NULL,
	[TodayNumber] [int] NOT NULL,
 CONSTRAINT [PK_generatenumber] PRIMARY KEY CLUSTERED 
(
	[LogiticsCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
