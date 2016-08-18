USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[company]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[company](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[EngName] [nvarchar](50) NULL,
	[Description] [nvarchar](150) NULL,
	[CountryID] [int] NULL,
	[CurrencyType] [char](10) NOT NULL,
	[TableName] [varchar](150) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[AccountType] [char](1) NULL,
	[CompanyCode] [char](10) NULL,
	[ACCT_GROUP] [nvarchar](4) NULL,
	[PUR_ORG] [nvarchar](4) NULL,
	[Email] [nvarchar](241) NULL,
	[City] [nvarchar](35) NULL,
	[PostCode] [nvarchar](10) NULL,
	[Region] [nvarchar](3) NULL,
	[Sortl] [nvarchar](10) NULL,
	[Address] [nvarchar](35) NULL,
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
 CONSTRAINT [PK_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_Company_createdate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[company] ADD  CONSTRAINT [DF_Company_updated]  DEFAULT ((0)) FOR [Updated]
GO
