USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GetItemTaxDetail]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE PROCEDURE [dbo].[UP_EC_GetItemTaxDetail] 
      @itemid nvarchar(2000) 
     ,@itemlistid nvarchar(2000) 
     ,@shippingpricelist nvarchar(2000) 
AS 
BEGIN

--@item_pricesell=賣價 from item join 
--@item_pricecost=美蛋成本價格 from product join
--@item_shippingprice=分攤的運費價格 from input
--@item_tradetaxrate=關稅稅率 from join product
--@item_producttaxrate=貨物稅 from join product

DECLARE @tbl TABLE 
( 
     item_id int 
    ,itemlist_id int 
    ,pricetaxdetail nvarchar(100) 
) 

DECLARE @productvalue decimal(10, 2)
DECLARE @tradetax decimal(10, 5)
DECLARE @producttax decimal(10, 5)
DECLARE @runtax decimal(10, 2)
DECLARE @tradepromotax decimal(10, 2)

DECLARE @totaltax decimal(10, 2)
DECLARE @serviceprice decimal(10, 2)
DECLARE @item_sourcepriceout int

DECLARE @cursoritemid int
DECLARE @cursoritemdelvtype int
DECLARE @cursoritemprice int
DECLARE @cursoritemserviceprice int
DECLARE @cursoritemcountryid int
DECLARE @cursoritemcost decimal(10, 2)
DECLARE @cursoritemaverageexchangerate decimal(10,4)
DECLARE @cursoritemlistaverageexchangerate decimal(10,4)
DECLARE @cursoritemtradetax decimal(10, 5)
DECLARE @cursoritemlistid int
DECLARE @cursoritemlistprice int
DECLARE @cursoritemlistserviceprice int
DECLARE @cursoritemlistcountryid int
DECLARE @cursoritemlistcost decimal(10, 2)
DECLARE @cursoritemlisttradetax decimal(10, 5)
DECLARE @cursoritemshippingprice int
DECLARE @cursoritemproducttax decimal(10, 5)
DECLARE @cursoritemlistproducttax decimal(10, 5)



DECLARE mycursor CURSOR 
FOR 
SELECT 
    ISNULL(R1.ItemID,0) 
   ,R1.DelvType AS item_delvtype 
   ,R1.PriceCash AS item_price 
   ,R1.Cost  AS item_cost 
   ,R1.AverageExchangerate AS currency_averageexchangerate 
   ,R1.ServicePrice AS item_serviceprice 
   ,R1.CountryID AS item_countryid 
   ,R1.TradeTax AS item_tradetax 
   ,ISNULL(R1.Tax,0) AS item_producttax 
   ,ISNULL(R4.ItemListID,0) 
   ,R4.Price 
   ,R4.Cost AS itemlistcost 
   ,R4.AverageExchangerate AS itemlist_averageexchangerate 
   ,R4.Serviceprice AS itemlist_serviceprice 
   ,R4.CountryID AS itemlist_countryid 
   ,R4.TradeTax AS itemlist_tradetax 
   ,ISNULL(R4.Tax,0) AS itemlist_producttax 
   ,R3.num AS shippingprice 
FROM dbo.fn_EC_DataList('item_id',@itemid) A
left join (
select A1.ID as ItemID,A1.DelvType,A1.PriceCash,A1.ServicePrice,A2.Cost,A2.TradeTax,A2.Tax,C2.AverageexChangerate,C1.ID as CountryID FROM item A1 with (nolock) 
inner join Product A2 with (nolock) on A1.ProductID=A2.ID  
inner join seller S1 with (nolock) on A2.SellerID=S1.ID
inner join country C1 with (nolock) on S1.CountryID=C1.ID
inner join currency C2 with (nolock) on C2.CountryID=C1.ID and C2.[Year]=datepart("yyyy",getdate()) and [Month]=datepart("MM",getdate())
) R1 on A.num=R1.ItemID
left join (
select C1.sn,C1.name,C1.num FROM dbo.fn_EC_DataList('itemlist_id',@itemlistid 
) C1 
) R2 on A.sn=R2.sn
left join (
SELECT 
    E1.ID as ItemListID
   ,E1.Price 
   ,E1.ServicePrice 
   ,A3.Cost 
   ,A3.TradeTax 
   ,A3.Tax 
   ,C4.AverageExchangerate 
   ,C3.ID as CountryID 
FROM itemlist  E1  WITH (NOLOCK) 
INNER JOIN product A3 WITH (NOLOCK) 
    ON E1.ItemlistProductid=A3.ID 
INNER JOIN seller S2 WITH (NOLOCK) 
    ON A3.SellerID=S2.ID 
INNER JOIN country C3 WITH (NOLOCK) 
    ON S2.CountryID=C3.ID 
INNER JOIN currency C4 WITH (NOLOCK) 
    ON C4.CountryID=C3.ID 
    AND C4.[Year]=datepart("yyyy",GETDATE())  
    AND [Month]=datepart("MM",GETDATE()) ) R4 
    ON R2.num=R4.ItemListID 
