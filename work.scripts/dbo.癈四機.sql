--dbo.�u�|��

--dbo.item_maintain.sql
use TWSQLDB_PRD

declare @item_id varchar(50) ='125523'; --140,472077,125523
--declare @category_id varchar(50) ='403';
--declare @itemtemp_id varchar(50) ='526032';

--����~
select N'����~'
,ID
,SellerID --������
,ShowOrder --�[����
,ProductID --�n�檺�ӫ~
,CategoryID --�D����
,*
 from dbo.item
where ID=@item_id


--����~�D����
select N'����~�D����', * from ItemPropertyGroup g, item b
where g.CategoryID = b.CategoryID
and b.ID=@item_id

--����~�ݩ�, �O�� �~�P �t�C ���� �~�P �e�q ���� �ؤo ���� ��
select N'����~�ݩ�', * from ItemPropertyName where GroupID in(
	--����~�D����
	select g.ID from ItemPropertyGroup g, item b
	where g.CategoryID = b.CategoryID
	and b.ID=@item_id
) 


--����~�ݩʭ�, one to one from ItemPropertyName
select N'����~�ݩʭ�', v.* from ItemPropertyValue v , ItemPropertyName n 
where v.PropertyNameID = n.ID
and n.GroupID in(
	--����~�D����
	select g.ID from ItemPropertyGroup g, item b
	where g.CategoryID = b.CategoryID
	and b.ID=@item_id
)

--�ӫ~�ݩ�, ����~�ݩʭȦۭq�ɰO��
--��PropertyNameID�o���Ӧ�ItemPropertyValue.PropertyNameID
select N'�ӫ~�ݩ�, ����~�ݩʭȦۭq�ɰO��', pp.* from  ProductProperty pp, item i
where pp.ProductID = i.ProductID
and i.ID =@item_id


--����~�f��
--���ز��ʮɦb���f��, item�Pitemtemp�P�ɦs�b
select N'����~�f��', * from itemtemp
where ItemID=@item_id


--����~��������
select N'����~��������' , ItemID, i.SellerID, s.Name from salesorderitem soi with (nolock) , item i , seller s
where 0=0
and soi.ItemID = i.ID
and s.ID = i.SellerID
and soi.ItemID=@item_id
