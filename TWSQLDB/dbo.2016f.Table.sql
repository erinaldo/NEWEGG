USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[2016f]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[2016f](
	[貨品分類號列] [char](11) NULL,
	[第一欄稅率] [nvarchar](50) NULL,
	[第二欄稅率] [nvarchar](50) NULL,
	[第三欄稅率] [nvarchar](50) NULL,
	[統計數量單位] [nvarchar](20) NULL,
	[統計重量單位] [nvarchar](20) NULL,
	[稽徵規定] [nvarchar](50) NULL,
	[輸入規定] [nvarchar](50) NULL,
	[輸出規定] [nvarchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
