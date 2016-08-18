USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[nodefromws]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[nodefromws](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[SubcategoryID] [int] NOT NULL,
	[NodeID] [int] NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](150) NOT NULL,
	[ItemCount] [int] NOT NULL,
	[WebURL] [varchar](250) NULL,
 CONSTRAINT [PK_nodefromws_sn] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
