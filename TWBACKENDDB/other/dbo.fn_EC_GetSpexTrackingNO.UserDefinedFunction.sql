USE [TWBACKENDDB]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_EC_GetSpexTrackingNO]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fn_EC_GetSpexTrackingNO]()
RETURNS char(11)
AS
/**************************************************
SPEX TRACKING NO. 編碼規則:

ex.63614000001

編碼說明:
636		(USA-TWN)
14		(年分)
000001	(序號)
**************************************************/
BEGIN
	DECLARE @CODE char (3),
			@YY char(2),
			@CurrentNo char(11),
			@NextNo char(11)
			
	SET @CODE = '636' --固定碼
	SET @YY = CONVERT(char(2),DATEPART(YEAR,GETDATE())%100) --末兩碼


	/* 取得資料表之編號  */ 
	SELECT @CurrentNo = MAX(CWBNumber)
	FROM ShippingInfo
	WHERE LEFT(CWBNumber,5) = (@CODE + @YY)
	IF @CurrentNo IS NULL
		BEGIN 
			SET @NextNo = @CODE + @YY + '000001'
		END
	ELSE 
		BEGIN
			SET @NextNo = @CODE + @YY + RIGHT('000000' + CONVERT(VARCHAR(6),CONVERT(INT,RIGHT(@CurrentNo,6)) + 1),6)
		END 
	RETURN  @NextNo
END

GO
