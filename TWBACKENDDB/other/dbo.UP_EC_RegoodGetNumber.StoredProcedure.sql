USE [TWBACKENDDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_RegoodGetNumber]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[UP_EC_RegoodGetNumber]
AS
Declare @get_GetRegoodcode nvarchar(15)
Declare @run_repeat int
set @run_repeat=0
Declare @dboutput nvarchar(50)
set @dboutput=''




	set @get_GetRegoodcode=dbo.fn_EC_GetRegoodAutoSN('LBR')
select @get_GetRegoodcode as RegoodID

				/*if(@@ERROR<>0)
					begin
						set @run_repeat=@run_repeat+1
						
						
						CLOSE mycursor
						DEALLOCATE mycursor
						ROLLBACK TRANSACTION
						set @dboutput='getsalesorderitemAutoSN is error.'
						return 
					end
				set @run_repeat=0*/









GO
