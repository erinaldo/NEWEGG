USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_InsertSalesOrdersBySeller]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE PROCEDURE [dbo].[UP_EC_InsertSalesOrdersBySeller]
	@instype int --新增的類型 0是預設 1表示Seller
	,@item_id varchar(4000) --主件的item_id
	,@salesorderPrefix varchar(4)
	,@salesorderitemPrefix varchar(4)
	,@pricesum decimal(10,2)
	,@ordernum int
	,@note nvarchar(100)
	,@item_attribid varchar(200) --主件屬性id
	,@salesorder_telday varchar(30)  --訂單白天聯絡電話
	,@salesorder_invoreceiver nvarchar(50) --發票接收狀態, 託管or 寄送
	,@salesorder_invoid char(10) --發票編號
	,@salesorder_invotitle nvarchar(50) --發票抬頭
	,@salesorder_involoc nvarchar(10)  --發票區域
	,@salesorder_invozip char(5) --發票郵遞區號
	,@salesorder_invoaddr nvarchar(150) --發票寄送地址
	,@salesorder_name nvarchar(50) --訂購人姓名
	,@salesorder_paytypeid int --訂單付款類型Table ID
	,@salesorder_paytype int --訂單付款類型
	,@salesorder_email varchar(256) --訂單email
	,@salesorder_delivloc nvarchar(10)  --配送區域
	,@salesorder_delivzip char(5) --配送郵遞區號
	,@salesorder_delivaddr nvarchar(150) --配送地址
	,@salesorder_delivengaddr varchar(150) --英文配送地址
	,@salesorder_idno varchar(20) 
	,@salesorder_mobile varchar(30) 
	,@salesorder_accountid int 
	,@salesorder_recvname nvarchar(50)
	,@salesorder_recvengname varchar(50)
	,@salesorder_recvmobile varchar(30)  
	,@salesorder_recvtelday varchar(30)
	,@salesorder_cardno varchar(64) 
	,@salesorder_cardtype char(10) 
	,@salesorder_cardbank nvarchar(50) 
	,@salesorder_cardexpire char(10) 
	,@salesorder_cardbirthday char(10) 
	,@salesorder_cardloc nvarchar(10) 
	,@salesorder_cardzip char(5) 
	,@salesorder_cardaddr nvarchar(150) 
	,@salesorder_status int	
	,@salesorders_note nvarchar(4000) 
	,@salesorders_delivtype nvarchar(4000)
	,@salesorders_delivdata nvarchar(4000)
	,@salesorder_remoteip char(15) 
	,@salesorder_coservername varchar(50) 
	,@salesorder_servername varchar(50) 
	,@salesorder_authcode varchar(50) 
	,@salesorder_authdate datetime 
	,@salesorder_authnote nvarchar(50) 
	,@salesorder_updateuser nvarchar(50) 
	--,@salesorder_salesordergroupid bigint
	,@salesorders_itemname nvarchar(4000)
	,@salesorderitems_itemlistid nvarchar(4000)
	,@salesorderitems_qty nvarchar(4000)
	,@salesorderitems_note nvarchar(4000)
	,@salesorderitems_price nvarchar(4000)
	,@salesorderitems_displayprice nvarchar(4000)
    ,@salesorderitems_discountprice nvarchar(4000)
	,@salesorderitems_shippingexpense nvarchar(4000)
	,@salesorderitems_serviceexpense nvarchar(4000)
	,@salesorderitems_tax nvarchar(4000)
    ,@salesorderitems_itempricesum nvarchar(4000)
	,@salesorderitems_installmentfee nvarchar(4000)
	,@salesorderitems_priceinst nvarchar(4000)
	,@salesorderitems_pricecoupon nvarchar(4000)
	,@salesorderitems_coupons nvarchar(4000)
	,@salesorderitems_redmbln nvarchar(4000)
	,@salesorderitems_redmtkout nvarchar(4000)
	,@salesorderitems_redmfdbck nvarchar(4000)
	,@salesorderitems_wfbln nvarchar(4000)
	,@salesorderitems_wftkout nvarchar(4000)
	,@salesorderitems_actid nvarchar(4000)
	,@salesorderitems_acttkout nvarchar(4000)
	,@salesorderitems_isnew nvarchar(4000)
	,@itemlist_attribid nvarchar(4000) --配件屬性id
	,@salesordergroupext_pscartid int
	,@salesordergroupext_pssellerid nvarchar(4000)
	,@salesordergroupext_pscarrynote nvarchar(4000)
	,@salesordergroupext_pshasact int
	,@salesordergroupext_pshaspartialauth int
	,@salesorderitemexts_psproductid nvarchar(4000)
	,@salesorderitemexts_psmproductid nvarchar(4000)
	,@salesorderitemexts_psoriprice nvarchar(4000)
	,@salesorderitemexts_pssellcatid nvarchar(4000)
	,@salesorderitemexts_psattribname nvarchar(4000)
	,@salesorderitemexts_psmodelno nvarchar(4000)
	,@salesorderitemexts_pscost nvarchar(4000)
	,@salesorderitemexts_psfvf nvarchar(4000)
	,@salesorderitemexts_psproducttype nvarchar(4000)
	,@dboutput nvarchar(4000) output
AS

--傳入的 salesorderitem 參數
     
declare @salesorderitems_productid int --從DB
declare @salesorderitems_productlistid int  --從DB
declare @salesorderitems_name nvarchar(200) --從DB

Declare @outnumbydate int
Declare @salesorder_salesordergroupid int
Declare @dbsn int
Declare @dbitemlist_id int
Declare @dbitem_id int--------------------------------Declare @dbitem_id int
Declare @dbitemlist_productid int
Declare @dbitemlist_sellingQty int
Declare @dbitem_sellingQty int
Declare @dbitem_productid int
Declare @dbitem_pricecoupon decimal(10,2)
Declare @dbitem_name nvarchar(200)


