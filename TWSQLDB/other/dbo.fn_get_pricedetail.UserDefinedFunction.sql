USE [TWSQLDB]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_get_pricedetail]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_get_pricedetail] (@item_price int,@item_tradetax decimal(10, 2))
RETURNS @tbl TABLE (sourcepriceout int ,tradetaxout int,serviceamoutout int) 
AS 
BEGIN 
		DECLARE @item_servicerate decimal(10, 2)
		DECLARE @totalrate decimal(10, 2)
		
		DECLARE @item_sourcepriceout int
		DECLARE @item_tradetaxout int
		DECLARE @item_serviceamoutout int
		
		SET @item_servicerate=0.05
		set @totalrate=(1+@item_servicerate+@item_tradetax)

		set @item_sourcepriceout=@item_price-convert(int,((@item_price/@totalrate)*@item_tradetax))-convert(int,((@item_price/@totalrate)*@item_servicerate))
		set @item_tradetaxout=convert(int,((@item_price/@totalrate)*@item_tradetax))
		set @item_serviceamoutout=convert(int,((@item_price/@totalrate)*@item_servicerate))
		INSERT @tbl
        (
             sourcepriceout
            ,tradetaxout
            ,serviceamoutout
        ) 
        VALUES
        (
             @item_sourcepriceout
            ,@item_tradetaxout
            ,@item_serviceamoutout
        )
RETURN 
END

GO
