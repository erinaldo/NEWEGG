USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[CustomErrorInfo]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomErrorInfo](
	[ErrorID] [int] IDENTITY(1,1) NOT NULL,
	[ErrorInfo] [nvarchar](50) NOT NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL CONSTRAINT [DF_CustomErrorInfo_Indate]  DEFAULT (getdate()),
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_CustomErrorInfo_Updatedate]  DEFAULT (getdate()),
 CONSTRAINT [PK_CustomErrorInfo] PRIMARY KEY CLUSTERED 
(
	[ErrorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
