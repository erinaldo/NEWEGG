USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[tempTable]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tempTable](
	[spexedit_id] [int] IDENTITY(1,1) NOT NULL,
	[spexedit_so] [varchar](30) NULL,
	[spexedit_sales_dep] [char](5) NOT NULL,
	[spexedit_account_number] [varchar](11) NOT NULL,
	[spexedit_simple_name] [varchar](20) NOT NULL,
	[spexedit_sales] [varchar](16) NULL,
	[spexedit_company_name_sent] [varchar](200) NOT NULL,
	[spexedit_contact_person_sent] [varchar](200) NULL,
	[spexedit_address1_sent] [varchar](150) NOT NULL,
	[spexedit_address2_sent] [varchar](150) NULL,
	[spexedit_state] [char](2) NOT NULL,
	[spexedit_zip_code_sent] [char](10) NOT NULL,
	[spexedit_city_name_sent] [varchar](100) NOT NULL,
	[spexedit_phone_no_sent] [varchar](30) NULL,
	[spexedit_company_name_receive] [nvarchar](150) NOT NULL,
	[spexedit_contact_person_receive] [varchar](50) NULL,
	[spexedit_address_1_receive] [nvarchar](150) NOT NULL,
	[spexedit_address_2_receive] [varchar](150) NULL,
	[spexedit_consignee_state] [char](10) NOT NULL,
	[spexedit_zip_code_receive] [char](5) NULL,
	[spexedit_country_name] [varchar](100) NOT NULL,
	[spexedit_city_name_receive] [nvarchar](100) NULL,
	[spexedit_phone_no_receive] [varchar](30) NOT NULL,
	[spexedit_pieces] [int] NULL,
	[spexedit_package_type] [varchar](20) NULL,
	[spexedit_dim_weight] [decimal](10, 2) NULL,
	[spexedit_gross_weight] [decimal](10, 2) NULL,
	[spexedit_invoice_value] [int] NULL,
	[spexedit_description] [nvarchar](200) NULL,
	[spexedit_fuel_surcharge] [decimal](10, 2) NULL,
	[spexedit_flag] [int] NOT NULL,
	[spexedit_filename] [varchar](50) NULL,
	[spexedit_delivtype] [int] NULL,
	[spexedit_createdate] [datetime] NULL,
	[spexedit_no] [int] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