Declare @dbsalesorder_delivtype int
Declare @dbsalesorder_delivdata nvarchar(50)
Declare @dbsalesorderitem_note nvarchar(50)
Declare @dbsalesorderitem_price decimal(10,2)
Declare @dbsalesorderitem_displayprice decimal(10,2)
Declare @dbsalesorderitem_discountprice decimal(10,2)
Declare @dbsalesorderitem_shippingexpense decimal(10,2)
Declare @dbsalesorderitem_serviceexpense decimal(10,2)
Declare @dbsalesorderitem_tax decimal(10,2)
Declare @dbsalesorderitem_itempricesum decimal(10,2)
Declare @dbsalesorderitem_installmentfee decimal(10,2)
Declare @dbsalesorderitem_priceinst decimal(10,2)
Declare @dbsalesorderitem_pricecoupon decimal(10,2)
Declare @dbsalesorderitem_coupons nvarchar(20)
Declare @dbsalesorderitem_redmbln int
Declare @dbsalesorderitem_redmtkout int
Declare @dbsalesorderitem_redmfdbck int
Declare @dbsalesorderitem_wfbln int
Declare @dbsalesorderitem_wftkout int
Declare @dbsalesorderitem_actid nvarchar(10)
Declare @dbsalesorderitem_acttkout int
Declare @dbsalesorderitem_isnew char(1)
Declare @dbitem_canbuyqty int --可買的主商品數量
Declare @dbitemlist_canbuyqty int --可買的配件商品數量
Declare @dbitemlist_sellingQtyset int --設定的配件商品數量
Declare @dbitem_attribid int --主件屬性id
Declare @dbitem_attribname nvarchar(200) --主件屬性名稱
Declare @dbitem_attribproductid int --主件屬性商品id
Declare @dbsalesorders_note nvarchar(100) --配件屬性id
Declare @dbitemlist_attribid int --配件屬性id
Declare @dbitemlist_attribname nvarchar(200) --配件屬性名稱
Declare @dbitemlist_attribproductid int --配件屬性商品ID
--ProStore
Declare @dbsalesorderitemext_psproductid nvarchar(50)
Declare @dbsalesorderitemext_psmproductid nvarchar(50)
Declare @dbsalesorderitemext_psoriprice decimal(10,2)
Declare @dbsalesorderitemext_pssellcatid nvarchar(50)
Declare @dbsalesorderitemext_psattribname nvarchar(200)
Declare @dbsalesorderitemext_psmodelno nvarchar(50)
Declare @dbsalesorderitemext_pscost int
Declare @dbsalesorderitemext_psfvf int
Declare @dbsalesorderitemext_psproducttype int

Declare @get_salesorderitemcode nvarchar(15)



Declare @run_repeat int
set @run_repeat=0

Declare @grpscount int
set @grpscount=0

Declare @salesordergroupext_status_default int
set @salesordergroupext_status_default=0


declare @dbsalesorder_code nvarchar(15)        

