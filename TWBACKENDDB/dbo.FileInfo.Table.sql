USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[FileInfo]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FileInfo](
	[FileID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](25) NOT NULL,
	[FileName] [nvarchar](50) NOT NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_FileInfo] PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[FileInfo] ADD  CONSTRAINT [DF_FileInfo_Indate]  DEFAULT (getdate()) FOR [InDate]
GO
ALTER TABLE [dbo].[FileInfo] ADD  CONSTRAINT [DF_FileInfo_Updatedate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
