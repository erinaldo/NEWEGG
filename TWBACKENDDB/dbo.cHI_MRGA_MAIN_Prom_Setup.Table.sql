USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cHI_MRGA_MAIN_Prom_Setup]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cHI_MRGA_MAIN_Prom_Setup](
	[PreID] [varchar](10) NOT NULL,
	[HiProductID] [varchar](8) NULL,
	[PromoCode] [varchar](8) NULL,
	[PromoStartDate] [varchar](8) NOT NULL,
	[PromoEndDate] [varchar](8) NULL,
	[PromoPrice] [decimal](10, 0) NULL,
	[MarketPrice] [decimal](10, 0) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_cHI_MRGA_MAIN_Prom_Setup] PRIMARY KEY CLUSTERED 
(
	[PreID] ASC,
	[PromoStartDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[cHI_MRGA_MAIN_Prom_Setup] ADD  CONSTRAINT [DF_cHI_MRGA_MAIN_Prom_Setup_Indate]  DEFAULT (getdate()) FOR [InDate]
GO
ALTER TABLE [dbo].[cHI_MRGA_MAIN_Prom_Setup] ADD  CONSTRAINT [DF_cHI_MRGA_MAIN_Prom_Setup_Updatedate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
