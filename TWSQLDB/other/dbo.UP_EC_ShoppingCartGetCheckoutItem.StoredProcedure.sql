USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_ShoppingCartGetCheckoutItem]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[UP_EC_ShoppingCartGetCheckoutItem]
	@accunt_id int , --(使用者的 accunt_id)
	@item_ids nvarchar(4000)
as
Begin
	-- 查詢每個欄位的定義

	-- track_status		主件商品追蹤的狀態, 0表示本次購買, 1表示放入下次買
	-- track_createdate		一起結帳建立的時間
	-- trackitem_status	配件追蹤的狀態 0表示該用戶有追蹤, null 表示該項目沒有追蹤 , 0在UI 配件勾選打勾.
	-- 主商品資訊
	-- item_id				商品編號
	-- item_name			商品名稱
	-- item_layout			主商品賣場類型 1(PcDiy),4(N選M) 兩種賣場皆不可再選配件, 0表示主商品具有多屬性, 1表示一般賣場商品
	-- item_sellingQty				主商品庫存數量
	-- itemlist_price		配件商品價格

	-- 配件商品資訊
	-- itemlist_id			配件商品編號, NULL表示該項目為主件商品, 沒有配件, 但有配件id, 在將來UI呈現的時候, 要補上主件商品
	-- itemlist_name			配件商品名稱
	-- itemlistgroup_type		配件群組類型 0表示一般,  10表示屬性 ,  20表示贈品
	-- itemlist_sellingQty			配件商品庫存數量

declare @tmp_tracks_active table (
	TrackAccID int,
	TrackStatus int,
	TrackCreateDate datetime,
	TrackItemStatus int,
	ItemID int,
	ItemProductID int,
	ItemName nvarchar(200),
	ItemPriceCash decimal(10, 2),
	ItemLayout int,
	ItemPhotoName nvarchar(50),
	ItemListID int,
	ItemListItemListID int,
	ItemListName nvarchar(200),
	ItemListPrice decimal(10, 2),
	ItemListGroupType int,
	ItemSaleType int,
	ItemPayType int,
	ItemDelvType int,
	ItemDelvDate nvarchar(50),
	ItemDateStart datetime,
	ItemDateEnd datetime,
	ItemDateDel datetime,
	ItemClass int,
	ItemlistItemlistProductID int,
	ItemListOrder int,
	ItemShowOrder int,
	ItemListGroupOrder int,
	ItemSellingQty int,
	ItemSellerID int,
	ItemProductLength decimal(10, 2),
	ItemProductWidth decimal(10, 2),
	ItemProductHeight decimal(10, 2),
	ItemProductWeight decimal(10, 2),
	ItemProductTradeTax decimal(10, 2),
	ItemListSellingQty int,
	ItemListSellerID int,
	ItemListProductLength decimal(10, 2),
	ItemListProductWidth decimal(10, 2),
	ItemListProductHeight decimal(10, 2),
	ItemListProductWeight decimal(10, 2),
	ItemListProductTradeTax decimal(10, 2));

insert @tmp_tracks_active
select *
	FROM dbo.ViewTracksActive  with (nolock)
	WHERE AccID=@accunt_id;



declare @tmp_buy_active_items table(
ItemID int)

insert into @tmp_buy_active_items
select A.Num FROM dbo.fn_EC_DataList('ItemID',@item_ids) A;


delete @tmp_tracks_active where ItemID not in (select ItemID from @tmp_buy_active_items)


declare @checkattribsnum table(
ItemID int,
AttribNum int)

declare @checkattribdnum table(
ItemID int,
AttribNum int)
 
insert into  @checkattribdnum
SELECT ItemID,count(ItemListID) as ItemListNumD
					FROM @tmp_tracks_active C
					WHERE  
						(
								ItemListGroupType=10 and ItemListID=0 and ItemlistSellingQty<=0 --主件屬性只要查到有一個有量就可以
							) 
							group by ItemID; 

