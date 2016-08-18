--SELECT A.ID,A.Title,A.Description,A.ClassName,B.Title,C.*
--FROM TWSQLDB.dbo.category AS A
--JOIN TWSQLDB.dbo.SubCategory_NormalStore AS B ON A.ID = B.CategoryID
--JOIN TWSQLDB.dbo.ItemAndSubCategoryMapping_NormalStore AS C ON B.ID = C.SubCategoryID
--ORDER BY B.CategoryID,C.SubCategoryID

select * from TWSQLDB_PRD.dbo.SubCategory_NormalStore
where 0=0
--and UpdateUser='bh96'
order by UpdateDate desc