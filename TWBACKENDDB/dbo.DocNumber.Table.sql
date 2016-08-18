USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[DocNumber]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocNumber](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DocType] [nvarchar](6) NULL,
	[CurrentNumber] [nvarchar](18) NULL,
	[StartNumber] [nvarchar](18) NULL,
	[EndNumber] [nvarchar](18) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[CurrentNumberV2] [nvarchar](18) NULL,
 CONSTRAINT [PK_DocNumber] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
