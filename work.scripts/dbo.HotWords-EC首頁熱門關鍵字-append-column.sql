use TWSQLDB
--use TWSQLDB_PRD
select * from dbo.HotWords
where 0=0
--and ShowAll=1
order by Showorder

----append schema by bruce------------------from BSATW-172 問候卡, BSATW-178 進站問候語模組

--ALTER TABLE  dbo.HotWords
----ALTER COLUMN Name nvarchar(50) --名稱
--ADD Name nvarchar(50)

----MemoCode 自訂代碼文字(new add 20160324)
--ALTER TABLE  dbo.HotWords  ADD MemoCode nvarchar(50)
--ALTER TABLE  dbo.HotWords ALTER COLUMN MemoCode nvarchar(50)
--ALTER TABLE  dbo.HotWords DROP COLUMN MemoCode

----CategoryId
----0 首頁熱門關鍵字
----1 登入問候語
----2 節日問候卡

----Images Uniform Resource Locator 圖片位置
----CategoryId是2是稱為 活動素材(new add 20160324)
--ALTER TABLE  dbo.HotWords
----ALTER COLUMN ImagesURL nvarchar(100)
--ADD ImagesURL nvarchar(100) 

--ALTER TABLE  dbo.HotWords
--ALTER COLUMN [Description] nvarchar(50)

----ALTER TABLE  dbo.HotWords DROP COLUMN MemoCode











