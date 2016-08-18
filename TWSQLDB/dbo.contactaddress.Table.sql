USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[contactaddress]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[contactaddress](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[SellerID] [int] NOT NULL,
	[PrimaryCode] [char](1) NOT NULL,
	[ContactTypeID] [int] NOT NULL,
	[FirstName] [nvarchar](30) NOT NULL,
	[LastName] [nvarchar](30) NOT NULL,
	[PhoneRegion] [varchar](10) NULL,
	[Phone] [varchar](300) NULL,
	[PhoneExt] [varchar](10) NULL,
	[Email] [varchar](256) NULL,
	[Address] [nvarchar](200) NULL,
	[City] [nvarchar](50) NULL,
	[State] [nvarchar](50) NULL,
	[CountryCode] [int] NULL,
	[ZipCode] [varchar](10) NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [varchar](10) NULL,
	[InDate] [datetime] NOT NULL,
	[InUser] [varchar](10) NOT NULL,
 CONSTRAINT [PK_ContactAddress] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
