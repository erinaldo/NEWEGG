USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[delivery]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[delivery](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[ShipperID] [int] NULL,
	[SpID] [int] NULL,
	[Receiver] [nvarchar](50) NULL,
	[ZipCode] [char](5) NULL,
	[ADDR] [nvarchar](150) NULL,
	[Phone] [varchar](30) NULL,
	[Phone2] [varchar](30) NULL,
	[Mobile] [varchar](30) NULL,
	[Email] [varchar](256) NULL,
	[Date] [datetime] NULL,
	[FinalDate] [datetime] NULL,
	[DeliveryUser] [nvarchar](50) NULL,
	[EndUser] [nvarchar](50) NULL,
	[Status] [int] NULL,
	[EcanCode] [varchar](50) NULL,
	[Location] [nvarchar](150) NULL,
	[Carton] [int] NULL,
	[Size] [int] NULL,
	[Gmt] [int] NULL,
	[Cod] [decimal](10, 2) NULL,
	[Filename] [varchar](50) NULL,
	[Note] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[PurchaseorderitemCode] [varchar](50) NULL,
 CONSTRAINT [PK_delivery_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
