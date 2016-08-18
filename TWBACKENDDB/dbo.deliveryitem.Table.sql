USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[deliveryitem]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[deliveryitem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DeliveryID] [int] NULL,
	[ProcessID] [varchar](15) NULL,
	[ProductID] [int] NULL,
	[PktID] [int] NULL,
	[Title] [nvarchar](50) NULL,
	[Attribs] [nvarchar](50) NULL,
	[Price] [decimal](10, 2) NULL,
	[Qty] [int] NULL,
	[InvoiceNO] [varchar](50) NULL,
	[InvoiceTitle] [nvarchar](50) NULL,
	[InvoiceNUM] [varchar](50) NULL,
	[StckDate] [datetime] NULL,
	[StckUser] [char](10) NULL,
	[EraseDate] [datetime] NULL,
	[EraseCause] [int] NULL,
	[EraseCauseNote] [nvarchar](50) NULL,
	[SysDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_deliveryitem_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
