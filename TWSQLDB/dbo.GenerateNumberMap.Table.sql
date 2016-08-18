USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[GenerateNumberMap]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GenerateNumberMap](
	[SourceNumber] [varchar](20) NOT NULL,
	[GenerateNumber] [varchar](30) NOT NULL,
	[GetTime] [datetime] NOT NULL,
 CONSTRAINT [PK_GenerateNumberMap] PRIMARY KEY CLUSTERED 
(
	[SourceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[GenerateNumberMap] ADD  DEFAULT (getdate()) FOR [GetTime]
GO