--P1.item_id,P1.item_productid,P1.item_pricecoupon,P1.item_name
--1020622 為一般商品的item_productid
DECLARE mycursor CURSOR FOR 
select A.sn,A.num as 'ItemListID',ISNULL(P.ItemListProductID,0),ISNULL(P.ItemListSellingQty,0),P2.num as 'ItemID',P3.ProductID as 'ItemProductID' ,P3.ItemSellingQty,ISNULL(P3.PriceCoupon,0) as 'ItemPriceCoupon',R1.num as 'ItemName',R2.num as 'SalesOrdersDelivType',ISNULL(R3.num,'') as 'SalesOrdersDelivData',ISNULL(R4.num,'') as 'SalesOrderItemsNote',convert(decimal(10,2),S.num) as 'SalesOrderItemsPrice',convert(decimal(10,2),S1.num) as 'SalesOrderItemsDisplayPrice',convert(decimal(10,2),S2.num) as 'SalesOrderItemsDiscountPrice',convert(decimal(10,2),PP.num) as 'SalesOrderItemsShippingexpense',convert(decimal(10,2),QQ.num) as 'SalesOrderItemsServiceexpense',convert(decimal(10,2),RR.num) as 'SalesOrderItemsTax',convert(decimal(10,2),SS.num) as 'SalesOrderItemsItemPriceSum',convert(decimal(10,2),SS1.num) as 'SalesOrderItemsInstallmentFee',convert(decimal(10,2),ISNULL(T.num,0)) as 'SalesOrderitemPriceInst',convert(decimal(10,2),ISNULL(U.num,0)) as 'SalesOrderItemPriceCoupon',ISNULL(U1.num,'') as 'SalesOrderitemCoupons',ISNULL(V.num,0) as 'SalesOrderitemRedmbln',ISNULL(W.num,0) as 'SalesOrderitemRedmtkout',ISNULL(X.num,0) as 'SalesOrderitemRedmfdbck',ISNULL(Y.num,0) as 'SalesOrderitemWfbln',ISNULL(Z.num,0) as 'SalesOrderitemWftkout',ISNULL(Z1.num,'') as 'SalesOrderitemActID',ISNULL(Z2.num,0) as 'SalesOrderitemActtkout',ISNULL(ZA1.num,'') as 'SalesOrderitemIsNew',ISNULL(Z3.num1,0) as 'ItemAttribID',Z3.num2 as 'ItemAttribName',Z3.num3 as 'ItemAttribProductID' , ISNULL(Z4.num,'') as 'SalesOrdersNote',ISNULL(Z5.num1,0) as 'ItemListAttribID',Z5.num2 as 'ItemListAttribName', Z5.num3 as 'ItemListAttribProductID',YS1.num as 'SalesOrderitemExtPSProductID',YS2.num as 'SalesOrderItemExtPSmProductID',0.00 as 'SalesOrderItemExtPSOriPrice',YS4.num as 'SalesOrderItemExt_PSSellCatID',ISNULL(YS5.num,'') as 'SalesOrderItemExtPSAttribName',ISNULL(YS6.num,'') as 'SalesOrderItemExtPSmodelNo',YS7.num as 'SalesOrderItemExtPSCost',YS8.num as 'SalesOrderItemExtPSfvf',YS9.num as 'SalesOrderItemExtPSProductType' FROM dbo.fn_EC_DataList('itemlist_id',@salesorderitems_itemlistid ) A
--select A.sn,A.num as 'itemlist_id',ISNULL(P.itemlist_itemlistproductid,0),ISNULL(P.itemlist_sellingQty,0),P2.num as 'item_id',P3.item_productid as 'item_productid',P3.item_sellingQty,ISNULL(P3.item_pricecoupon,0) as 'item_pricecoupon',R1.num as 'item_name',R2.num as 'salesorders_delivtype',ISNULL(R3.num,'') as 'salesorders_delivdata',ISNULL(R4.num,'') as 'salesorderitems_note',convert(decimal(10,2),S.num) as 'salesorderitems_price',convert(decimal(10,2),ISNULL(T.num,0)) as 'salesorderitem_priceinst',convert(decimal(10,2),ISNULL(U.num,0)) as 'salesorderitem_pricecoupon',ISNULL(V.num,0) as 'salesorderitem_redmbln',ISNULL(W.num,0) as 'salesorderitem_redmtkout',ISNULL(X.num,0) as 'salesorderitem_redmfdbck',ISNULL(Y.num,0) as 'salesorderitem_wfbln',ISNULL(Z.num,0) as 'salesorderitem_wftkout',ISNULL(Z1.num,'') as 'salesorderitem_actid',ISNULL(Z2.num,0) as 'salesorderitem_acttkout',ISNULL(Z3.num1,0) as 'item_attribid',Z3.num2 as 'item_attribname',Z3.num3 as 'item_attribproductid', ISNULL(Z4.num,'') as 'salesorders_note',ISNULL(Z5.num1,0) as 'itemlist_attribid',Z5.num2 as 'itemlist_attribname', Z5.num3 as 'itemlist_attribproductid',YS1.num as 'salesorderitemext_psproductid',YS2.num as 'salesorderitemext_psmproductid',0.00 as 'salesorderitemext_psoriprice'   FROM dbo.fn_EC_DataList('itemlist_id',@salesorderitems_itemlistid ) A
left join 
(
	select K1.ID as ItemListID,K1.ItemListProductID,dbo.fn_EC_GetSellingQty(K1.Qty,K1.Qtyreg,AT.Qty,AT.Qtyreg,K1.Qtylimit) as ItemListSellingQty from ItemList K1 left join ItemStock AT with (nolock) on AT.ProductID=K1.ItemListProductID where K1.ID  in (select num FROM dbo.fn_EC_DataList('itemlist_id',@salesorderitems_itemlistid ))
) P on A.num=P.ItemListID
left join 
(
	select B.sn,B.name,B.num FROM dbo.fn_EC_DataList('item_id',@item_id) B 
) P2 on A.sn=P2.sn
left join 
(
	select K2.ID as ItemID,K2.ProductID,dbo.fn_EC_GetSellingQty(K2.Qty,K2.Qtyreg,MT.Qty,MT.Qtyreg,K2.QtyLimit) as ItemSellingQty, K2.PriceCoupon from item K2 inner join itemstock MT on MT.ProductID=K2.ProductID where K2.ID in (select num FROM dbo.fn_EC_DataList('item_id',@item_id ) )
) P3 on P2.num=P3.ItemID
left join 
(
SELECT C1.sn,C1.name,C1.num FROM dbo.fn_EC_DataList('item_name',@salesorders_itemname) C1 
) R1 on A.sn=R1.sn
left join 
(
SELECT C2.sn,C2.name,C2.num FROM dbo.fn_EC_DataList('salesorders_delivtype',@salesorders_delivtype) C2 
) R2 on A.sn=R2.sn
left join 
(
SELECT C3.sn,C3.name,C3.num FROM dbo.fn_EC_DataList('salesorders_delivdata',@salesorders_delivdata) C3 
) R3 on A.sn=R3.sn
left join 
(
SELECT C4.sn,C4.name,C4.num FROM dbo.fn_EC_DataList('salesorderitems_note',@salesorderitems_note) C4
) R4 on A.sn=R4.sn
left join 
(
SELECT D.sn,D.name,D.num FROM dbo.fn_EC_DataList('salesorderitems_price',@salesorderitems_price) D 
) S on A.sn=S.sn
left join 
(
SELECT D1.sn,D1.name,D1.num FROM dbo.fn_EC_DataList('salesorderitems_displayprice',@salesorderitems_displayprice) D1 
) S1 on A.sn=S1.sn
left join 
(
SELECT D2.sn,D2.name,D2.num FROM dbo.fn_EC_DataList('salesorderitems_discountprice',@salesorderitems_discountprice) D2
) S2 on A.sn=S2.sn
left join 
(
SELECT P.sn,P.name,P.num FROM dbo.fn_EC_DataList('salesorderitems_shippingexpense',@salesorderitems_shippingexpense) P 
) PP on A.sn=PP.sn
left join 
(
SELECT Q.sn,Q.name,Q.num FROM dbo.fn_EC_DataList('salesorderitems_serviceexpense',@salesorderitems_serviceexpense) Q 
) QQ on A.sn=QQ.sn
left join 
(
SELECT R.sn,R.name,R.num FROM dbo.fn_EC_DataList('salesorderitems_tax',@salesorderitems_tax) R 
) RR on A.sn=RR.sn
left join 
(
SELECT S.sn,S.name,S.num FROM dbo.fn_EC_DataList('salesorderitems_itempricesum',@salesorderitems_itempricesum) S 
) SS on A.sn=SS.sn
left join 
(
SELECT S1.sn,S1.name,S1.num FROM dbo.fn_EC_DataList('salesorderitems_installmentfee',@salesorderitems_installmentfee) S1 
) SS1 on A.sn=SS1.sn
left join 
(
SELECT E.sn,E.name,E.num FROM dbo.fn_EC_DataList('salesorderitem_priceinst',@salesorderitems_priceinst) E 
) T on A.sn=T.sn
left join 
(
SELECT F.sn,F.name,F.num FROM dbo.fn_EC_DataList('salesorderitem_pricecoupon',@salesorderitems_pricecoupon) F 
) U on A.sn=U.sn
left join 
(
SELECT F1.sn,F1.name,F1.num FROM dbo.fn_EC_DataList('salesorderitem_coupons',@salesorderitems_coupons) F1 
) U1 on A.sn=U1.sn
left join 
(
SELECT G.sn,G.name,G.num FROM dbo.fn_EC_DataList('salesorderitem_redmbln',@salesorderitems_redmbln) G 
) V on A.sn=V.sn
left join 
(
SELECT H.sn,H.name,H.num FROM dbo.fn_EC_DataList('salesorderitem_redmtkout',@salesorderitems_redmtkout) H 
) W on A.sn=W.sn
left join 
(
SELECT I.sn,I.name,I.num FROM dbo.fn_EC_DataList('salesorderitem_redmfdbck',@salesorderitems_redmfdbck) I 
) X on A.sn=X.sn
left join 
(
SELECT J.sn,J.name,J.num FROM dbo.fn_EC_DataList('salesorderitem_wfbln',@salesorderitems_wfbln) J 
) Y on A.sn=Y.sn
left join 
(
SELECT K.sn,K.name,K.num FROM dbo.fn_EC_DataList('salesorderitem_wftkout',@salesorderitems_wftkout) K
) Z on A.sn=Z.sn
left join 
(
SELECT L.sn,L.name,L.num FROM dbo.fn_EC_DataList('salesorderitem_actid',@salesorderitems_actid) L
) Z1 on A.sn=Z1.sn
left join 
(
SELECT M.sn,M.name,M.num FROM dbo.fn_EC_DataList('salesorderitem_acttkout',@salesorderitems_acttkout) M
) Z2 on A.sn=Z2.sn
left join 
(
SELECT M1.sn,M1.name,M1.num FROM dbo.fn_EC_DataList('salesorderitem_isnew',@salesorderitems_isnew) M1
) ZA1 on A.sn=ZA1.sn
left join 
(
SELECT N.sn,N.name,NT.ID as 'num1',NT.Name as 'num2', NT.ItemListProductID as 'num3' FROM dbo.fn_EC_DataList('item_attribid',@item_attribid) N
    left join itemlist NT on NT.ID=N.num
) Z3 on A.sn=Z3.sn
left join 
(
SELECT N2.sn,N2.name,N2.num FROM dbo.fn_EC_DataList('salesorders_note',@salesorders_note) N2
) Z4 on A.sn=Z4.sn
left join 
(
	SELECT O.sn,O.name,ZT.ID as 'num1',ZT.Name as 'num2', ZT.ItemListProductID as 'num3' FROM dbo.fn_EC_DataList('itemlist_attribid',@itemlist_attribid) O 
		left join itemlist ZT on ZT.ID=O.num
) Z5 on A.sn=Z5.sn
left join 
(
SELECT YY1.sn,YY1.name,YY1.num FROM dbo.fn_EC_DataList('salesorderitemext_psproductid',@salesorderitemexts_psproductid) YY1
) YS1 on A.sn=YS1.sn
left join 
(
SELECT YY2.sn,YY2.name,YY2.num FROM dbo.fn_EC_DataList('salesorderitemext_psmproductid',@salesorderitemexts_psmproductid) YY2
) YS2 on A.sn=YS2.sn
left join 
(
SELECT YY3.sn,YY3.name,YY3.num FROM dbo.fn_EC_DataList('salesorderitemext_psoriprice',@salesorderitemexts_psoriprice) YY3
) YS3 on A.sn=YS3.sn
left join 
(
SELECT YY4.sn,YY4.name,YY4.num FROM dbo.fn_EC_DataList('salesorderitemext_pssellcatid',@salesorderitemexts_pssellcatid) YY4
) YS4 on A.sn=YS4.sn
left join 
(
SELECT YY5.sn,YY5.name,YY5.num FROM dbo.fn_EC_DataList('salesorderitemext_psattribname',@salesorderitemexts_psattribname) YY5
) YS5 on A.sn=YS5.sn
left join 
(
SELECT YY6.sn,YY6.name,YY6.num FROM dbo.fn_EC_DataList('salesorderitemext_psmodelno',@salesorderitemexts_psmodelno) YY6
) YS6 on A.sn=YS6.sn
left join 
(
SELECT YY7.sn,YY7.name,YY7.num FROM dbo.fn_EC_DataList('salesorderitemext_pscost',@salesorderitemexts_pscost) YY7
) YS7 on A.sn=YS7.sn
left join 
(
SELECT YY8.sn,YY8.name,YY8.num FROM dbo.fn_EC_DataList('salesorderitemext_psfvf',@salesorderitemexts_psfvf) YY8
) YS8 on A.sn=YS8.sn
left join 
(
SELECT YY9.sn,YY9.name,YY9.num FROM dbo.fn_EC_DataList('salesorderitemext_psproducttype',@salesorderitemexts_psproducttype) YY9
) YS9 on A.sn=YS9.sn



