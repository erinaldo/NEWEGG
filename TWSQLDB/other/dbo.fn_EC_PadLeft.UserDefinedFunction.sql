USE [TWSQLDB]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_EC_PadLeft]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



Create FUNCTION [dbo].[fn_EC_PadLeft]
(
@PadChar char(1), 
@PadToLen int, 
@BaseString varchar(100)
)
RETURNS varchar(1000)
AS
BEGIN

DECLARE @Padded varchar(1000) 
DECLARE @BaseLen int 
SET @BaseLen = datalength(@BaseString) 
IF @BaseLen >= @PadToLen 
BEGIN 
SET @Padded = @BaseString 
END 
ELSE 
BEGIN 
SET @Padded = REPLICATE(@PadChar, @PadToLen - @BaseLen) + @BaseString 
END 
RETURN @Padded

END


GO
