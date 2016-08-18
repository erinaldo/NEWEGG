USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[CargoStatus]    Script Date: 2016/08/18 14:12:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CargoStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderNo] [nvarchar](20) NOT NULL,
	[ShipID] [nvarchar](9) NULL,
	[SellerID] [nvarchar](9) NULL,
	[ShipStatus] [nvarchar](10) NULL,
	[ShipIssue] [nvarchar](200) NULL,
	[Note] [nvarchar](200) NULL,
	[SubHawbNums] [int] NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_CargoStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
