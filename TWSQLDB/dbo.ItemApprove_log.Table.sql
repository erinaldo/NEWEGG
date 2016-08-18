USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemApprove_log]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemApprove_log](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemtempID] [int] NOT NULL,
	[ApproveStatus] [int] NOT NULL,
	[ItemID] [int] NULL,
	[Name] [nvarchar](200) NULL,
	[Cost] [decimal](10, 2) NULL,
	[PriceCash] [decimal](10, 2) NULL,
	[ApproveMan] [nvarchar](50) NULL,
	[ApproveDate] [datetime] NULL CONSTRAINT [DF_ItemApprove_log_ApproveDate]  DEFAULT (getdate()),
	[SubmitMan] [nvarchar](50) NULL,
	[SubmitDate] [datetime] NULL,
 CONSTRAINT [PK_ItemApprove_log_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
