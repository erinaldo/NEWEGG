USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[checkin]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[checkin](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PoCode] [nvarchar](15) NULL,
	[PoItemCode] [nvarchar](15) NULL,
	[RetID] [int] NULL,
	[WhID] [int] NULL,
	[SCMEnabled] [int] NULL,
	[Code] [nvarchar](50) NULL,
	[CheckInUser] [nvarchar](50) NULL,
	[FinalDate] [datetime] NULL,
	[Status] [int] NULL,
	[SupplierID] [int] NULL,
	[SupplierType] [int] NULL,
	[WareHouse] [int] NULL,
	[Type] [int] NULL,
	[VouhCode] [varchar](50) NULL,
	[EndUser] [nvarchar](50) NULL,
	[Note] [nvarchar](200) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[StckStatus] [int] NULL,
	[DelivType] [int] NULL,
	[AsnNumber] [int] NOT NULL CONSTRAINT [DF_checkin_checkin_asnnumber]  DEFAULT ((0)),
	[StckstatusDate] [datetime] NULL,
	[StckstatusUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_checkin_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
