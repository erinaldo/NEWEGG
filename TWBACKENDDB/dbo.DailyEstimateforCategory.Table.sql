USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[DailyEstimateforCategory]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DailyEstimateforCategory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NOT NULL,
	[Mouth] [int] NOT NULL,
	[CategoryName] [nvarchar](50) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[Estimate] [decimal](12, 4) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_DailyEstimateforCategory_1_CreateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_DailyEstimateforCategory_1_UpdateDate]  DEFAULT (getdate()),
	[SetCreateDate] [datetime] NULL,
 CONSTRAINT [PK_DailyEstimateforCategory_1] PRIMARY KEY CLUSTERED 
(
	[Year] ASC,
	[Mouth] ASC,
	[CategoryName] ASC,
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
