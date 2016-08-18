USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[NeweggBoxInfo]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[NeweggBoxInfo](
	[Index] [int] IDENTITY(1,1) NOT NULL,
	[BoxNumber] [int] NOT NULL,
	[Area] [nvarchar](10) NULL,
	[Length] [decimal](6, 3) NOT NULL,
	[Width] [decimal](6, 3) NOT NULL,
	[Height] [decimal](6, 3) NOT NULL,
	[Unit] [varchar](15) NULL,
 CONSTRAINT [PK_dbo.NeweggBoxInfo] PRIMARY KEY CLUSTERED 
(
	[Index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
