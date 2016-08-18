USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_ShoppingCartGetNum]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[UP_EC_ShoppingCartGetNum] 
	@accunt_id int 

as
BEGIN


Declare @foreignbuyitemsum int;
Declare @foreignreservingsum int;
Declare @foreigntrackingsum int;

Declare @buyitemsum int;
Declare @reservingsum int;
Declare @trackingsum int;



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
	FROM ViewTracksActive  with (nolock)
	WHERE AccID=@accunt_id;



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
		SELECT distinct T.ItemID as buyscount
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
							AND ItemDelvType in (3,6)
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
	select @foreignbuyitemsum=count(buyscount) from ForeignBuyingSearchTemp with (nolock);

	--海外下次買
	With ForeignReservingSearchTemp as 
	(
		SELECT distinct T.ItemID as reservingCount
		FROM @tmp_tracks_active T
		left join @checkattribsnum A  on A.ItemID=T.ItemID
		left join @checkattribdnum B  on A.ItemID=B.ItemID
		WHERE T.ItemID in
			(
				SELECT top 25 ItemID
				FROM @tmp_tracks_active
				WHERE TrackStatus=1 											
					AND
					(
						(
							ItemSaleType in (1,2,3)                          	
							AND ItemDelvType in (3,6)            	
							AND getdate() BETWEEN ItemDateStart AND ItemDateEnd   
							AND ItemSellingQty>0  		
						)
						And 
						( 
							(( A.AttribNum - B.AttribNum) >0)
							or
							( A.ItemID is Null)
						)
						
					)
				GROUP BY ItemID, TrackCreateDate
				ORDER BY TrackCreateDate desc
			)
			
			
	)
	
	select @foreignreservingsum=count(reservingCount) from ForeignReservingSearchTemp with (nolock);

	--海外追蹤
	With ForeignTrackingSearchTemp as 
	(
		SELECT distinct T.ItemID as trackingcount
        FROM @tmp_tracks_active T 
        LEFT JOIN @checkattribsnum A  
            ON A.ItemID=T.ItemID 
        LEFT JOIN @checkattribdnum B  
            ON A.ItemID=B.ItemID WHERE  (	 T.ItemID IN ( 
                SELECT TOP 25 
                    ItemID 
                FROM @tmp_tracks_active WHERE (  ItemSaleType IN (1,2,3)	 AND ItemDelvType in (3,6)          	 AND GETDATE()<ItemDateDel                      	 AND ( GETDATE()<ItemDateStart OR GETDATE()>ItemDateEnd OR  ItemSellingQty <=0  						 )			               )OR ( (ItemDelvType in (3,6))            	 AND (A.AttribNum - B.AttribNum) =0 )   
                GROUP BY ItemID, TrackCreateDate 
                ORDER BY TrackCreateDate DESC 
             )  )   
     )
	select @foreigntrackingsum=count(trackingcount) from ForeignTrackingSearchTemp with (nolock) ;

	--宅配

	With BuyingSearchTemp as
	(
		SELECT distinct T.ItemID as buyscount
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
	select @buyitemsum=count(buyscount) from BuyingSearchTemp with (nolock);
	
	
	--下次買
	With ReservingSearchTemp as 
	(
		SELECT distinct T.ItemID as reservingCount
		FROM @tmp_tracks_active T
		left join @checkattribsnum A  on A.ItemID=T.ItemID
		left join @checkattribdnum B  on A.ItemID=B.ItemID
		WHERE T.ItemID in
			(
				SELECT top 25 ItemID
				FROM @tmp_tracks_active
				WHERE TrackStatus=1 											
					AND
					(
						(
							ItemSaleType in (1,2,3)                          	
							AND ItemDelvtype in (0,1,2,4,7,8,9)                            	
							AND getdate() BETWEEN ItemDateStart AND ItemDateEnd   
							AND ItemSellingQty>0  		
						)
						And 
						( 
							(( A.AttribNum - B.AttribNum) >0)
							or
							( A.ItemID is Null)
						)
						
					)
				GROUP BY ItemID, TrackCreateDate
				ORDER BY TrackCreateDate desc
			)
			
			
	)
	select @reservingsum=count(reservingCount) from ReservingSearchTemp with (nolock);

	--追蹤 
	WITH TrackingSearchTemp AS 
                    ( 
                        SELECT distinct T.ItemID as trackingcount
                        FROM @tmp_tracks_active T 
                        LEFT JOIN @checkattribsnum A  
                            ON A.ItemID=T.ItemID 
                        LEFT JOIN @checkattribdnum B  
                            ON A.ItemID=B.ItemID WHERE  (	 T.ItemID IN ( 
                                SELECT TOP 25 
                                    ItemID 
                                FROM @tmp_tracks_active WHERE (  ItemSaleType IN (1,2,3)	 AND ItemDelvType IN (0,1,2,4,7,8,9)            	 AND GETDATE()<ItemDateDel                   	 AND ( GETDATE()<ItemDateStart OR GETDATE()>ItemDateEnd OR  ItemSellingQty <=0   								 )			               )OR ( (ItemDelvType IN (0,1,2,4))          	 AND (A.AttribNum - B.AttribNum) =0 )   
                                GROUP BY ItemID, TrackCreateDate 
                                ORDER BY TrackCreateDate DESC 
                             )  )   
                    ) 
select @trackingsum=count(trackingcount) from TrackingSearchTemp with (nolock) ;
	
	select @foreignbuyitemsum as foreignbuysum,@foreignreservingsum as foreignreservesum,@foreigntrackingsum as foreigntracksum, @buyitemsum as buysum,@reservingsum as reservesum,@trackingsum as tracksum

END





GO
