USE [TWBACKENDDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_InsertPurchaseOrdersBySeller]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








	CREATE PROCEDURE [dbo].[UP_EC_InsertPurchaseOrdersBySeller]
		@instype int
		,@item_id varchar(4000)
		,@purchaseorderPrefix varchar(4)
		,@purchaseorderitemPrefix varchar(4)
		,@pricesum decimal(10, 2)
		,@ordernum int
		,@PurchaseOrderItem_localcurrencyid int
		,@note nvarchar(100)
		,@item_attribid nvarchar(200)
		,@purchaseorder_telday varchar(30) 
		,@purchaseorder_invoreceiver nvarchar(50) 
		,@purchaseorder_invoid varchar(10)
		,@purchaseorder_invotitle nvarchar(50)
		,@purchaseorder_involoc nvarchar(10)  
		,@purchaseorder_invozip char(5) 
		,@purchaseorder_invoaddr nvarchar(150) 
		,@purchaseorder_name nvarchar(20) 
		,@purchaseorder_paytype int 
		,@purchaseorder_email varchar(256) 
		,@purchaseorder_delivloc nvarchar(20)  
		,@purchaseorder_delivzip char(5) 
		,@purchaseorder_delivaddr nvarchar(150) 
		,@purchaseorder_idno nvarchar(44) 
		,@purchaseorder_mobile varchar(30) 
		,@purchaseorder_accountid int 
		,@purchaseorder_recvname nvarchar(50)
		,@purchaseorder_recvmobile varchar(30)  
		,@purchaseorder_recvtelday varchar(30)
		,@purchaseorder_cardno nvarchar(64) 
		,@purchaseorder_cardtype char(10) 
		,@purchaseorder_cardbank nvarchar(50) 
		,@purchaseorder_cardexpire char(10) 
		,@purchaseorder_cardbirthday char(10) 
		,@purchaseorder_cardloc nvarchar(20) 
		,@purchaseorder_cardzip char(5) 
		,@purchaseorder_cardaddr nvarchar(150) 
		,@purchaseorder_status int
		,@purchaseorders_note nvarchar(4000) 
		,@purchaseorders_delivtype nvarchar(4000)
		,@purchaseorders_delivdata nvarchar(4000)
		,@purchaseorder_remoteip char(15) 
		,@purchaseorder_coservername nvarchar(50) 
		,@purchaseorder_servername nvarchar(50) 
		,@purchaseorder_authcode varchar(50) 
		,@purchaseorder_authdate datetime 
		,@purchaseorder_authnote nvarchar(50) 
		,@purchaseorder_updateuser nvarchar(50) 
		,@purchaseorders_itemname nvarchar(4000)
		,@purchaseorderitems_itemlistid nvarchar(4000)
		,@purchaseorderitems_qty nvarchar(4000)
		,@purchaseorderitems_note nvarchar(4000)
		,@purchaseorderitems_price nvarchar(4000)
		,@purchaseorderitems_priceinst nvarchar(4000)
		,@purchaseorderitems_pricecoupon nvarchar(4000)
		,@purchaseorderitems_redmbln nvarchar(4000)
		,@purchaseorderitems_redmtkout nvarchar(4000)
		,@purchaseorderitems_redmfdbck nvarchar(4000)
		,@purchaseorderitems_wfbln nvarchar(4000)
		,@purchaseorderitems_wftkout nvarchar(4000)
		,@purchaseorderitems_actid nvarchar(4000)
		,@purchaseorderitems_acttkout nvarchar(4000)
		,@itemlist_attribid nvarchar(4000) 
		,@purchaseordergroupext_pscartid int
		,@purchaseordergroupext_pssellerid nvarchar(4000)
		,@purchaseordergroupext_pscarrynote nvarchar(4000)
		,@purchaseordergroupext_pshasact int
		,@purchaseordergroupext_pshaspartialauth int
		,@purchaseorderitemexts_psproductid nvarchar(4000)
		,@purchaseorderitemexts_psmproductid nvarchar(4000)
		,@purchaseorderitemexts_psoriprice nvarchar(4000)
		,@purchaseorderitemexts_pssellcatid nvarchar(4000)
		,@purchaseorderitemexts_psattribname nvarchar(4000)
		,@purchaseorderitemexts_psmodelno nvarchar(4000)
		,@purchaseorderitemexts_pscost nvarchar(4000)
		,@purchaseorderitemexts_psfvf nvarchar(4000)
		,@purchaseorderitemexts_psproducttype nvarchar(4000)
        ,@purchaseorderitemexts_WareHouse int
		,@dboutput nvarchar(4000) output
	AS


	declare @salesorder_salesordergroupid int 
	     
	declare @purchaseorderitems_productid int --從DB
	declare @purchaseorderitems_productlistid int  --從DB
	declare @purchaseorderitems_name nvarchar(200) --從DB

	Declare @outnumbydate int
	Declare @purchaseorder_purchaseordergroupid int
	Declare @PurchaseOrderItem_qty int
	Declare @dbsn int
	Declare @dbitemlist_id int
	Declare @dbitem_id int
	Declare @dbitemlist_productid int
	Declare @dbitemlist_sellingQty int
	Declare @dbitem_sellingQty int
	Declare @dbitem_productid int
	Declare @dbitem_currency_id int
	Declare @dbitem_product_cost int
	Declare @dbitem_currency_averageexchangerate decimal(10, 4)
	Declare @dbitem_pricecoupon decimal(10, 2)
	Declare @dbitem_name nvarchar(200)
	Declare @dbpurchaseorder_delivtype int
	Declare @dbpurchaseorder_delivdata nvarchar(50)
	Declare @dbPurchaseOrderItem_note nvarchar(50)
	Declare @dbPurchaseOrderItem_sourcecurrencyid int
	Declare @dbPurchaseOrderItem_sourceprice decimal(10, 2)
	Declare @dbPurchaseOrderItem_currencyexchangerate decimal(10, 2)
	Declare @dbPurchaseOrderItem_localprice decimal(10, 2)
	Declare @dbPurchaseOrderItem_localpriceinst decimal(10, 2)
	Declare @dbPurchaseOrderItem_localpricecoupon decimal(10, 2)
	Declare @dbPurchaseOrderItem_redmbln int
	Declare @dbPurchaseOrderItem_redmtkout int
	Declare @dbPurchaseOrderItem_redmfdbck int
	Declare @dbPurchaseOrderItem_wfbln int
	Declare @dbPurchaseOrderItem_wftkout int
	Declare @dbPurchaseOrderItem_actid nvarchar(10)
	Declare @dbPurchaseOrderItem_acttkout int
	Declare @dbitem_canbuyqty int --可買的主商品數量
	Declare @dbitemlist_canbuyqty int --可買的配件商品數量
	Declare @dbitemlist_sellingQtyset int --設定的配件商品數量
	Declare @dbitem_attribid int --主件屬性id
	Declare @dbitem_attribname nvarchar(200) --主件屬性名稱
	Declare @dbitem_attribproductid int --主件屬性商品id
	Declare @dbpurchaseorders_note nvarchar(100) --配件屬性id
	Declare @dbitemlist_attribid int --配件屬性id
	Declare @dbitemlist_attribname nvarchar(200) --配件屬性名稱
	Declare @dbitemlist_attribproductid int --配件屬性商品ID

	Declare @dbpurchaseorderitemext_psproductid nvarchar(50)
	Declare @dbpurchaseorderitemext_psmproductid nvarchar(50)
	Declare @dbpurchaseorderitemext_psoriprice decimal(10, 2)
	Declare @dbpurchaseorderitemext_pssellcatid nvarchar(50)
	Declare @dbpurchaseorderitemext_psattribname nvarchar(200)
	Declare @dbpurchaseorderitemext_psmodelno nvarchar(50)
	Declare @dbpurchaseorderitemext_pscost int
	Declare @dbpurchaseorderitemext_psfvf int
	Declare @dbpurchaseorderitemext_psproducttype int

	Declare @get_purchaseorderitemcode nvarchar(15)



	Declare @run_repeat int
	set @run_repeat=0

	Declare @grpscount int
	set @grpscount=0

	Declare @purchaseordergroupext_status_default int
	set @purchaseordergroupext_status_default=0


	declare @dbpurchaseorder_code nvarchar(15)        


	DECLARE mycursor CURSOR FOR 
	select A.sn,A.num as 'ItemListID',ISNULL(P.ItemListProductID,0),ISNULL(P.ItemListSellingQty,0),P2.num as 'ItemID',ISNULL(P3.ProductID,0) as 'ItemProductID' ,P3.ItemSellingQty,P3.CurrencyID,ISNULL(P3.ProductCost,0),P3.CurrencyAverageExchangerate,ISNULL(P3.PriceCoupon,0) as 'ItemPriceCoupon',R1.num as 'ItemName',R2.num as 'PurchaseOrdersDelivType',ISNULL(R3.num,'') as 'PurchaseOrdersDelivData',ISNULL(R4.num,'') as 'PurchaseOrderItemsNote',R5.num as 'PurchaseOrderItemsQty',S.num as 'PurchaseOrderItemsPrice',ISNULL(T.num,0) as 'PurchaseOrderItemPriceinst',ISNULL(U.num,0) as 'PurchaseOrderItemPriceCoupon',ISNULL(V.num,0) as 'PurchaseOrderitemRedmbln',ISNULL(W.num,0) as 'PurchaseOrderItemRedmtkout',ISNULL(X.num,0) as 'PurchaseOrderItemRedmfdbck',ISNULL(Y.num,0) as 'PurchaseOrderItemWfbln',ISNULL(Z.num,0) as 'PurchaseOrderItemWftkout',ISNULL(Z1.num,'') as 'PurchaseOrderItemActID',ISNULL(Z2.num,0) as 'PurchaseOrderItemActtkout',ISNULL(Z3.num1,0) as 'ItemAttribID',Z3.num2 as 'ItemAttribName',Z3.num3 as 'ItemAttribProductID' , ISNULL(Z4.num,'') as 'PurchaseOrdersNote',ISNULL(Z5.num1,0) as 'ItemListAttribID',Z5.num2 as 'ItemListAttribName', Z5.num3 as 'ItemListAttribProductID',YS1.num as 'PurchaseOrderItemextPsProductID',YS2.num as 'PurchaseOrderItemExtPsmProductID',0.00 as 'PurchaseOrderItemExtPsOriPrice',YS4.num as 'PurchaseOrderItemExtPsSellCatID',ISNULL(YS5.num,'') as 'PurchaseOrderItemExtPsAttribName',ISNULL(YS6.num,'') as 'PurchaseOrderItemExtPsModelNo',YS7.num as 'PurchaseOrderitemExtPsCost',YS8.num as 'PurchaseOrderItemExtPsfvf',YS9.num as 'PurchaseOrderItemExtPsProductType' FROM dbo.fn_EC_DataList('ItemListID',@purchaseorderitems_itemlistid ) A
	left join 
	(
		select K1.ID as 'ItemListID',K1.ItemListProductID,TWSQLDB.dbo.fn_EC_GetSellingQty(K1.Qty,K1.Qtyreg,AT.Qty,AT.QtyReg,K1.QtyLimit) as ItemListSellingQty from TWSQLDB.dbo.itemlist K1 left join TWSQLDB.dbo.itemstock AT with (nolock) on AT.ProductID=K1.ItemListProductID where K1.ID  in (select num FROM dbo.fn_EC_DataList('ItemlistID',@purchaseorderitems_itemlistid ))
	) P on A.num=P.ItemListID
	left join 
	(
		select B.sn,B.name,B.num FROM dbo.fn_EC_DataList('ItemID',@item_id) B 
	) P2 on A.sn=P2.sn
	left join 
	(
		select K2.ID as ItemID,K2.ProductID,TWSQLDB.dbo.fn_EC_GetSellingQty(K2.Qty,K2.Qtyreg,MT.Qty,MT.Qtyreg,K2.QtyLimit) as ItemSellingQty, K2.PriceCoupon,YL.ID as CurrencyID,PL.Cost as ProductCost,YL.AverageExchangeRate as CurrencyAverageExchangeRate from TWSQLDB.dbo.item K2 
		inner join TWSQLDB.dbo.itemstock MT on MT.ProductID=K2.ProductID 
		inner join TWSQLDB.dbo.product PL with (nolock) on MT.ProductID=PL.ID
		inner join TWSQLDB.dbo.seller SL with (nolock) on PL.SellerID=SL.ID
		inner join TWSQLDB.dbo.country CL with (nolock) on CL.ID=SL.CountryID
		inner join TWSQLDB.dbo.currency YL with (nolock) on YL.[Type]=CL.UsageCurrency and YL.[Year]=datepart("yyyy",getdate()) and YL.[Month]=datepart("MM",getdate())
		where K2.ID in (select num FROM dbo.fn_EC_DataList('ItemID',@item_id ) )
	) P3 on P2.num=P3.ItemID
	left join 
	(
	SELECT C1.sn,C1.name,C1.num FROM dbo.fn_EC_DataList('ItemName',@purchaseorders_itemname) C1 
	) R1 on A.sn=R1.sn
	left join 
	(
	SELECT C2.sn,C2.name,C2.num FROM dbo.fn_EC_DataList('PurchaseOrdersDelivType',@purchaseorders_delivtype) C2 
	) R2 on A.sn=R2.sn
	left join 
	(
	SELECT C3.sn,C3.name,C3.num FROM dbo.fn_EC_DataList('PurchaseOrdersDelivData',@purchaseorders_delivdata) C3 
	) R3 on A.sn=R3.sn
	left join 
	(
	SELECT C4.sn,C4.name,C4.num FROM dbo.fn_EC_DataList('PurchaseOrderItemsNote',@purchaseorderitems_note) C4
	) R4 on A.sn=R4.sn
	left join 
	(
	SELECT C5.sn,C5.name,C5.num FROM dbo.fn_EC_DataList('PurchaseOrderItemsQty',@purchaseorderitems_qty) C5
	) R5 on A.sn=R5.sn
	left join 
	(
	SELECT D.sn,D.name,D.num FROM dbo.fn_EC_DataList('PurchaseOrderItemsPrice',@purchaseorderitems_price) D 
	) S on A.sn=S.sn
	left join 
	(
	SELECT E.sn,E.name,E.num FROM dbo.fn_EC_DataList('PurchaseOrderItemPriceInst',@purchaseorderitems_priceinst) E 
	) T on A.sn=T.sn
	left join 
	(
	SELECT F.sn,F.name,F.num FROM dbo.fn_EC_DataList('PurchaseOrderItemPriceCoupon',@purchaseorderitems_pricecoupon) F 
	) U on A.sn=U.sn
	left join 
	(
	SELECT G.sn,G.name,G.num FROM dbo.fn_EC_DataList('PurchaseOrderItemRedmbln',@purchaseorderitems_redmbln) G 
	) V on A.sn=V.sn
	left join 
	(
	SELECT H.sn,H.name,H.num FROM dbo.fn_EC_DataList('PurchaseOrderItemRedmtkout',@purchaseorderitems_redmtkout) H 
	) W on A.sn=W.sn
	left join 
	(
	SELECT I.sn,I.name,I.num FROM dbo.fn_EC_DataList('PurchaseOrderItemRedmfdbck',@purchaseorderitems_redmfdbck) I 
	) X on A.sn=X.sn
	left join 
	(
	SELECT J.sn,J.name,J.num FROM dbo.fn_EC_DataList('PurchaseOrderItemWfbln',@purchaseorderitems_wfbln) J 
	) Y on A.sn=Y.sn
	left join 
	(
	SELECT K.sn,K.name,K.num FROM dbo.fn_EC_DataList('PurchaseOrderItemWftkout',@purchaseorderitems_wftkout) K
	) Z on A.sn=Z.sn
	left join 
	(
	SELECT L.sn,L.name,L.num FROM dbo.fn_EC_DataList('PurchaseOrderItemActID',@purchaseorderitems_actid) L
	) Z1 on A.sn=Z1.sn
	left join 
	(
	SELECT M.sn,M.name,M.num FROM dbo.fn_EC_DataList('PurchaseOrderItemActtkout',@purchaseorderitems_acttkout) M
	) Z2 on A.sn=Z2.sn
	left join 
	(
	SELECT N.sn,N.name,NT.ID as 'num1',NT.Name as 'num2', NT.ItemListProductID as 'num3' FROM dbo.fn_EC_DataList('ItemAttribID',@item_attribid) N
		left join TWSQLDB.dbo.itemlist NT on NT.ID=N.num
	) Z3 on A.sn=Z3.sn
	left join 
	(
	SELECT N2.sn,N2.name,N2.num FROM dbo.fn_EC_DataList('PurchaseOrdersNote',@purchaseorders_note) N2
	) Z4 on A.sn=Z4.sn
	left join 
	(
		SELECT O.sn,O.name,ZT.ID as 'num1',ZT.Name as 'num2', ZT.ItemListProductID as 'num3' FROM dbo.fn_EC_DataList('ItemListAttribID',@itemlist_attribid) O 
			left join TWSQLDB.dbo.itemlist ZT on ZT.ID=O.num
	) Z5 on A.sn=Z5.sn
	left join 
	(
	SELECT YY1.sn,YY1.name,YY1.num FROM dbo.fn_EC_DataList('PurchaseOrderItemExtPsproductID',@purchaseorderitemexts_psproductid) YY1
	) YS1 on A.sn=YS1.sn
	left join 
	(
	SELECT YY2.sn,YY2.name,YY2.num FROM dbo.fn_EC_DataList('PurchaseOrderItemExtPsmproductID',@purchaseorderitemexts_psmproductid) YY2
	) YS2 on A.sn=YS2.sn
	left join 
	(
	SELECT YY3.sn,YY3.name,YY3.num FROM dbo.fn_EC_DataList('PurchaseOrderItemExtPsOriPrice',@purchaseorderitemexts_psoriprice) YY3
	) YS3 on A.sn=YS3.sn
	left join 
	(
	SELECT YY4.sn,YY4.name,YY4.num FROM dbo.fn_EC_DataList('PurchaseOrderItemExtPsSellCatID',@purchaseorderitemexts_pssellcatid) YY4
	) YS4 on A.sn=YS4.sn
	left join 
	(
	SELECT YY5.sn,YY5.name,YY5.num FROM dbo.fn_EC_DataList('PurchaseOrderItemExtPsAttribName',@purchaseorderitemexts_psattribname) YY5
	) YS5 on A.sn=YS5.sn
	left join 
	(
	SELECT YY6.sn,YY6.name,YY6.num FROM dbo.fn_EC_DataList('PurchaseOrderItemExtPsmodelNo',@purchaseorderitemexts_psmodelno) YY6
	) YS6 on A.sn=YS6.sn
	left join 
	(
	SELECT YY7.sn,YY7.name,YY7.num FROM dbo.fn_EC_DataList('PurchaseOrderItemExtPsCost',@purchaseorderitemexts_pscost) YY7
	) YS7 on A.sn=YS7.sn
	left join 
	(
	SELECT YY8.sn,YY8.name,YY8.num FROM dbo.fn_EC_DataList('PurchaseOrderItemExtPsfvf',@purchaseorderitemexts_psfvf) YY8
	) YS8 on A.sn=YS8.sn
	left join 
	(
	SELECT YY9.sn,YY9.name,YY9.num FROM dbo.fn_EC_DataList('PurchaseOrderItemExtPsProductType',@purchaseorderitemexts_psproducttype) YY9
	) YS9 on A.sn=YS9.sn


	----COMMIT TRANSACTION 

	--開始新的SQL Transaction

	BEGIN TRANSACTION


	--新增purchaseordergroupid



	TagA7:

	insert TWSQLDB.dbo.purchaseordergroup(PriceSum,Note,OrderNum) values (@pricesum,@note,0)
	if(@@ERROR<>0)
		begin
			set @run_repeat=@run_repeat+1
			if(@run_repeat<4)
				goto TagA7
			ROLLBACK TRANSACTION
			set @dboutput='insert salesordergroup is error.'
			return
		end
	set @run_repeat=0
	set @salesorder_salesordergroupid=@@identity



	TagA:
	insert purchaseordergroup(ID,PriceSum,Note,OrderNum) values (@salesorder_salesordergroupid,@pricesum,@note,0)
	if(@@ERROR<>0)
		begin
			set @run_repeat=@run_repeat+1
			if(@run_repeat<4)
				goto TagA
			ROLLBACK TRANSACTION
			set @dboutput='insert purchaseordergroup is error.'
			return
		end
	set @run_repeat=0
	set @purchaseorder_purchaseordergroupid=@salesorder_salesordergroupid


	declare @oldpurchaseorderitemext_psproductid nvarchar(50)
	declare @oldpurchaseorderitemext_psmproductid nvarchar(50)
	declare @oldPurchaseOrderItem_itemid int
	set @oldpurchaseorderitemext_psproductid='0'
	set @oldpurchaseorderitemext_psmproductid='0'
	set @oldPurchaseOrderItem_itemid=0


	OPEN mycursor
	FETCH NEXT FROM mycursor INTO @dbsn,@dbitemlist_id,@dbitemlist_productid, @dbitemlist_sellingQty,@dbitem_id,@dbitem_productid,@dbitem_sellingQty,@dbPurchaseOrderItem_sourcecurrencyid,@dbPurchaseOrderItem_sourceprice, @dbPurchaseOrderItem_currencyexchangerate,@dbitem_pricecoupon,@dbitem_name,@dbpurchaseorder_delivtype,@dbpurchaseorder_delivdata,@dbPurchaseOrderItem_note,@PurchaseOrderItem_qty,@dbPurchaseOrderItem_localprice, @dbPurchaseOrderItem_localpriceinst, @dbPurchaseOrderItem_localpricecoupon, @dbPurchaseOrderItem_redmbln, @dbPurchaseOrderItem_redmtkout,@dbPurchaseOrderItem_redmfdbck,@dbPurchaseOrderItem_wfbln,@dbPurchaseOrderItem_wftkout,@dbPurchaseOrderItem_actid,@dbPurchaseOrderItem_acttkout,@dbitem_attribid,@dbitem_attribname, @dbitem_attribproductid,@dbpurchaseorders_note,@dbitemlist_attribid,@dbitemlist_attribname,@dbitemlist_attribproductid,@dbpurchaseorderitemext_psproductid,@dbpurchaseorderitemext_psmproductid,@dbpurchaseorderitemext_psoriprice,@dbpurchaseorderitemext_pssellcatid,@dbpurchaseorderitemext_psattribname,@dbpurchaseorderitemext_psmodelno,@dbpurchaseorderitemext_pscost,@dbpurchaseorderitemext_psfvf,@dbpurchaseorderitemext_psproducttype
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		--處理新增配件子單
		if(@dbitemlist_id=0)
			begin
				--主單
				if(@oldPurchaseOrderItem_itemid<>@dbitem_id)
				begin
					TagA3:	
					set @dbpurchaseorder_code=dbo.fn_EC_GetPurchaseorderAutoSN(@purchaseorderPrefix)
					
					if(@@ERROR<>0)
						begin
							set @run_repeat=@run_repeat+1
							if(@run_repeat<4)
								goto TagA3
							CLOSE mycursor
							DEALLOCATE mycursor
							ROLLBACK TRANSACTION
							set @dboutput='getpurchaseorderAutoSN is error.'
							return
						end
					set @run_repeat=0	
					
					TagB:
					insert purchaseorder
					( 
						Code,Telday,InvoReceiver,InvoID,InvoTitle,InvoLoc,InvoZip,InvoAddr,Name,[Status],PayType,Email,
						DelivLoc,DelivZip,DelivAddr,IdNo,Mobile,AccountID,RecvName,RecvMobile,RecvTelDay,CardNO,CardType,CardBank,CardExpire,CardBirthday,CardLoc,CardZip,
						CardAddr,Note,DelivType,DelivData,RemoteIP,CoServerName,ServerName,AuthCode,AuthDate,AuthNote,UpdateUser,PurchaseOrderGroupID,[Date],CreateDate,UpdateDate
					) 
					values
					( 
						@dbpurchaseorder_code,@purchaseorder_telday,@purchaseorder_invoreceiver,@purchaseorder_invoid,@purchaseorder_invotitle,@purchaseorder_involoc,@purchaseorder_invozip,@purchaseorder_invoaddr,@purchaseorder_name,@purchaseorder_status,@purchaseorder_paytype,@purchaseorder_email,
						@purchaseorder_delivloc,@purchaseorder_delivzip,@purchaseorder_delivaddr,@purchaseorder_idno,@purchaseorder_mobile,@purchaseorder_accountid,@purchaseorder_recvname,@purchaseorder_recvmobile,@purchaseorder_recvtelday,@purchaseorder_cardno,@purchaseorder_cardtype,@purchaseorder_cardbank,@purchaseorder_cardexpire,@purchaseorder_cardbirthday,@purchaseorder_cardloc,@purchaseorder_cardzip,
						@purchaseorder_cardaddr,replace(@dbpurchaseorders_note,'repdot',','),@dbpurchaseorder_delivtype,@dbpurchaseorder_delivdata,@purchaseorder_remoteip,@purchaseorder_coservername,@purchaseorder_servername,@purchaseorder_authcode,@purchaseorder_authdate,@purchaseorder_authnote,@purchaseorder_updateuser,@purchaseorder_purchaseordergroupid,getdate(),getdate(),getdate()
					)
					
					if(@@ERROR<>0)
						begin
							set @run_repeat=@run_repeat+1
							if(@run_repeat<4)
								goto TagB
							CLOSE mycursor
							DEALLOCATE mycursor
							ROLLBACK TRANSACTION
							set @dboutput='insert purchaseorder is error.'
							return
						end
					set @run_repeat=0
				end	
				
			
				--主件的子單
				TagC:	
					set @get_purchaseorderitemcode=dbo.fn_EC_GetPurchaseorderitemAutoSN(@purchaseorderitemPrefix)	
					if(@@ERROR<>0)
						begin
							set @run_repeat=@run_repeat+1
							if(@run_repeat<4)
								goto TagC
							CLOSE mycursor
							DEALLOCATE mycursor
							ROLLBACK TRANSACTION
							set @dboutput='getpurchaseorderitemAutoSN is error.'
							return
						end
					set @run_repeat=0
					
				
				TagD:
				
				if(@dbpurchaseorderitemext_psproducttype=1)
					begin
						if(@dbitem_attribid >0)
						begin
							insert purchaseorderitem 
							(
								Code,PurchaseOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,LocalPriceInst,
								SourceCurrencyID,SourcePrice,LocalCurrencyID,LocalPrice,LocalPriceCoupon,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,ActtkOut,[Date],CreateDate,UpdateDate,WarehouseID
							)
							values
							(
								@get_purchaseorderitemcode,@dbpurchaseorder_code,@dbitem_id,@dbitemlist_id,@PurchaseOrderItem_qty,@dbitem_attribproductid,0,replace(@dbitem_name,'repdot',','),replace(@dbPurchaseOrderItem_note,'repdot',','),@dbPurchaseOrderItem_localpriceinst,@dbPurchaseOrderItem_sourcecurrencyid,@dbPurchaseOrderItem_sourceprice,@PurchaseOrderItem_localcurrencyid,@dbPurchaseOrderItem_localprice,@dbPurchaseOrderItem_localpricecoupon,replace(@dbitem_attribname,'repdot',','),@dbPurchaseOrderItem_redmbln,@dbPurchaseOrderItem_redmtkout,@dbPurchaseOrderItem_redmfdbck,@dbPurchaseOrderItem_wfbln,@dbPurchaseOrderItem_wftkout,@dbPurchaseOrderItem_actid,@dbPurchaseOrderItem_acttkout,getdate(),getdate(),getdate(),@purchaseorderitemexts_WareHouse 
							)
						end  
						else
							begin
								insert purchaseorderitem 
								(
									Code,PurchaseOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,LocalPriceInst,
									SourceCurrencyID,SourcePrice,LocalCurrencyID,LocalPrice,LocalPriceCoupon,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,ActtkOut,[Date],CreateDate,UpdateDate,WarehouseID
								)
								values
								(
									@get_purchaseorderitemcode,@dbpurchaseorder_code,@dbitem_id,@dbitemlist_id,@PurchaseOrderItem_qty,@dbpurchaseorderitemext_psproductid,0,replace(@dbitem_name,'repdot',','),replace(@dbPurchaseOrderItem_note,'repdot',','),@dbPurchaseOrderItem_localpriceinst,@dbPurchaseOrderItem_sourcecurrencyid,@dbPurchaseOrderItem_sourceprice,@PurchaseOrderItem_localcurrencyid,@dbPurchaseOrderItem_localprice,@dbPurchaseOrderItem_localpricecoupon,replace(@dbpurchaseorders_note,'repdot',','),@dbPurchaseOrderItem_redmbln,@dbPurchaseOrderItem_redmtkout,@dbPurchaseOrderItem_redmfdbck,@dbPurchaseOrderItem_wfbln,@dbPurchaseOrderItem_wftkout,@dbPurchaseOrderItem_actid,@dbPurchaseOrderItem_acttkout,getdate(),getdate(),getdate(),@purchaseorderitemexts_WareHouse 								
								)
							end
					end
				else
					begin
						if(@dbpurchaseorderitemext_psproducttype=30)
							begin
								insert purchaseorderitem 
								(
									Code,PurchaseOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,LocalPriceInst,
									SourceCurrencyID,SourcePrice,LocalCurrencyID,LocalPrice,LocalPriceCoupon,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,ActtkOut,[Date],CreateDate,UpdateDate,WarehouseID
								)
								values
								(
									@get_purchaseorderitemcode,@dbpurchaseorder_code,@dbitem_id,@dbitemlist_id,@PurchaseOrderItem_qty,'1155973',0,replace(@dbitem_name,'repdot',','),replace(@dbPurchaseOrderItem_note,'repdot',','),@dbPurchaseOrderItem_localpriceinst,@dbPurchaseOrderItem_sourcecurrencyid,@dbPurchaseOrderItem_sourceprice,@PurchaseOrderItem_localcurrencyid,@dbPurchaseOrderItem_localprice,@dbPurchaseOrderItem_localpricecoupon,'',@dbPurchaseOrderItem_redmbln,@dbPurchaseOrderItem_redmtkout,@dbPurchaseOrderItem_redmfdbck,@dbPurchaseOrderItem_wfbln,@dbPurchaseOrderItem_wftkout,@dbPurchaseOrderItem_actid,@dbPurchaseOrderItem_acttkout,getdate(),getdate(),getdate(),@purchaseorderitemexts_WareHouse 
								)
							end
						else
							begin
								if(@dbitem_attribid >0)
									begin
										insert purchaseorderitem 
										(
											Code,PurchaseOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,LocalPriceInst,
											SourceCurrencyID,SourcePrice,LocalCurrencyID,LocalPrice,LocalPriceCoupon,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,ActtkOut,[Date],CreateDate,UpdateDate,WarehouseID
										)
										values
										(
											@get_purchaseorderitemcode,@dbpurchaseorder_code,@dbitem_id,@dbitemlist_id,@PurchaseOrderItem_qty,ISNULL(@dbitem_attribproductid,0),0,replace(@dbitem_name,'repdot',','),replace(@dbPurchaseOrderItem_note,'repdot',','),@dbPurchaseOrderItem_localpriceinst,@dbPurchaseOrderItem_sourcecurrencyid,@dbPurchaseOrderItem_sourceprice,@PurchaseOrderItem_localcurrencyid,@dbPurchaseOrderItem_localprice,@dbPurchaseOrderItem_localpricecoupon,replace(@dbitem_attribname,'repdot',','),@dbPurchaseOrderItem_redmbln,@dbPurchaseOrderItem_redmtkout,@dbPurchaseOrderItem_redmfdbck,@dbPurchaseOrderItem_wfbln,@dbPurchaseOrderItem_wftkout,@dbPurchaseOrderItem_actid,@dbPurchaseOrderItem_acttkout,getdate(),getdate(),getdate(),@purchaseorderitemexts_WareHouse 
										)
									end
								else
								begin
									insert purchaseorderitem 
									(
										Code,PurchaseOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,LocalPriceInst,
										SourceCurrencyID,SourcePrice,LocalCurrencyID,LocalPrice,LocalPriceCoupon,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,ActtkOut,[Date],CreateDate,UpdateDate,WarehouseID
									)
									values
									(
										@get_purchaseorderitemcode,@dbpurchaseorder_code,@dbitem_id,@dbitemlist_id,@PurchaseOrderItem_qty,@dbitem_productid,0,replace(@dbitem_name,'repdot',','),replace(@dbPurchaseOrderItem_note,'repdot',','),@dbPurchaseOrderItem_localpriceinst,@dbPurchaseOrderItem_sourcecurrencyid,@dbPurchaseOrderItem_sourceprice,@PurchaseOrderItem_localcurrencyid,@dbPurchaseOrderItem_localprice,@dbPurchaseOrderItem_localpricecoupon,'',@dbPurchaseOrderItem_redmbln,@dbPurchaseOrderItem_redmtkout,@dbPurchaseOrderItem_redmfdbck,@dbPurchaseOrderItem_wfbln,@dbPurchaseOrderItem_wftkout,@dbPurchaseOrderItem_actid,@dbPurchaseOrderItem_acttkout,getdate(),getdate(),getdate(),@purchaseorderitemexts_WareHouse 
									)
								end
							end					
					end
				
				if(@@ERROR<>0)
					begin
						set @run_repeat=@run_repeat+1
						if(@run_repeat<4)
							goto TagD
						CLOSE mycursor
						DEALLOCATE mycursor
						ROLLBACK TRANSACTION
						set @dboutput='insert purchaseorderitem is error.'
						return
					end
				set @run_repeat=0

				TagD3:
				
				
					
			end
		else --配件單
			begin
				--新增配件
				TagE:
				set @get_purchaseorderitemcode=dbo.fn_EC_GetPurchaseorderitemAutoSN(@purchaseorderitemPrefix)
				if(@@ERROR<>0)
					begin
						set @run_repeat=@run_repeat+1
						if(@run_repeat<4)
							goto TagE
						CLOSE mycursor
						DEALLOCATE mycursor
						ROLLBACK TRANSACTION
						set @dboutput='getpurchaseorderitemAutoSN is error.'
						return
					end
				set @run_repeat=0
				
				
				TagH:
				if(@dbitemlist_attribid>0)
					begin
						insert purchaseorderitem 
						(
							Code,PurchaseOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,LocalPriceInst,
							SourceCurrencyID,SourcePrice,LocalCurrencyID,LocalPrice,LocalPriceCoupon,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,ActtkOut,[Date],CreateDate,UpdateDate,WarehouseID
						)
						values
						(
							@get_purchaseorderitemcode,@dbpurchaseorder_code,@dbitem_id,@dbitemlist_id,@PurchaseOrderItem_qty,0,@dbitemlist_attribproductid,replace(@dbitem_name,'repdot',','),replace(@dbPurchaseOrderItem_note,'repdot',','),@dbPurchaseOrderItem_localpriceinst,@dbPurchaseOrderItem_sourcecurrencyid,@dbPurchaseOrderItem_sourceprice,@PurchaseOrderItem_localcurrencyid,@dbPurchaseOrderItem_localprice,@dbPurchaseOrderItem_localpricecoupon,replace(@dbitemlist_attribname,'repdot',','),@dbPurchaseOrderItem_redmbln,@dbPurchaseOrderItem_redmtkout,@dbPurchaseOrderItem_redmfdbck,@dbPurchaseOrderItem_wfbln,@dbPurchaseOrderItem_wftkout,@dbPurchaseOrderItem_actid,@dbPurchaseOrderItem_acttkout,getdate(),getdate(),getdate(),@purchaseorderitemexts_WareHouse 
						)
					end
				else
					begin
						insert purchaseorderitem 
						(
							Code,PurchaseOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,LocalPriceInst,
							SourceCurrencyID,SourcePrice,LocalCurrencyID,LocalPrice,LocalPriceCoupon,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,ActtkOut,[Date],CreateDate,UpdateDate,WarehouseID
						)
						values
						(
							@get_purchaseorderitemcode,@dbpurchaseorder_code,@dbitem_id,@dbitemlist_id,@PurchaseOrderItem_qty,0,@dbitemlist_productid,replace(@dbitem_name,'repdot',','),replace(@dbPurchaseOrderItem_note,'repdot',','),@dbPurchaseOrderItem_localpriceinst,@dbPurchaseOrderItem_sourcecurrencyid,@dbPurchaseOrderItem_sourceprice,@PurchaseOrderItem_localcurrencyid,@dbPurchaseOrderItem_localprice,@dbPurchaseOrderItem_localpricecoupon,replace(@dbpurchaseorderitemext_psattribname,'repdot',','),@dbPurchaseOrderItem_redmbln,@dbPurchaseOrderItem_redmtkout,@dbPurchaseOrderItem_redmfdbck,@dbPurchaseOrderItem_wfbln,@dbPurchaseOrderItem_wftkout,@dbPurchaseOrderItem_actid,@dbPurchaseOrderItem_acttkout,getdate(),getdate(),getdate() ,@purchaseorderitemexts_WareHouse 
						)
					end

				if(@@ERROR<>0)
					begin
						set @run_repeat=@run_repeat+1
						if(@run_repeat<4)
							goto TagH
						CLOSE mycursor
						DEALLOCATE mycursor
						ROLLBACK TRANSACTION
						set @dboutput='insert purchaseorderitem is error.'
						return
					end
				set @run_repeat=0

				
			
				
			end
			set @oldpurchaseorderitemext_psproductid=@dbpurchaseorderitemext_psproductid
			set @oldpurchaseorderitemext_psmproductid=@dbpurchaseorderitemext_psmproductid
			set @oldPurchaseOrderItem_itemid=@dbitem_id
		FETCH NEXT FROM mycursor INTO @dbsn,@dbitemlist_id,@dbitemlist_productid, @dbitemlist_sellingQty,@dbitem_id,@dbitem_productid,@dbitem_sellingQty,@dbPurchaseOrderItem_sourcecurrencyid,@dbPurchaseOrderItem_sourceprice, @dbPurchaseOrderItem_currencyexchangerate , @dbitem_pricecoupon,@dbitem_name,@dbpurchaseorder_delivtype,@dbpurchaseorder_delivdata,@dbPurchaseOrderItem_note,@PurchaseOrderItem_qty,@dbPurchaseOrderItem_localprice, @dbPurchaseOrderItem_localpriceinst, @dbPurchaseOrderItem_localpricecoupon, @dbPurchaseOrderItem_redmbln, @dbPurchaseOrderItem_redmtkout,@dbPurchaseOrderItem_redmfdbck,@dbPurchaseOrderItem_wfbln,@dbPurchaseOrderItem_wftkout,@dbPurchaseOrderItem_actid,@dbPurchaseOrderItem_acttkout,@dbitem_attribid,@dbitem_attribname,@dbitem_attribproductid,@dbpurchaseorders_note,@dbitemlist_attribid,@dbitemlist_attribname,@dbitemlist_attribproductid,@dbpurchaseorderitemext_psproductid,@dbpurchaseorderitemext_psmproductid,@dbpurchaseorderitemext_psoriprice,@dbpurchaseorderitemext_pssellcatid,@dbpurchaseorderitemext_psattribname,@dbpurchaseorderitemext_psmodelno,@dbpurchaseorderitemext_pscost,@dbpurchaseorderitemext_psfvf,@dbpurchaseorderitemext_psproducttype

	END

	CLOSE mycursor
	DEALLOCATE mycursor

	COMMIT TRANSACTION 

	select O.*, I.* from purchaseorder O with (nolock) inner join purchaseorderitem I with (nolock) on O.Code=I.PurchaseOrderCode where O.PurchaseOrderGroupID=@purchaseorder_purchaseordergroupid order by O.Code,I.Code











GO