--COMMIT TRANSACTION 

--開始新的SQL Transaction

BEGIN TRANSACTION

--檢查是否在salesordergroupext有成功的 salesordergroupext_status 與 salesordergroupext_pscartid, 有可能在處理中, 目前判斷五分鐘內, 若salesordergroup_status=99 , 則一樣回傳處理中狀態
--select @grpscount=id,@salesordergroupext_status_default=[Status] from salesordergroupext EX inner join salesordergroup G on G.ID=EX.SalesOrderGroupID where EX.pscartid=@salesordergroupext_pscartid and ( EX.[Status]=0 or ( EX.[Status]=99 and datediff(mi,G.CreateDate,getdate())<=5 ) )
--if(@grpscount>0)
--begin

--	ROLLBACK TRANSACTION
--	if (@salesordergroupext_status_default=99)
--		set @dboutput='salesorder is processing.'
--	else
--		set @dboutput='duplicate cartId.'
--	return
--end


--新增salesordergroupid

TagA:

insert salesordergroup(Pricesum,Note,OrderNum) values (@pricesum,@note,@ordernum)
if(@@ERROR<>0)
	begin
		set @run_repeat=@run_repeat+1
		if(@run_repeat<4)
			goto TagA
		ROLLBACK TRANSACTION
		set @dboutput='insert salesordergroup is error.'
		return
	end
set @run_repeat=0
set @salesorder_salesordergroupid=@@identity

TagA2:
if(@salesorder_status=0)
begin
	insert salesordergroupext(SalesOrderGroupID,PscartID,PsSellerID,PsCarryNote,PshasAct,PshasPartialAuth,[Status]) values(@salesorder_salesordergroupid,@salesordergroupext_pscartid,@salesordergroupext_pssellerid,@salesordergroupext_pscarrynote,@salesordergroupext_pshasact,@salesordergroupext_pshaspartialauth,0)
	if(@@ERROR<>0)
		begin
			set @run_repeat=@run_repeat+1
			if(@run_repeat<4)
				goto TagA2
			ROLLBACK TRANSACTION
			set @dboutput='insert salesordergroupext is error.'
			return
		end
end
else
begin

	insert salesordergroupext(SalesOrderGroupID,PsCartID,PsSellerID,PsCarryNote,PshasAct,PshaspartialAuth,[Status]) values(@salesorder_salesordergroupid,@salesordergroupext_pscartid,@salesordergroupext_pssellerid,@salesordergroupext_pscarrynote,@salesordergroupext_pshasact,@salesordergroupext_pshaspartialauth,99)
	if(@@ERROR<>0)
		begin
			set @run_repeat=@run_repeat+1
			if(@run_repeat<4)
				goto TagA2
			ROLLBACK TRANSACTION
			set @dboutput='insert salesordergroupext is error.'
			return
		end
end
set @run_repeat=0

declare @oldsalesorderitemext_psproductid nvarchar(50)
declare @oldsalesorderitemext_psmproductid nvarchar(50)
declare @oldsalesorderitem_itemid int
set @oldsalesorderitemext_psproductid='0'
set @oldsalesorderitemext_psmproductid='0'
set @oldsalesorderitem_itemid=0


