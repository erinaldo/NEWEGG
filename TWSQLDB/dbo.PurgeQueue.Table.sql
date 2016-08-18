USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PurgeQueue]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurgeQueue](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[URL] [nvarchar](1000) NOT NULL,
	[isPurged] [int] NOT NULL CONSTRAINT [DF_PurgeQueue_isPurged]  DEFAULT ((0)),
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PurgeQueue] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
