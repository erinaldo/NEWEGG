USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemstockhstry]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[itemstockhstry](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[OldQty] [int] NOT NULL,
	[NewQty] [int] NOT NULL,
	[OldSafeQty] [int] NOT NULL,
	[NewSafeQty] [int] NOT NULL,
	[Oldfdbcklmt] [int] NOT NULL,
	[Newfdbcklmt] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_itemstockhstry_itemstockhstry_createdate]  DEFAULT (getdate()),
 CONSTRAINT [PK_itemstockhty_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
