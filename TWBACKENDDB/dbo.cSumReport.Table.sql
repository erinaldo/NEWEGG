USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cSumReport]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cSumReport](
	[FileName] [varchar](50) NOT NULL,
	[Count] [varchar](6) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL CONSTRAINT [DF_cSumReport_Indate]  DEFAULT (getdate()),
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_cSumReport_Updatedate]  DEFAULT (getdate()),
	[ExportDate] [datetime] NULL,
 CONSTRAINT [PK_cSumReport] PRIMARY KEY CLUSTERED 
(
	[FileName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
