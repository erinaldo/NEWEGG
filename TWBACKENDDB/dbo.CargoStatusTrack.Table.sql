USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[CargoStatusTrack]    Script Date: 2016/08/18 14:12:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CargoStatusTrack](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderNo] [nvarchar](20) NOT NULL,
	[TrackNo] [int] NOT NULL,
	[SubHawb] [nvarchar](30) NULL,
	[DeliveryNo] [nvarchar](20) NULL,
	[HawbDate] [nvarchar](20) NULL,
	[HawbType] [nvarchar](10) NULL,
	[HawbSite] [nvarchar](20) NULL,
	[HawbMan] [nvarchar](20) NULL,
	[HawbManCode] [nvarchar](20) NULL,
	[DispatchorSendMan] [nvarchar](20) NULL,
	[PreOrNextStation] [nvarchar](20) NULL,
	[SignMan] [nvarchar](20) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_CargoStatusTrack] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