--select @dbsn,@dbitemlist_id,@dbitemlist_productid, @dbitemlist_sellingQty,@dbitem_id,@dbitem_productid,@dbitem_sellingQty,@dbitem_pricecoupon,@dbitem_name,@dbsalesorder_delivtype,@dbsalesorder_delivdata,@dbsalesorderitem_note,@dbsalesorderitem_price, @dbsalesorderitem_priceinst, @dbsalesorderitem_pricecoupon, @dbsalesorderitem_redmbln, @dbsalesorderitem_redmtkout,@dbsalesorderitem_redmfdbck,@dbsalesorderitem_wfbln,@dbsalesorderitem_wftkout,@dbsalesorderitem_actid,@dbsalesorderitem_acttkout,@dbitem_attribid,@dbsalesorders_note,@dbitemlist_attribname,@dbsalesorderitemext_psproductid,@dbsalesorderitemext_psmproductid,@dbsalesorderitemext_psoriprice,@dbsalesorderitemext_pssellcatid,@dbsalesorderitemext_psattribname,@dbsalesorderitemext_psmodelno,@dbsalesorderitemext_pscost,@dbsalesorderitemext_psfvf,@dbsalesorderitemext_psproducttype
OPEN mycursor
FETCH NEXT FROM mycursor INTO @dbsn,@dbitemlist_id,@dbitemlist_productid, @dbitemlist_sellingQty,@dbitem_id,@dbitem_productid,@dbitem_sellingQty, @dbitem_pricecoupon,@dbitem_name,@dbsalesorder_delivtype,@dbsalesorder_delivdata,@dbsalesorderitem_note,@dbsalesorderitem_price,@dbsalesorderitem_displayprice,@dbsalesorderitem_discountprice, @dbsalesorderitem_shippingexpense, @dbsalesorderitem_serviceexpense, @dbsalesorderitem_tax,@dbsalesorderitem_itempricesum,@dbsalesorderitem_installmentfee, @dbsalesorderitem_priceinst, @dbsalesorderitem_pricecoupon, @dbsalesorderitem_coupons, @dbsalesorderitem_redmbln, @dbsalesorderitem_redmtkout,@dbsalesorderitem_redmfdbck,@dbsalesorderitem_wfbln,@dbsalesorderitem_wftkout,@dbsalesorderitem_actid,@dbsalesorderitem_acttkout,@dbsalesorderitem_isnew,@dbitem_attribid,@dbitem_attribname, @dbitem_attribproductid,@dbsalesorders_note,@dbitemlist_attribid,@dbitemlist_attribname,@dbitemlist_attribproductid,@dbsalesorderitemext_psproductid,@dbsalesorderitemext_psmproductid,@dbsalesorderitemext_psoriprice,@dbsalesorderitemext_pssellcatid,@dbsalesorderitemext_psattribname,@dbsalesorderitemext_psmodelno,@dbsalesorderitemext_pscost,@dbsalesorderitemext_psfvf,@dbsalesorderitemext_psproducttype
WHILE (@@FETCH_STATUS = 0)
BEGIN
	--處理新增配件子單
	if(@dbitemlist_id=0)
		begin
			--主單
			--if( (@oldsalesorderitemext_psproductid<>@dbsalesorderitemext_psproductid  or ( @oldsalesorderitemext_psproductid='0' and @oldsalesorderitemext_psmproductid='0') ) and (@oldsalesorderitemext_psmproductid<>@dbsalesorderitemext_psmproductid) and (right(@dbsalesorderitemext_psmproductid,2)='_0'))
			if(@oldsalesorderitem_itemid<>@dbitem_id)
			begin
				TagA3:	
				set @dbsalesorder_code=dbo.fn_EC_GetSalesOrderAutoSN(@salesorderPrefix)
				
				if(@@ERROR<>0)
					begin
						set @run_repeat=@run_repeat+1
						if(@run_repeat<4)
							goto TagA3
						CLOSE mycursor
						DEALLOCATE mycursor
						ROLLBACK TRANSACTION
						set @dboutput='getsalesorderAutoSN is error.'
						return
					end
				set @run_repeat=0	
				
				TagB:
				insert SalesOrder
				( 
					Code,TelDay,InvoiceReceiver,InvoiceID,InvoiceTitle,InvoiceLoc,InvoiceZip,InvoiceAddr,Name,[Status],PayTypeID,PayType,Email,
					DelivLoc,DelivZip,DelivAddr,DelivEngAddr,IDno,Mobile,AccountID,RecvName,RecvEngName,RecvMobile,RecvtelDay,CardNo,CardType,CardBank,CardExpire,CardBirthday,CardLoc,CardZip,
					CardAddr,Note,DelivType,DelivData,RemoteIP,CoServerName,ServerName,AuthCode,AuthDate,AuthNote,UpdateUser,SalesOrderGroupID,[Date],CreateDate,UpdateDate
				) 
				values
				( 
					@dbsalesorder_code,@salesorder_telday,@salesorder_invoreceiver,@salesorder_invoid,@salesorder_invotitle,@salesorder_involoc,@salesorder_invozip,@salesorder_invoaddr,@salesorder_name,@salesorder_status,@salesorder_paytypeid,@salesorder_paytype,@salesorder_email,
					@salesorder_delivloc,@salesorder_delivzip,@salesorder_delivaddr,replace(@salesorder_delivengaddr,'repdot',','),@salesorder_idno,@salesorder_mobile,@salesorder_accountid,@salesorder_recvname,@salesorder_recvengname,@salesorder_recvmobile,@salesorder_recvtelday,@salesorder_cardno,@salesorder_cardtype,@salesorder_cardbank,@salesorder_cardexpire,@salesorder_cardbirthday,@salesorder_cardloc,@salesorder_cardzip,
					@salesorder_cardaddr,replace(@dbsalesorders_note,'repdot',','),@dbsalesorder_delivtype,@dbsalesorder_delivdata,@salesorder_remoteip,@salesorder_coservername,@salesorder_servername,@salesorder_authcode,@salesorder_authdate,@salesorder_authnote,@salesorder_updateuser,@salesorder_salesordergroupid,getdate(),getdate(),getdate()
				)
				
				if(@@ERROR<>0)
					begin
						set @run_repeat=@run_repeat+1
						if(@run_repeat<4)
							goto TagB
						CLOSE mycursor
						DEALLOCATE mycursor
						ROLLBACK TRANSACTION
						set @dboutput='insert salesorder is error.'
						return
					end
				set @run_repeat=0
			end	
			
		
			--主件的子單
			TagC:	
				set @get_salesorderitemcode=dbo.fn_EC_GetSalesOrderitemAutoSN(@salesorderitemPrefix)	
				if(@@ERROR<>0)
					begin
						set @run_repeat=@run_repeat+1
						if(@run_repeat<4)
							goto TagC
						CLOSE mycursor
						DEALLOCATE mycursor
						ROLLBACK TRANSACTION
						set @dboutput='getsalesorderitemAutoSN is error.'
						return
					end
				set @run_repeat=0
				
			
			TagD:
			
			if(@dbsalesorderitemext_psproducttype=1)
				begin
					if(@dbitem_attribid >0)
					begin
						insert salesorderitem 
						(
							Code,SalesOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,Priceinst,
							Price,DisplayPrice,DiscountPrice,ShippingExpense,ServiceExpense,Tax,ItemPriceSum,InstallmentFee, PriceCoupon, Coupons,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,Acttkout,IsNew,[Date],CreateDate,UpdateDate
						)
						values
						(
							@get_salesorderitemcode,@dbsalesorder_code,@dbitem_id,@dbitemlist_id,1,@dbitem_attribproductid,0,replace(@dbitem_name,'repdot',','),replace(@dbsalesorderitem_note,'repdot',','),@dbsalesorderitem_priceinst,@dbsalesorderitem_price,@dbsalesorderitem_displayprice,@dbsalesorderitem_discountprice,@dbsalesorderitem_shippingexpense,@dbsalesorderitem_serviceexpense,@dbsalesorderitem_tax,@dbsalesorderitem_itempricesum,@dbsalesorderitem_installmentfee, @dbsalesorderitem_pricecoupon, @dbsalesorderitem_coupons,replace(@dbitem_attribname,'repdot',','),@dbsalesorderitem_redmbln,@dbsalesorderitem_redmtkout,@dbsalesorderitem_redmfdbck,@dbsalesorderitem_wfbln,@dbsalesorderitem_wftkout,@dbsalesorderitem_actid,@dbsalesorderitem_acttkout,@dbsalesorderitem_isnew,getdate(),getdate(),getdate()
						)
					end  
					else
						begin
							insert salesorderitem 
							(
								Code,SalesOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,Priceinst,
								Price,DisplayPrice,DiscountPrice,ShippingExpense,ServiceExpense,Tax,ItemPriceSum,InstallmentFee, PriceCoupon,Coupons,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,Acttkout,IsNew,[Date],CreateDate,UpdateDate
							)	
							values
							(
								@get_salesorderitemcode,@dbsalesorder_code,@dbitem_id,@dbitemlist_id,1,@dbsalesorderitemext_psproductid,0,replace(@dbitem_name,'repdot',','),replace(@dbsalesorderitem_note,'repdot',','),@dbsalesorderitem_priceinst,@dbsalesorderitem_price,@dbsalesorderitem_displayprice,@dbsalesorderitem_discountprice,@dbsalesorderitem_shippingexpense,@dbsalesorderitem_serviceexpense,@dbsalesorderitem_tax,@dbsalesorderitem_itempricesum,@dbsalesorderitem_installmentfee, @dbsalesorderitem_pricecoupon,@dbsalesorderitem_coupons,replace(@dbsalesorders_note,'repdot',','),@dbsalesorderitem_redmbln,@dbsalesorderitem_redmtkout,@dbsalesorderitem_redmfdbck,@dbsalesorderitem_wfbln,@dbsalesorderitem_wftkout,@dbsalesorderitem_actid,@dbsalesorderitem_acttkout,@dbsalesorderitem_isnew,getdate(),getdate(),getdate()
							)
						end
				end
			else
				begin
					if(@dbsalesorderitemext_psproducttype=30)
						begin
							insert salesorderitem 
							(
								Code,SalesOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,Priceinst,
								Price,DisplayPrice,DiscountPrice,ShippingExpense,ServiceExpense,Tax,ItemPriceSum,InstallmentFee, PriceCoupon,Coupons,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,Acttkout,IsNew,[Date],CreateDate,UpdateDate
							)
							values
							(
								@get_salesorderitemcode,@dbsalesorder_code,@dbitem_id,@dbitemlist_id,1,'1155973',0,replace(@dbitem_name,'repdot',','),replace(@dbsalesorderitem_note,'repdot',','),@dbsalesorderitem_priceinst,@dbsalesorderitem_price,@dbsalesorderitem_displayprice,@dbsalesorderitem_discountprice,@dbsalesorderitem_shippingexpense,@dbsalesorderitem_serviceexpense,@dbsalesorderitem_tax,@dbsalesorderitem_itempricesum,@dbsalesorderitem_installmentfee, @dbsalesorderitem_pricecoupon,@dbsalesorderitem_coupons,'',@dbsalesorderitem_redmbln,@dbsalesorderitem_redmtkout,@dbsalesorderitem_redmfdbck,@dbsalesorderitem_wfbln,@dbsalesorderitem_wftkout,@dbsalesorderitem_actid,@dbsalesorderitem_acttkout,@dbsalesorderitem_isnew,getdate(),getdate(),getdate()
							)
						end
					else
						begin
							if(@dbitem_attribid >0)
								begin
									insert salesorderitem 
									(
										Code,SalesOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,Priceinst,
										Price,DisplayPrice,DiscountPrice,ShippingExpense,ServiceExpense,Tax,ItemPriceSum,InstallmentFee, PriceCoupon,Coupons,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,Acttkout,IsNew,[Date],CreateDate,UpdateDate
									)
									values
									(
										@get_salesorderitemcode,@dbsalesorder_code,@dbitem_id,@dbitemlist_id,1,@dbitem_attribproductid,0,replace(@dbitem_name,'repdot',','),replace(@dbsalesorderitem_note,'repdot',','),@dbsalesorderitem_priceinst,@dbsalesorderitem_price,@dbsalesorderitem_displayprice,@dbsalesorderitem_discountprice,@dbsalesorderitem_shippingexpense,@dbsalesorderitem_serviceexpense,@dbsalesorderitem_tax,@dbsalesorderitem_itempricesum,@dbsalesorderitem_installmentfee, @dbsalesorderitem_pricecoupon,@dbsalesorderitem_coupons,replace(@dbitem_attribname,'repdot',','),@dbsalesorderitem_redmbln,@dbsalesorderitem_redmtkout,@dbsalesorderitem_redmfdbck,@dbsalesorderitem_wfbln,@dbsalesorderitem_wftkout,@dbsalesorderitem_actid,@dbsalesorderitem_acttkout,@dbsalesorderitem_isnew,getdate(),getdate(),getdate()
									)
								end
							else
							begin
								insert salesorderitem 
								(
									Code,SalesOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,Priceinst,
									Price,DisplayPrice,DiscountPrice,ShippingExpense,ServiceExpense,Tax,ItemPriceSum,InstallmentFee, PriceCoupon,Coupons,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,Acttkout,IsNew,[Date],CreateDate,UpdateDate
								)
								values
								(
									@get_salesorderitemcode,@dbsalesorder_code,@dbitem_id,@dbitemlist_id,1,@dbitem_productid,0,replace(@dbitem_name,'repdot',','),replace(@dbsalesorderitem_note,'repdot',','),@dbsalesorderitem_priceinst,@dbsalesorderitem_price,@dbsalesorderitem_displayprice,@dbsalesorderitem_discountprice,@dbsalesorderitem_shippingexpense,@dbsalesorderitem_serviceexpense,@dbsalesorderitem_tax,@dbsalesorderitem_itempricesum,@dbsalesorderitem_installmentfee, @dbsalesorderitem_pricecoupon,@dbsalesorderitem_coupons,'',@dbsalesorderitem_redmbln,@dbsalesorderitem_redmtkout,@dbsalesorderitem_redmfdbck,@dbsalesorderitem_wfbln,@dbsalesorderitem_wftkout,@dbsalesorderitem_actid,@dbsalesorderitem_acttkout,@dbsalesorderitem_isnew,getdate(),getdate(),getdate()
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
					set @dboutput='insert salesorderitem is error.'
					return
				end
			set @run_repeat=0

			TagD3:
			insert salesorderitemext(SalesOrderitemCode,PsProductID,PsmProductID,PsOriPrice,PsSellcatID,PsAttribName,PsModelNO,PsCost,Psfvf) values(@get_salesorderitemcode,@dbsalesorderitemext_psproductid,@dbsalesorderitemext_psmproductid,@dbsalesorderitemext_psoriprice,@dbsalesorderitemext_pssellcatid,replace(@dbsalesorderitemext_psattribname,'repdot',','),@dbsalesorderitemext_psmodelno,@dbsalesorderitemext_pscost,@dbsalesorderitemext_psfvf)
			if(@@ERROR<>0)
				begin
					set @run_repeat=@run_repeat+1
					if(@run_repeat<4)
						goto TagD3
					CLOSE mycursor
					DEALLOCATE mycursor
					ROLLBACK TRANSACTION
					set @dboutput='insert salesorderitemext is error.'
					return
				end
			set @run_repeat=0
			
				
			TagU: --更新主件商品數量
				if(@dbitem_sellingQty>0)
					begin
					
						update item set Qtyreg=Qtyreg+1,Updated=Updated+1,UpdateUser=@salesorder_updateuser,UpdateDate=getdate()  where ID=@dbitem_id
					
				
						if(@@ERROR<>0)
							begin
								set @run_repeat=@run_repeat+1
								if(@run_repeat<4)
									goto TagU
								CLOSE mycursor
								DEALLOCATE mycursor
								ROLLBACK TRANSACTION
								set @dboutput='update item qty is error'
								return
							end
						set @run_repeat=0
					end 
				else
					begin
						CLOSE mycursor
							DEALLOCATE mycursor
							ROLLBACK TRANSACTION
							set @dboutput='item id=' + convert(nvarchar(1000),@dbitem_id) + ' qty is not enough.'
							return
					
					end
					
			TagX: --更新主件商品數量		
			   if(@dbitem_attribid >0)
				   begin
						update itemlist set Qtyreg=Qtyreg+1,Updated=Updated%255+1,UpdateUser=@salesorder_updateuser,Updatedate=getdate()  where ItemID=@dbitem_id and ID=@dbitem_attribid
					
						if(@@ERROR<>0)
							begin
								set @run_repeat=@run_repeat+1
								if(@run_repeat<4)
									goto TagX
								CLOSE mycursor
								DEALLOCATE mycursor
								ROLLBACK TRANSACTION
								set @dboutput='update item attribs productid qty is error'
								return
							end
						set @run_repeat=0
				   
				   
				   end 
		end
	else --配件單
		begin
			--新增配件
			TagE:
			set @get_salesorderitemcode=dbo.fn_EC_GetSalesOrderitemAutoSN(@salesorderitemPrefix)
			if(@@ERROR<>0)
				begin
					set @run_repeat=@run_repeat+1
					if(@run_repeat<4)
						goto TagE
					CLOSE mycursor
					DEALLOCATE mycursor
					ROLLBACK TRANSACTION
					set @dboutput='getsalesorderitemAutoSN is error.'
					return
				end
			set @run_repeat=0
			
			
			TagH:
			if(@dbitemlist_attribid>0)
				begin
					insert salesorderitem 
					(
						Code,SalesOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,Priceinst,
						Price,DisplayPrice,DiscountPrice,ShippingExpense,ServiceExpense,Tax,ItemPriceSum,InstallmentFee, PriceCoupon,Coupons,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,Acttkout,IsNew,[Date],CreateDate,UpdateDate
					)
					values
					(
						@get_salesorderitemcode,@dbsalesorder_code,@dbitem_id,@dbitemlist_id,1,0,@dbitemlist_attribproductid,replace(@dbitem_name,'repdot',','),replace(@dbsalesorderitem_note,'repdot',','),@dbsalesorderitem_priceinst,@dbsalesorderitem_price,@dbsalesorderitem_displayprice,@dbsalesorderitem_discountprice,@dbsalesorderitem_shippingexpense,@dbsalesorderitem_serviceexpense,@dbsalesorderitem_tax,@dbsalesorderitem_itempricesum,@dbsalesorderitem_installmentfee, @dbsalesorderitem_pricecoupon,@dbsalesorderitem_coupons,replace(@dbitemlist_attribname,'repdot',','),@dbsalesorderitem_redmbln,@dbsalesorderitem_redmtkout,@dbsalesorderitem_redmfdbck,@dbsalesorderitem_wfbln,@dbsalesorderitem_wftkout,@dbsalesorderitem_actid,@dbsalesorderitem_acttkout,@dbsalesorderitem_isnew,getdate(),getdate(),getdate()
					)
				end
			else
				begin
					insert salesorderitem 
					(
						Code,SalesOrderCode,ItemID,ItemListID,Qty,ProductID,ProductListID,Name,Note,Priceinst,
						Price,DisplayPrice,DiscountPrice,ShippingExpense,ServiceExpense,Tax,ItemPriceSum,InstallmentFee, PriceCoupon,Coupons,Attribs,Redmbln,Redmtkout,Redmfdbck,Wfbln,Wftkout,ActID,Acttkout,IsNew,[Date],CreateDate,UpdateDate
					)
					values
					(
						@get_salesorderitemcode,@dbsalesorder_code,@dbitem_id,@dbitemlist_id,1,0,@dbitemlist_productid,replace(@dbitem_name,'repdot',','),replace(@dbsalesorderitem_note,'repdot',','),@dbsalesorderitem_priceinst,@dbsalesorderitem_price,@dbsalesorderitem_displayprice,@dbsalesorderitem_discountprice,@dbsalesorderitem_shippingexpense,@dbsalesorderitem_serviceexpense,@dbsalesorderitem_tax,@dbsalesorderitem_itempricesum,@dbsalesorderitem_installmentfee, @dbsalesorderitem_pricecoupon,@dbsalesorderitem_coupons,replace(@dbsalesorderitemext_psattribname,'repdot',','),@dbsalesorderitem_redmbln,@dbsalesorderitem_redmtkout,@dbsalesorderitem_redmfdbck,@dbsalesorderitem_wfbln,@dbsalesorderitem_wftkout,@dbsalesorderitem_actid,@dbsalesorderitem_acttkout,@dbsalesorderitem_isnew,getdate(),getdate(),getdate()
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
					set @dboutput='insert salesorderitem is error.'
					return
				end
			set @run_repeat=0

			
			TagH3:
			insert salesorderitemext(SalesOrderItemCode,PsProductID,PsmProductID,PsOriPrice,PsSellcatID,PsAttribName,PsModelNo,PsCost,Psfvf) values(@get_salesorderitemcode,@dbsalesorderitemext_psproductid,@dbsalesorderitemext_psmproductid,@dbsalesorderitemext_psoriprice,@dbsalesorderitemext_pssellcatid,replace(@dbsalesorderitemext_psattribname,'repdot',','),@dbsalesorderitemext_psmodelno,@dbsalesorderitemext_pscost,@dbsalesorderitemext_psfvf)
			if(@@ERROR<>0)
				begin
					set @run_repeat=@run_repeat+1
					if(@run_repeat<4)
						goto TagH3
					CLOSE mycursor
					DEALLOCATE mycursor
					ROLLBACK TRANSACTION
					set @dboutput='insert salesorderitemext is error.'
					return
				end
			set @run_repeat=0
			
			
			--更新配件數量
			TagI:
				if(@dbitemlist_sellingQty>0) --可買數量>0
					begin
							update itemlist set Qtyreg=Qtyreg+1,Updated=Updated%255+1,UpdateUser=@salesorder_updateuser,UpdateDate=getdate()  where ID=@dbitemlist_id
						if(@@ERROR<>0)
							begin
								set @run_repeat=@run_repeat+1
								if(@run_repeat<4)
									goto TagI
								CLOSE mycursor
								DEALLOCATE mycursor
								ROLLBACK TRANSACTION
								set @dboutput='update itemlist qty is error.'
								return
							end
						set @run_repeat=0
					end
				else
					begin
							CLOSE mycursor
							DEALLOCATE mycursor
							ROLLBACK TRANSACTION
							set @dboutput='itemlist id=' + convert(nvarchar(15),@dbitemlist_id) + ' qty is not enough.'
							return
					end
					
				
			TagI2:
				if(@dbitemlist_attribid>0)
					begin
							update itemlist set Qtyreg=Qtyreg+1,Updated=Updated%255+1,UpdateUser=@salesorder_updateuser,UpdateDate=getdate()  where ID=@dbitemlist_attribid
						if(@@ERROR<>0)
							begin
								set @run_repeat=@run_repeat+1
								if(@run_repeat<4)
									goto TagI2
								CLOSE mycursor
								DEALLOCATE mycursor
								ROLLBACK TRANSACTION
								set @dboutput='update itemlist attribs qty is error.'
								return
							end
						set @run_repeat=0
					end
			
		end
		set @oldsalesorderitemext_psproductid=@dbsalesorderitemext_psproductid
		set @oldsalesorderitemext_psmproductid=@dbsalesorderitemext_psmproductid
		set @oldsalesorderitem_itemid=@dbitem_id
	FETCH NEXT FROM mycursor INTO @dbsn,@dbitemlist_id,@dbitemlist_productid, @dbitemlist_sellingQty,@dbitem_id,@dbitem_productid,@dbitem_sellingQty, @dbitem_pricecoupon,@dbitem_name,@dbsalesorder_delivtype,@dbsalesorder_delivdata,@dbsalesorderitem_note,@dbsalesorderitem_price,@dbsalesorderitem_displayprice,@dbsalesorderitem_discountprice, @dbsalesorderitem_shippingexpense, @dbsalesorderitem_serviceexpense, @dbsalesorderitem_tax,@dbsalesorderitem_itempricesum,@dbsalesorderitem_installmentfee, @dbsalesorderitem_priceinst, @dbsalesorderitem_pricecoupon,@dbsalesorderitem_coupons, @dbsalesorderitem_redmbln, @dbsalesorderitem_redmtkout,@dbsalesorderitem_redmfdbck,@dbsalesorderitem_wfbln,@dbsalesorderitem_wftkout,@dbsalesorderitem_actid,@dbsalesorderitem_acttkout,@dbsalesorderitem_isnew,@dbitem_attribid,@dbitem_attribname,@dbitem_attribproductid,@dbsalesorders_note,@dbitemlist_attribid,@dbitemlist_attribname,@dbitemlist_attribproductid,@dbsalesorderitemext_psproductid,@dbsalesorderitemext_psmproductid,@dbsalesorderitemext_psoriprice,@dbsalesorderitemext_pssellcatid,@dbsalesorderitemext_psattribname,@dbsalesorderitemext_psmodelno,@dbsalesorderitemext_pscost,@dbsalesorderitemext_psfvf,@dbsalesorderitemext_psproducttype

END

CLOSE mycursor
DEALLOCATE mycursor

COMMIT TRANSACTION 
--select O.*, I.*,salesorder_code,salesorderitem_code,salesorderitem_price,salesorderitem_priceinst,salesorder_salesordergroupid,salesorderitemext_psproductid,salesorderitemext_psmproductid from salesorder O with (nolock) inner join salesorderitem I with (nolock) on O.salesorder_code=I.salesorderitem_salesordercode inner join orderitemext with (nolock) on salesorderitemext_salesorderitemcode=salesorderitem_code where O.salesorder_salesordergroupid=@salesorder_salesordergroupid order by O.salesorder_code,I.salesorderitem_code

select  O.*
   ,I.Code as SalesorderItem_Code
      ,I.SalesorderCode as SalesorderItem_SalesorderCode
      ,I.ItemID as SalesorderItem_ItemID
      ,I.ItemlistID as SalesorderItem_ItemlistID
      ,I.ProductID as SalesorderItem_ProductID
      ,I.ProductlistID as SalesorderItem_ProductlistID
      ,I.Name as SalesorderItem_Name
      ,I.Price as SalesorderItem_Price
      ,I.Priceinst as SalesorderItem_Priceinst
      ,I.Qty as SalesorderItem_Qty
      ,I.Pricecoupon as SalesorderItem_Pricecoupon
	  ,I.Coupons as SalesorderItem_Coupons
      ,I.RedmtkOut as SalesorderItem_RedmtkOut
      ,I.RedmBLN as SalesorderItem_RedmBLN
      ,I.Redmfdbck as SalesorderItem_Redmfdbck
      ,I.[Status] as SalesorderItem_Status
      ,I.StatusNote as SalesorderItem_StatusNote
      ,I.[Date] as SalesorderItem_Date
      ,I.Attribs as SalesorderItem_Attribs
      ,I.Note as SalesorderItem_Note
      ,I.WftkOut as SalesorderItem_WftkOut
      ,I.WfBLN as SalesorderItem_WfBLN
      ,I.AdjPrice as SalesorderItem_AdjPrice
      ,I.ActID as SalesorderItem_ActID
      ,I.ActtkOut as SalesorderItem_ActtkOut
	  ,I.IsNew as SalesorderItem_IsNew
      ,I.ProdcutCostID as SalesorderItem_ProdcutCostID
      ,I.CreateUser as SalesorderItem_CreateUser
      ,I.CreateDate as SalesorderItem_CreateDate
      ,I.Updated as SalesorderItem_Updated
      ,I.UpdateDate as SalesorderItem_UpdateDate
      ,I.UpdateUser as SalesorderItem_UpdateUser
      ,I.DisplayPrice as SalesorderItem_DisplayPrice
      ,I.DiscountPrice as SalesorderItem_DiscountPrice
      ,I.ShippingExpense as SalesorderItem_ShippingExpense
	  ,I.ServiceExpense as SalesorderItem_ServiceExpense
	  ,I.Tax as SalesorderItem_Tax
      ,I.ItemPriceSum as SalesorderItem_ItemPriceSum
	  ,I.InstallmentFee as SalesorderItem_InstallmentFee
   ,E.[ID] as SalesorderItemExt_ID
      ,E.[SalesorderitemCode] as SalesorderItemExt_SalesorderitemCode
      ,E.[PsProductID] as SalesorderItemExt_PsProductID
      ,E.[PsmProductID] as SalesorderItemExt_PsmProductID
      ,E.[PsoriPrice] as SalesorderItemExt_PsoriPrice
      ,E.[PsSellcatID] as SalesorderItemExt_PsSellcatID
      ,E.[PsAttribName] as SalesorderItemExt_PsAttribName
      ,E.[PsModelNO] as SalesorderItemExt_PsModelNO
      ,E.[PsCost] as SalesorderItemExt_PsCost
      ,E.[Psfvf] as SalesorderItemExt_Psfvf from salesorder O with (nolock) inner join salesorderitem I with (nolock) on O.Code=I.SalesOrderCode inner join salesorderitemext E with (nolock) on E.SalesOrderItemCode=I.Code where O.SalesOrderGroupID=@salesorder_salesordergroupid order by O.Code,I.Code















GO
