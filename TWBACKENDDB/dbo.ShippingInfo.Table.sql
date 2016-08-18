USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[ShippingInfo]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[ShippingInfo](
	[Index] [int] IDENTITY(1,1) NOT NULL,
	[CWBNumber] [varchar](11) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_ShippingInfo_CreateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_ShippingInfo_UpdateDate]  DEFAULT (getdate()),
	[Status] [int] NULL,
 CONSTRAINT [PK_dbo.ShippingInfo] PRIMARY KEY CLUSTERED 
(
	[Index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
