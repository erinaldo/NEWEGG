USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[JieMaiOrderInfo]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JieMaiOrderInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SalesOrderCode] [nvarchar](20) NOT NULL,
	[UploadFlag] [int] NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[OrderID] [nvarchar](20) NULL,
	[JMOrderID] [nvarchar](20) NULL,
	[TotalFee] [decimal](10, 2) NULL,
	[CartStatus] [int] NULL,
	[CartCreateDate] [datetime] NULL,
 CONSTRAINT [PK_JieMaiUpload] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
