--dbo.癈四機

--dbo.item_maintain.sql
use TWSQLDB_PRD

declare @item_id varchar(50) ='125523'; --140,472077,125523
--declare @category_id varchar(50) ='403';
--declare @itemtemp_id varchar(50) ='526032';

--賣場品
select N'賣場品'
,ID
,SellerID --供應商
,ShowOrder --加價購
,ProductID --要賣的商品
,CategoryID --主分類
,*
 from dbo.item
where ID=@item_id


--賣場品主分類
select N'賣場品主分類', * from ItemPropertyGroup g, item b
where g.CategoryID = b.CategoryID
and b.ID=@item_id

--賣場品屬性, 記錄 品牌 系列 型號 品牌 容量 介面 尺寸 顆粒 等
select N'賣場品屬性', * from ItemPropertyName where GroupID in(
	--賣場品主分類
	select g.ID from ItemPropertyGroup g, item b
	where g.CategoryID = b.CategoryID
	and b.ID=@item_id
) 


--賣場品屬性值, one to one from ItemPropertyName
select N'賣場品屬性值', v.* from ItemPropertyValue v , ItemPropertyName n 
where v.PropertyNameID = n.ID
and n.GroupID in(
	--賣場品主分類
	select g.ID from ItemPropertyGroup g, item b
	where g.CategoryID = b.CategoryID
	and b.ID=@item_id
)

--商品屬性, 賣場品屬性值自訂時記錄
--由PropertyNameID得知來自ItemPropertyValue.PropertyNameID
select N'商品屬性, 賣場品屬性值自訂時記錄', pp.* from  ProductProperty pp, item i
where pp.ProductID = i.ProductID
and i.ID =@item_id


--賣場品審核
--項目異動時在此審核, item與itemtemp同時存在
select N'賣場品審核', * from itemtemp
where ItemID=@item_id


--賣場品的供應商
select N'賣場品的供應商' , ItemID, i.SellerID, s.Name from salesorderitem soi with (nolock) , item i , seller s
where 0=0
and soi.ItemID = i.ID
and s.ID = i.SellerID
and soi.ItemID=@item_id
