USE [TWSQLDB]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_EC_GetSellingQty]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fn_EC_GetSellingQty] (@item_qty int,@item_qtyreg int,@itemstock_qty int,@itemstock_qtyreg int,@item_qtylimit int)
RETURNS int
AS 
BEGIN 
DECLARE @opt int
DECLARE @itemqtyamount int
DECLARE @itemstockqtyamount int
DECLARE @refitemqtyamount int
SET @itemqtyamount=@item_qty-@item_qtyreg
set @itemstockqtyamount=@itemstock_qty-@itemstock_qtyreg

		if (@itemstock_qty IS NOT NULL)  -- 有找到itemstock
		begin
				if (@item_qty > 0)  -- item_qty有數量,先看賣場數量再看到itemstock
				begin
						if ( @itemqtyamount > @itemstockqtyamount )  -- 賣場qty與itemstock qty取其小
						begin
								if ( @itemstockqtyamount > @item_qtylimit) AND (@item_qtylimit > 0) 
									set @opt= @item_qtylimit
								ELSE
									set @opt= @itemstockqtyamount
						end
						ELSE
						if ( @itemqtyamount > @item_qtylimit) AND (@item_qtylimit > 0) 
							set @opt= @item_qtylimit
						ELSE
							set @opt= @itemqtyamount
				End
				ELSE -- item_qty 為0,看到itemstock
				begin
					if ( @itemstockqtyamount > @item_qtylimit) AND (@item_qtylimit > 0) 
						set @opt= @item_qtylimit
					ELSE
						set @opt= @itemstockqtyamount
				end		
		End			
		
return @opt
END


GO
