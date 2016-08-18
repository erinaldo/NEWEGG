USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[DailyEstimate]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DailyEstimate](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NOT NULL,
	[Mouth] [int] NOT NULL,
	[Date] [int] NOT NULL,
	[Estimate] [decimal](12, 4) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[SetCreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_DailyEstimate] PRIMARY KEY CLUSTERED 
(
	[Year] ASC,
	[Mouth] ASC,
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
