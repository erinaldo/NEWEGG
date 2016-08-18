USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cHI_MRGA_MAIN]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cHI_MRGA_MAIN](
	[PreID] [varchar](10) NOT NULL,
	[PreStartDate] [varchar](8) NULL,
	[PreEndDate] [varchar](8) NULL,
	[HiProductID] [varchar](8) NULL,
	[ProductName] [nvarchar](20) NULL,
	[PromoCode] [varchar](8) NULL,
	[PromoStartDate] [varchar](8) NULL,
	[PromoEndDate] [varchar](8) NULL,
	[PromoPrice] [decimal](10, 0) NULL,
	[MarketPrice] [decimal](10, 0) NULL,
	[PublicFee] [decimal](9, 2) NULL,
	[TakeFee] [decimal](9, 2) NULL,
	[Stock] [decimal](10, 0) NULL,
	[Is24] [char](1) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_cHI_MRGA_MAIN] PRIMARY KEY CLUSTERED 
(
	[PreID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[cHI_MRGA_MAIN] ADD  CONSTRAINT [DF_cHI_MRGA_MAIN_Indate]  DEFAULT (getdate()) FOR [InDate]
GO
ALTER TABLE [dbo].[cHI_MRGA_MAIN] ADD  CONSTRAINT [DF_cHI_MRGA_MAIN_Updatedate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
