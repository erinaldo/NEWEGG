USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[FileErrorInfo]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FileErrorInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FileID] [int] NOT NULL,
	[Type] [varchar](25) NOT NULL,
	[FileName] [nvarchar](50) NOT NULL,
	[Content] [nvarchar](4000) NULL,
	[ErrorType] [int] NOT NULL,
	[ErrorID] [int] NULL,
	[ExInfo] [nvarchar](4000) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL CONSTRAINT [DF_FileErrorInfo_Indate]  DEFAULT (getdate()),
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_FileErrorInfo_Updatedate]  DEFAULT (getdate()),
 CONSTRAINT [PK_FileErrorInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
