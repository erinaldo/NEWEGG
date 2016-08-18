USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[checkoutitem]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[checkoutitem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CheckoutID] [int] NULL,
	[SoCode] [varchar](15) NULL,
	[SoitemCode] [varchar](15) NULL,
	[ProductID] [int] NULL,
	[Attribs] [nvarchar](200) NULL,
	[Qty] [int] NULL,
	[Price] [decimal](10, 2) NULL,
	[Stckdate] [datetime] NULL,
	[Stckuser] [nvarchar](50) NULL,
	[EraseDate] [datetime] NULL,
	[EraseCause] [int] NULL,
	[EraseCauseNote] [nvarchar](200) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[QtyBad] [int] NULL DEFAULT ((0)),
	[QtyDead] [int] NULL DEFAULT ((0)),
 CONSTRAINT [PK_checkoutitem_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
