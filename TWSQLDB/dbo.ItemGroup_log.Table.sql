USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemGroup_log]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemGroup_log](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[UserAction] [varchar](15) NULL,
	[EditDate] [datetime] NULL,
	[ID] [int] NULL,
	[SellerID] [int] NULL,
	[Amount] [int] NULL,
	[MasterPropertyID] [int] NULL,
	[CreateDate] [datetime] NULL,
	[InUser] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
