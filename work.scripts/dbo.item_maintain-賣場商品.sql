--dbo.item_maintain.sql
use TWSQLDB_PRD

declare @item_id varchar(50) ='140'; --140,472077,483231
--declare @category_id varchar(50) ='403';
--declare @itemtemp_id varchar(50) ='526032';

--����~
select 'item'
,ID
,SellerID --������
,ShowOrder --�[����
,ProductID --�n�檺�ӫ~
,CategoryID --�D����
,*
 from dbo.item
where ID=@item_id


--����~�D����
select 'ItemPropertyGroup', * from ItemPropertyGroup g, item b
where g.CategoryID = b.CategoryID
and b.ID=@item_id

--����~�ݩ�, �O�� �~�P �t�C ���� �~�P �e�q ���� �ؤo ���� ��
select 'ItemPropertyName', * from ItemPropertyName where GroupID in(
	--����~�D����
	select g.ID from ItemPropertyGroup g, item b
	where g.CategoryID = b.CategoryID
	and b.ID=@item_id
) 


--����~�ݩʭ�, one to one from ItemPropertyName
select 'ItemPropertyValue', v.* from ItemPropertyValue v , ItemPropertyName n 
where v.PropertyNameID = n.ID
and n.GroupID in(
	--����~�D����
	select g.ID from ItemPropertyGroup g, item b
	where g.CategoryID = b.CategoryID
	and b.ID=@item_id
)

--�ӫ~�ݩ�, ����~�ݩʭȦۭq�ɰO��
--��PropertyNameID�o���Ӧ�ItemPropertyValue.PropertyNameID
select 'ProductProperty', pp.* from  ProductProperty pp, item i
where pp.ProductID = i.ProductID
and i.ID =@item_id


--����~�f��
--���ز��ʮɦb���f��, item�Pitemtemp�P�ɦs�b
select 'itemtemp', * from itemtemp
where ItemID=@item_id

--������  
--����~��������
select N'����~��������' , ItemID, i.SellerID, s.Name from salesorderitem soi with (nolock) , item i , seller s
where 0=0
and soi.ItemID = i.ID
and s.ID = i.SellerID
and soi.ItemID=@item_id