insert into  @checkattribsnum		
SELECT ItemID,count(ItemListID) as ItemListNumS
					FROM @tmp_tracks_active B
					WHERE  ItemListGroupType=10 and ItemListID=0  group by ItemID;



 --三角貿易, 海外直購
	With ForeignBuyingSearchTemp as
	(
		SELECT TrackStatus,TrackCreateDate,TrackitemStatus,T.ItemID,ItemSellingQty,ItemSellerID,ItemName,ItemPriceCash,ItemLayout,ItemPhotoName,ItemProductLength,ItemProductWidth,ItemProductHeight,ItemProductWeight,ItemProductTradeTax,ItemListID,ItemListSellingQty,ItemListSellerID,ItemListItemListID,ItemListName,ItemListPrice,ItemListGroupType,ItemPayType,ItemClass,ItemShowOrder,ItemListOrder,ItemListGroupOrder,ItemListProductLength,ItemListProductWidth,ItemListProductHeight,ItemListProductWeight,ItemListProductTradeTax
		FROM @tmp_tracks_active T
		left join @checkattribsnum A  on A.ItemID=T.ItemID
		left join @checkattribdnum B  on A.ItemID=B.itemID
			WHERE T.ItemID in
			(
				SELECT top 25 ItemID
				FROM @tmp_tracks_active
				WHERE TrackStatus=0 											
					AND
					(
						(	
							ItemSaleType in (1,2,3)                          	
							AND ItemDelvType in (3, 6)                           
							AND ItemPayType=0                                    
							AND getdate() BETWEEN ItemDateStart AND ItemDateEnd   
							AND ItemSellingQty>0   
						)
						And 
						( 
							((A.AttribNum - B.AttribNum) >0)
							or
							( A.ItemID is Null)
						)
						
					)
				GROUP BY ItemID, TrackCreateDate
				ORDER BY TrackCreateDate desc
			)
			
	)
	SELECT ItemID ,ItemSellingQty,ItemSellerID,N'宅配' AS 'ItemSaleType',CONVERT(char(10),TrackCreateDate,111)+'<BR>'+LEFT(CONVERT(char(5),TrackCreateDate,114),5) AS TrackCreateDate,ItemShowOrder ,replace(ItemPhotoName,'-s200.','-s60.') AS 'ItemImage',CASE ItemLayout 
                WHEN 10 THEN CASE ItemClass 
                        WHEN 2 THEN N'主商品(書)(限制級)'
						ELSE N'主商品(書)' 
                     
                END ELSE CASE ItemClass 
                        WHEN 2 THEN N'主商品(限制級)'
							ELSE N'主商品'
						END
			END as 'ItemProductType',TrackItemStatus as 'ItemListSelectType',ItemProductLength,ItemProductWidth,ItemProductHeight,ItemProductWeight, ItemListID ,ItemListSellingQty,ItemListSellerID, ItemListItemListID ,CASE ItemListGroupType
			WHEN 0 THEN N'一般'
			WHEN 10 THEN N'屬性'
			WHEN 20 THEN N'贈品'
		ELSE N'一般'
		END as 'ItemListType',ItemListName ,ItemPriceCash ,case ItemListGroupType
			WHEN 10 THEN ItemListName
		END as 'ItemListAttribName',TrackStatus,ItemName,ItemLayout,ItemListPrice,ItemListOrder,ItemListProductLength,ItemListProductWidth,ItemlistProductHeight,ItemListProductWeight
	FROM ForeignBuyingSearchTemp with (nolock)
	ORDER BY TrackCreateDate desc, ItemID, ItemListOrder, ItemListID, ItemListItemListID, ItemListGroupOrder;



	
	--宅配

	With BuyingSearchTemp as
	(
		SELECT TrackStatus,TrackCreateDate,TrackitemStatus,T.ItemID,ItemSellingQty,ItemSellerID,ItemName,ItemPriceCash,ItemLayout,ItemPhotoName,ItemProductLength,ItemProductWidth,ItemProductHeight,ItemProductWeight,ItemProductTradeTax,ItemListID,ItemListSellingQty,ItemListSellerID,ItemListItemListID,ItemListName,ItemListPrice,ItemListGroupType,ItemPayType,ItemClass,ItemShowOrder,ItemListOrder,ItemListGroupOrder,ItemListProductLength,ItemListProductWidth,ItemListProductHeight,ItemListProductWeight,ItemListProductTradeTax
		FROM @tmp_tracks_active T
		left join @checkattribsnum A  on A.itemID=T.itemID
		left join @checkattribdnum B  on A.itemID=B.itemID
			WHERE T.itemID in
			(
				SELECT top 25 itemID
				FROM @tmp_tracks_active
				WHERE TrackStatus=0 											
					AND
					(
						(	
							ItemSaletype in (1,2,3)						
							AND ItemDelvtype in (0,1,2,4,7,8,9)                    
							AND ItemPayType=0                                
							AND getdate() BETWEEN ItemDateStart AND ItemDateEnd   
							AND ItemSellingQty>0  
						)
						And 
						( 
							((A.AttribNum - B.AttribNum) >0)
							or
							( A.ItemID is Null)
						)
						
					)
				GROUP BY ItemID, TrackCreateDate
				ORDER BY TrackCreateDate desc
			)
			
	)
	SELECT ItemID ,ItemSellingQty,ItemSellerID,N'宅配' AS 'ItemSaleType' ,CONVERT(char(10),TrackCreateDate,111)+'<BR>'+LEFT(CONVERT(char(5),TrackCreateDate,114),5) AS TrackCreateDate,ItemShowOrder ,replace(ItemPhotoName,'-s200.','-s60.') AS 'ItemImage' ,CASE ItemLayout 
                                    WHEN 10 THEN CASE ItemClass 
                                            WHEN 2 THEN N'主商品(書)(限制級)'
						ELSE N'主商品(書)' 
                                         
                                    END ELSE CASE ItemClass 
                                            WHEN 2 THEN N'主商品(限制級)'
							ELSE N'主商品'
						END
			END as 'ItemProductType',TrackitemStatus as 'ItemlistSelectType',ItemProductLength,ItemProductWidth,ItemProductHeight,ItemProductWeight, ItemListID ,ItemListSellingQty,ItemlistSellerID, ItemListItemListID ,CASE ItemListGroupType
			WHEN 0 THEN N'一般'
			WHEN 10 THEN N'屬性'
			WHEN 20 THEN N'贈品'
		ELSE N'一般'
		END as 'ItemListType',ItemListName ,ItemPriceCash ,case ItemListGroupType
			WHEN 10 THEN ItemListName
		END as 'ItemListAttribName',TrackStatus,ItemName,ItemLayout,ItemListPrice,ItemListOrder,ItemListProductLength,ItemListProductWidth,ItemListProductHeight,ItemListProductWeight
	FROM BuyingSearchTemp with (nolock)
	ORDER BY TrackCreateDate desc, ItemID, ItemListOrder, ItemListID, ItemListItemListID, ItemListGroupOrder;

	
	
	
END







GO
