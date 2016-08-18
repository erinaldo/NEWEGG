USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[Image_log]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Image_log](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[Container] [nvarchar](500) NOT NULL,
	[FromPath] [nvarchar](500) NOT NULL,
	[TargetPath] [nvarchar](500) NOT NULL,
	[FromSystem] [nvarchar](500) NOT NULL,
	[UpdateUser] [nvarchar](50) NOT NULL,
	[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_Image_log_UpdateDate]  DEFAULT (getdate()),
	[Status] [nvarchar](50) NOT NULL,
	[UserAction] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Image_log] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
