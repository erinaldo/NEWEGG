USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GetSalesOrderNumByDate]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UP_EC_GetSalesOrderNumByDate] 
       @accunt_id int
      ,@outnum int OUT  
AS
BEGIN

set @outnum=1
SELECT TOP 1 
    @outnum=OrderNum+1 
FROM salesorder S WITH (NOLOCK) 
INNER JOIN salesordergroup G WITH (NOLOCK) 
    ON G.ID=S.SalesOrderGroupID 
WHERE 
    AccountID= @accunt_id 
ORDER BY SalesOrderGroupID DESC

	
	
END



GO