LEFT JOIN ( 
    SELECT 
        C2.sn 
       ,C2.name 
       ,C2.num 
    FROM dbo.fn_EC_DataList('shippingprice',@shippingpricelist) C2 
) R3 on A.sn=R3.sn

OPEN mycursor


FETCH NEXT FROM mycursor INTO @cursoritemid,@cursoritemdelvtype,@cursoritemprice,@cursoritemcost,@cursoritemaverageexchangerate,@cursoritemserviceprice,@cursoritemcountryid,@cursoritemtradetax,@cursoritemproducttax,@cursoritemlistid,@cursoritemlistprice,@cursoritemlistcost,@cursoritemlistaverageexchangerate,@cursoritemlistserviceprice,@cursoritemlistcountryid,@cursoritemlisttradetax,@cursoritemlistproducttax,@cursoritemshippingprice
WHILE (@@FETCH_STATUS = 0)
BEGIN
		if(@cursoritemid>0)
		begin
				--完稅價格=離岸價格(FOB)＋運費(FREIGHT)＋保險費(INSURANCE) 
				set @productvalue=(Round((@cursoritemcost*@cursoritemaverageexchangerate),0)+@cursoritemshippingprice)
					
				--進口稅=完稅價格×進口稅率
				set @tradetax=Round(@productvalue*@cursoritemtradetax,0)
				
				--貨物稅=(完稅價格＋進口稅) ×貨物稅率
				--set @producttax=(@productvalue+@tradetax)*@item_producttaxrate
				set @producttax=Round((@productvalue+@tradetax)*@cursoritemproducttax,0)
				
				
				--營業稅=(完稅價格＋進口稅＋貨物稅) × 營業稅率(5%)
				set @runtax=Round((@productvalue+@tradetax+@producttax)*0.05,0)
				
				
				--推廣貿易服務費=完稅價格× 推廣貿易服務費率(0.0415%)
				set @tradepromotax=Round(@productvalue*0.0004,0)

				if(@cursoritemdelvtype=0)
				begin
					insert @tbl (item_id,itemlist_id,pricetaxdetail) values(@cursoritemid,@cursoritemlistid,'0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00'
 )
				END
				else
				begin

					if(@cursoritemprice=0)
					begin

						set @item_sourcepriceout=@cursoritemprice
											
						if(@cursoritemcountryid=1)
							set @totaltax=0
						else
							set @totaltax=convert(int,@tradetax+@producttax+@runtax+@tradepromotax)
							
						set @serviceprice=0-@totaltax
						
					end
					else
					begin

						set @serviceprice=@cursoritemserviceprice

						if(@cursoritemcountryid=1)
							set @totaltax=0
						else
							set @totaltax=convert(int,@tradetax+@producttax+@runtax+@tradepromotax)

						set @item_sourcepriceout=@cursoritemprice-@serviceprice-@totaltax
					end
					INSERT @tbl 
                    (
                         item_id
                        ,itemlist_id
                        ,pricetaxdetail
                    ) 
                    VALUES
                    (
                         @cursoritemid
                        ,@cursoritemlistid
                        ,CONVERT(nvarchar(20),@cursoritemcost)+',' + CONVERT(nvarchar(20),Round(@cursoritemcost*@cursoritemaverageexchangerate,0))+',' + CONVERT(nvarchar(20),@totaltax) +','+CONVERT(nvarchar(20),@serviceprice) +','+CONVERT(nvarchar(20),@tradetax)  +','+CONVERT(nvarchar(20),@runtax) +','+CONVERT(nvarchar(20),@producttax)   +','+CONVERT(nvarchar(20),@tradepromotax)
                    )
				end
		end
		else
		begin
			if(@cursoritemid=0 and @cursoritemlistid>0)
			begin
				--完稅價格=離岸價格(FOB)＋運費(FREIGHT)＋保險費(INSURANCE) 
					set @productvalue=Round((@cursoritemlistcost+@cursoritemshippingprice),0)
					
					--進口稅=完稅價格×進口稅率
					set @tradetax=Round(@productvalue*@cursoritemlisttradetax,0)
					
					--貨物稅=(完稅價格＋進口稅) ×貨物稅率
					set @producttax=Round((@productvalue+@tradetax)*@cursoritemlistproducttax,0)
					
					--營業稅=(完稅價格＋進口稅＋貨物稅) × 營業稅率(5%)
					set @runtax=Round((@productvalue+@tradetax+@producttax)*0.05,0)
					
					--推廣貿易服務費=完稅價格× 推廣貿易服務費率(0.0415%)
					set @tradepromotax=Round(@productvalue*0.0004,0)
					
					if(@cursoritemdelvtype=0)
					begin
						INSERT @tbl 
                        (
                             item_id
                            ,itemlist_id
                            ,pricetaxdetail
                        ) 
                        VALUES
                        (
                             @cursoritemid
                            ,@cursoritemlistid,'0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00')
					end
					else
					begin
						if(@cursoritemlistprice=0)
						begin
							set @item_sourcepriceout=@cursoritemlistprice
							if(@cursoritemlistcountryid=1)
								set @totaltax=0
							else
								set @totaltax=convert(int,@tradetax+@producttax+@runtax+@tradepromotax)
								
							set @serviceprice=0-@totaltax
						end
						else
						begin
							set @serviceprice=@cursoritemlistserviceprice
							if(@cursoritemlistcountryid=1)
								set @totaltax=0
							else
								set @totaltax=convert(int,@tradetax+@producttax+@runtax+@tradepromotax)
								
								
							set @item_sourcepriceout=@cursoritemlistprice-@serviceprice-@totaltax
						end
						insert @tbl (item_id,itemlist_id,pricetaxdetail) values(@cursoritemid,@cursoritemlistid,convert(nvarchar(20),@cursoritemlistcost)+',' + convert(nvarchar(20),Round(@cursoritemlistcost*@cursoritemlistaverageexchangerate,0))+',' + convert(nvarchar(20),@totaltax) +','+convert(nvarchar(20),@serviceprice) +','+convert(nvarchar(20),@tradetax)  +','+convert(nvarchar(20),@runtax)  +','+convert(nvarchar(20),@producttax)  +','+convert(nvarchar(20),@tradepromotax))
					end
			end
			else 
			begin		
			
				insert @tbl (item_id,itemlist_id,pricetaxdetail) values(@cursoritemid,@cursoritemlistid,convert(nvarchar(20),@cursoritemcost)+',' + convert(nvarchar(20),Round(@cursoritemcost*@cursoritemaverageexchangerate,0))+',' + convert(nvarchar(20),@totaltax) +','+convert(nvarchar(20),@serviceprice) +','+convert(nvarchar(20),@tradetax)   +','+convert(nvarchar(20),@runtax)  +','+convert(nvarchar(20),@producttax) +','+convert(nvarchar(20),@tradepromotax))
			end
		end
		
	FETCH NEXT FROM mycursor INTO @cursoritemid,@cursoritemdelvtype,@cursoritemprice,@cursoritemcost,@cursoritemaverageexchangerate,@cursoritemserviceprice,@cursoritemcountryid,@cursoritemtradetax,@cursoritemproducttax,@cursoritemlistid,@cursoritemlistprice,@cursoritemlistcost,@cursoritemlistaverageexchangerate,@cursoritemlistserviceprice,@cursoritemlistcountryid,@cursoritemlisttradetax,@cursoritemlistproducttax,@cursoritemshippingprice

END

CLOSE mycursor
DEALLOCATE mycursor
select * from @tbl
End

















GO
