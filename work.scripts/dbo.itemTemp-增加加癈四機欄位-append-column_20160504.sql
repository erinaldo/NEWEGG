--dbo.廢四機

--use TWSQLDB_PRD
use TWSQLDB

--Fail4Recove 增加廢四機欄位, new add 20160428
ALTER TABLE dbo.itemTemp
ADD Discard4 nvarchar(10) NULL 
CONSTRAINT DF_itemTemp_Discard4
default ('')
--ALTER TABLE dbo.itemTemp  ALTER COLUMN Discard4 nvarchar(10) NULL 
--ALTER TABLE dbo.itemTemp  DROP COLUMN Discard4

--select count(*) from dbo.ItemSketch
--where dbo.ItemSketch.Discard4 is null
----update dbo.ItemSketch set Discard4=''


--SELECT * FROM sys.extended_properties
--加入欄位說明
EXEC sp_addextendedproperty 
--EXEC sp_updateextendedproperty  
    @name = N'MS_Description'
    ,@value = N'廢四機商品, Y=是廢四機'
    ,@level0type = N'Schema', @level0name = dbo
    ,@level1type = N'Table',  @level1name = itemTemp
    ,@level2type = N'Column', @level2name = Discard4;
GO

--查詢已建立的說明
SELECT * FROM sys.extended_properties
where 0=0
and name=N'MS_Description'
and value=N'廢四機商品, Y=是廢四機'


