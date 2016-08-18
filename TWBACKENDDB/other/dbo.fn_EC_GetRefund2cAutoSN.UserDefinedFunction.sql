USE [TWBACKENDDB]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_EC_GetRefund2cAutoSN]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE FUNCTION [dbo].[fn_EC_GetRefund2cAutoSN]        -- function name                
(@type varchar(4))                    -- input parameter name and data type
RETURNS char(15)                          -- return parameter data type
AS
BEGIN                                -- begin body definition
--SO07072011249
DECLARE @YYMMDD char(6)
       ,@ano char(11)
       ,@sno char(15)
       ,@num int    
DECLARE @prefix_len int
DECLARE @resultvalue char(10)    /*  取得年月   */ 
IF datepart(month,getdate())<10 
	begin
		if datepart(day,getdate())<10 
			SELECT 
                @YYMMDD=RIGHT(str(datepart(YEAR,GETDATE()),4),2)+'0'+str(datepart(month,getdate()),1)+'0'+str(datepart(DAY,GETDATE()),1)
		ELSE
			SELECT 
                @YYMMDD=RIGHT(str(datepart(YEAR,GETDATE()),4),2)+'0'+str(datepart(month,getdate()),1)+str(datepart(day,getdate()),2)
	end
else 
   	begin 
	   	if datepart(day,getdate())<10 
			select @YYMMDD=right(str(datepart(year,getdate()),4),2)+str(datepart(month,getdate()),2)+'0'+str(datepart(DAY,GETDATE()),1)
		ELSE
			SELECT 
                @YYMMDD=RIGHT(str(datepart(YEAR,GETDATE()),4),2)+str(datepart(MONTH,GETDATE()),2)+str(datepart(DAY,GETDATE()),2)
	END
/* 取得資料表之編號  */ 

set @prefix_len=len(@type)
SELECT 
    @ano=(
        SELECT 
            MAX(CONVERT(int,RIGHT(Code,9-@prefix_len)))  
        FROM [TWBACKENDDB].[dbo].[refund2c] 
        WHERE 
            SUBSTRING(Code,@prefix_len+1,6)=@YYMMDD 
            AND SUBSTRING(Code,1,@prefix_len)=@type
     )   /* 給定編號*/ 
IF @ano  is null 
  begin 
	SELECT 
        @resultvalue=dbo.fn_EC_PadLeft('0', 9-@prefix_len, 1)
	set @sno= @type+rtrim(@YYMMDD)+@resultvalue 
  end 
else 
  begin 
	set @num=convert(int,max(@ano))+1
	SELECT @resultvalue=dbo.fn_EC_PadLeft('0'
       ,9-@prefix_len
       ,@num
 )
	set @sno=@type+rtrim(@YYMMDD)+@resultvalue

  end 
RETURN  @sno
END;









GO
