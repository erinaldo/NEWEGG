USE [TWBACKENDDB]
GO
/****** Object:  User [TWSQLReadOnly]    Script Date: 2016/08/18 13:17:12 ******/
CREATE USER [TWSQLReadOnly] FOR LOGIN [TWSQLReadOnly] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [TWSQLReadOnly]
GO
