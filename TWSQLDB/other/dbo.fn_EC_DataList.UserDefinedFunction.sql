USE [TWSQLDB]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_EC_DataList]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[fn_EC_DataList] (@name nvarchar(20),@list nvarchar(4000))
   RETURNS @tbl TABLE (name nvarchar(20) NOT NULL,num nvarchar(200) NOT NULL,sn char(3) NOT NULL) AS
BEGIN
   DECLARE @pos        int
          ,@nextpos    int
          ,@valuelen   int
          ,@n int

   SET @n=0

   SELECT 
       @pos = 0
      ,@nextpos = 1

   WHILE @nextpos > 0
   BEGIN
      SELECT 
          @nextpos = charindex(',', @list
         ,@pos + 1
   )
      SELECT 
          @valuelen = CASE 
              WHEN @nextpos > 0  THEN @nextpos 
              ELSE len(@list) + 1 
          END - @pos - 1
	  SET @n=@n+1
      INSERT @tbl 
      (
           sn
          ,name
          ,num
      )
      VALUES 
      (
           @n
          ,@name
          ,SUBSTRING(@list, @pos + 1, @valuelen)
      )
      SELECT 
          @pos = @nextpos
   END
  RETURN
END


GO
