/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1000 [ID]
      ,[Name]
      ,[ShowMenu]
      ,[URL]
      ,[FuncmenuRight]
      ,[CreateDate]
      ,[CreateUser]
      ,[Updated]
      ,[UpdateUser]
      ,[UpdateDate]
      ,[classification]
  FROM [TWBACKENDDB_PRD].[dbo].[funcmenu]
  --update [TWBACKENDDB_PRD].[dbo].[funcmenu] set classification='8'
  where 0=0
  --and ID=216
  and classification='8'
  
  