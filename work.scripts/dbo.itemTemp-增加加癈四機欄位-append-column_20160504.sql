--dbo.�o�|��

--use TWSQLDB_PRD
use TWSQLDB

--Fail4Recove �W�[�o�|�����, new add 20160428
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
--�[�J��컡��
EXEC sp_addextendedproperty 
--EXEC sp_updateextendedproperty  
    @name = N'MS_Description'
    ,@value = N'�o�|���ӫ~, Y=�O�o�|��'
    ,@level0type = N'Schema', @level0name = dbo
    ,@level1type = N'Table',  @level1name = itemTemp
    ,@level2type = N'Column', @level2name = Discard4;
GO

--�d�ߤw�إߪ�����
SELECT * FROM sys.extended_properties
where 0=0
and name=N'MS_Description'
and value=N'�o�|���ӫ~, Y=�O�o�|��'


