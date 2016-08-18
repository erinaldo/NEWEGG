USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[OrderQueueLog]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderQueueLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNumber] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nchar](50) NOT NULL,
	[LockedBy] [nchar](50) NOT NULL,
	[Status] [int] NOT NULL,
	[ErrMsg] [nvarchar](max) NULL,
 CONSTRAINT [PK_OrderQueueLog] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
