
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create PROCEDURE CopyTempSales
	@SessionId uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;

	declare @SaleDate datetime
	declare @ManagerName varchar(100)
	declare @CustomerName varchar(255)
	declare @ProductName varchar(255)
	declare @Total float = 0	

    declare @ManagerId int
	declare @CustomerId int
	declare @ProductId int
	declare @IdentityTable table (Id int)
	
	BEGIN TRANSACTION Tr

	-- locks until we finish job..
	--select top 1 @ManagerId = Id
	--from Managers with (tablockx, holdlock)
	
	--select top 1 @ManagerId = Id
	--from Customers with (tablockx, holdlock)
	
	--select top 1 @ManagerId = Id
	--from Products with (tablockx, holdlock)

	set @ManagerId = 0
	
	BEGIN TRY

		declare Cur cursor fast_forward
		for
		select t.SaleDate, t.ManagerName, t.CustomerName, t.ProductName, t.Total
		from TempSales t
		where t.SessionId = @SessionId

		open Cur

		fetch next from Cur
		into @SaleDate, @ManagerName, @CustomerName, @ProductName, @Total

		while @@FETCH_STATUS = 0
		begin
			set @ManagerId = 0
			set @CustomerId = 0
			set @ProductId = 0

			select @ManagerId = m.Id
			from Managers m
			where m.LastName = @ManagerName

			select @CustomerId = c.Id
			from Customers c
			where c.CustomerName = @CustomerName

			select @ProductId = p.Id
			from Products p
			where p.ProductName = @ProductName	

			if (@ManagerId = 0)
			begin
				insert into Managers (LastName)
				  output inserted.Id into @IdentityTable
				values (@ManagerName)

				select @ManagerId = Id 
				from @IdentityTable
			end

			if (@CustomerId = 0)
			begin
				insert into Customers (CustomerName)
				  output inserted.Id into @IdentityTable
				values (@CustomerName)

				select @CustomerId = Id 
				from @IdentityTable
			end

			if (@ProductId = 0)
			begin
				insert into Products (ProductName, Price)
				  output inserted.Id into @IdentityTable
				values (@ProductName, @Total)

				select @ProductId = Id 
				from @IdentityTable
			end
		 
			insert into Sales (SaleDate, ManagerId, CustomerId, ProductId, Total)
			values (@SaleDate, @ManagerId, @CustomerId, @ProductId, @Total)


			fetch next from Cur
			into @SaleDate, @ManagerName, @CustomerName, @ProductName, @Total
		end
	
		close Cur
		deallocate Cur	
		
		delete from TempSales
		where SessionId = @SessionId

		COMMIT TRANSACTION Tr

		select 0 as ErrorNumber, '' as ErrorMessage
	END TRY
	BEGIN CATCH		
		close Cur
		deallocate Cur
		
		ROLLBACK TRANSACTION Tr				
		
		select ERROR_NUMBER() as ErrorNumber, ERROR_MESSAGE() as ErrorMessage
	END CATCH
		

END
GO
