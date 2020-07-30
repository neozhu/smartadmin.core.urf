USE [smartdb]
GO

/****** Object:  StoredProcedure [dbo].[SP_NextVal]    Script Date: 2020/7/30 8:59:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_NextVal](@prefix nvarchar(10))
as
begin  
declare @val int=1;
IF NOT EXISTS (SELECT * FROM [dbo].[Sequence] WHERE prefix = @prefix)
 begin
  begin try
      INSERT INTO [dbo].[Sequence]
        (prefix, seed)
      VALUES (@prefix, @val);
      select @val
  end try
  begin catch
    update [dbo].[Sequence] set seed=seed+1,@val=seed+1 where prefix = @prefix
    select @val
  end catch
 end
else
 begin
 --WAITFOR DELAY '00:00:10.000';
 update [dbo].[Sequence] set seed=seed+1,@val=seed+1 where prefix = @prefix
 select @val
 end
end 
GO


