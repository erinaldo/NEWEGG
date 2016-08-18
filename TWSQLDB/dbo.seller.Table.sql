USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[seller]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[seller](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[EngName] [nvarchar](50) NULL,
	[Description] [nvarchar](150) NULL,
	[CountryID] [int] NULL,
	[CurrencyType] [char](10) NOT NULL,
	[TableName] [varchar](150) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_seller_seller_createdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_seller_seller_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[AccountType] [char](1) NULL,
	[CompanyCode] [char](10) NULL,
	[ACCT_GROUP] [nvarchar](4) NULL,
	[PUR_ORG] [nvarchar](4) NULL,
	[Email] [nvarchar](256) NULL,
	[City] [nvarchar](35) NULL,
	[PostCode] [nvarchar](10) NULL,
	[Region] [nvarchar](3) NULL,
	[Sortl] [nvarchar](25) NULL,
	[Address] [nvarchar](150) NULL,
	[TELF1] [nvarchar](16) NULL,
	[TELF2] [nvarchar](16) NULL,
	[TELFX] [nvarchar](31) NULL,
	[LANGUAGE] [char](1) NULL,
	[VAT_NO] [nvarchar](20) NULL,
	[ZTERM] [nvarchar](4) NULL,
	[RECON_ACCT] [nvarchar](10) NULL,
	[ACTION_CODE] [nvarchar](1) NULL,
	[Istosap] [int] NULL,
	[AboutInfo] [nvarchar](4000) NULL,
	[FirstName] [nvarchar](30) NULL,
	[LastName] [nvarchar](30) NULL,
	[State] [nvarchar](20) NULL,
	[ComAdd] [nvarchar](150) NULL,
	[Status] [char](1) NULL,
	[Identy] [int] NULL,
	[BillingCycle] [int] NULL,
 CONSTRAINT [PK_seller_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
