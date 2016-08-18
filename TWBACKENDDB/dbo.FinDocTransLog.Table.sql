USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[FinDocTransLog]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FinDocTransLog](
	[Index] [int] IDENTITY(1,1) NOT NULL,
	[ActionType] [nvarchar](5) NULL,
	[DocType] [nvarchar](10) NULL,
	[DocNo] [nvarchar](200) NULL,
	[FileName] [nvarchar](200) NULL,
	[ResultType] [nvarchar](10) NULL,
	[Reason] [nvarchar](1000) NULL,
	[TransactionNumber] [nvarchar](200) NULL,
	[TransTime] [nvarchar](200) NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.FinDocTransLog] PRIMARY KEY CLUSTERED 
(
	[Index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
