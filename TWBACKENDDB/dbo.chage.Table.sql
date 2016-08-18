USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[chage]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[chage](
	[ID] [int] NOT NULL,
	[CartID] [varchar](50) NULL,
	[NO] [varchar](50) NULL,
	[Updated] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_chage_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
