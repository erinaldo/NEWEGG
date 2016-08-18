select * FROM TWSQLDB.dbo.advertising
select * FROM TWSQLDB.dbo.SubCategory_NormalStore
select * FROM TWSQLDB.dbo.subcategorylogo


--SELECT  COLLATIONPROPERTY('Chinese_PRC_Stroke_CI_AI_KS_WS', 'CodePage')
sp_helpdb TWSQLDB


SELECT * FROM Information_Schema.COLUMNS

SELECT distinct(TABLE_NAME) FROM Information_Schema.COLUMNS

--IPP
use TWBACKENDDB
SELECT count(distinct(TABLE_NAME)) FROM Information_Schema.COLUMNS

--EC
use TWSQLDB
SELECT count(distinct(TABLE_NAME)) FROM Information_Schema.COLUMNS

--¨ÑÀ³°Ó
use TWSELLERPORTALDB
SELECT count(distinct(TABLE_NAME)) FROM Information_Schema.COLUMNS