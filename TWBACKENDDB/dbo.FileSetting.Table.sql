USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[FileSetting]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FileSetting](
	[FilePath] [nvarchar](500) NOT NULL,
	[FileName] [nvarchar](50) NOT NULL,
	[Type] [varchar](25) NOT NULL,
	[Sign] [varchar](9) NULL,
	[IO] [char](1) NOT NULL,
	[ExpEndString] [varchar](50) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL CONSTRAINT [DF_FileSetting_Indate]  DEFAULT (getdate()),
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_FileSetting_Updatedate]  DEFAULT (getdate()),
 CONSTRAINT [PK_FileSetting] PRIMARY KEY CLUSTERED 
(
	[FileName] ASC,
	[Type] ASC,
	[IO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
