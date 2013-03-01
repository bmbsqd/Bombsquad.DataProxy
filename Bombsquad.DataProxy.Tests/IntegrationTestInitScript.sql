if object_id('dbo.UserMessages') is not null
begin
  drop table dbo.UserMessages
end

if object_id('dbo.Users') is not null
begin
  drop table dbo.Users
end

if object_id('dbo.UsersList') is not null
begin
  drop proc dbo.UsersList
end

if object_id('dbo.UsersListUserIds') is not null
begin
  drop proc dbo.UsersListUserIds
end

if object_id('dbo.UsersAdd') is not null
begin
  drop proc dbo.UsersAdd
end

if object_id('dbo.OutputParameterTest') is not null
begin
  drop proc dbo.OutputParameterTest
end

if object_id('dbo.DataTypesTest') is not null
begin
	drop proc dbo.DataTypesTest
end

if object_id('dbo.UsersFindByUserIds') is not null
begin
	drop proc dbo.UsersFindByUserIds
end

if object_id('dbo.UsersAddMultiple') is not null
begin
	drop proc dbo.UsersAddMultiple
end

if object_id('dbo.UsersFullFindById') is not null
begin
	drop proc dbo.UsersFullFindById
end

if type_id('dbo.IntegerList') is not null
begin
	drop type dbo.IntegerList
end

if type_id('dbo.UserList') is not null
begin
	drop type dbo.UserList
end

create table dbo.Users
(
	UserId int identity(1,1) not null,
	FirstName varchar(64) not null,
	LastName varchar(64) not null,
	EmailAddress varchar(128) not null,
	BirthDate smalldatetime null,
    constraint PK_Users primary key clustered ( UserId asc )
)

create table dbo.UserMessages
(
	MessageId int identity(1,1) not null,
	ToUserId int not null,
	FromUserId int not null,
	SentDate datetime not null,
	[Subject] varchar(256) not null,
	Body text not null,
	constraint PK_UserMessages primary key clustered ( MessageId asc )
)

alter table dbo.UserMessages with check add constraint FK_UserMessages_Users_ToUserId foreign key (ToUserId)
references dbo.Users (UserId)

alter table dbo.UserMessages with check add constraint FK_UserMessages_Users_FromUserId foreign key (FromUserId)
references dbo.Users (UserId)

insert into dbo.Users(FirstName, LastName, EmailAddress, BirthDate)
values('Rolf-Göran', 'Bengtsson', 'rbg@hotmail.com', '1948-05-01')

insert into dbo.Users(FirstName, LastName, EmailAddress, BirthDate)
values('Nils-Petter', 'Sundgren', 'nisse-p@gmail.com', NULL)

insert into dbo.UserMessages(ToUserId, FromUserId, SentDate, [Subject], Body)
values(1, 2, getdate(), 'Hello my friend!', 'Hello Roffe! I hope you are well. Cheers!')

GO

create procedure dbo.UsersListUserIds
as
begin
	select UserId from dbo.Users order by UserId
end

GO

create procedure dbo.UsersList
as
begin
	select * from dbo.Users order by UserId
end

GO

create procedure dbo.UsersAdd
(
	@FirstName varchar(64),
	@LastName varchar(64),
	@EmailAddress varchar(128),
	@BirthDate smalldatetime = null
)
as
begin
	insert into dbo.Users(FirstName, LastName, EmailAddress, BirthDate)
	values(@FirstName, @LastName, @EmailAddress, @BirthDate);

	select cast(scope_identity() as int)
end

GO

create procedure dbo.OutputParameterTest
(
	@Input int,
	@Output int output
)
as
begin
	set @Output = @Input * 2;
end

GO

create type dbo.IntegerList as table
(
	Value int
);

GO

create type dbo.UserList as table
(
	FirstName varchar(64),
	LastName varchar(64),
	EmailAddress varchar(128),
	BirthDate smalldatetime null
);

GO

create procedure dbo.UsersFindByUserIds
(
	@UserIds dbo.IntegerList readonly
)
as
begin
	select *
	from @UserIds uids
	inner join dbo.Users u
	on uids.Value = u.UserId
end

GO

create procedure dbo.UsersAddMultiple
(
	@Users dbo.UserList readonly
)
as
begin
	insert into dbo.Users(FirstName, LastName, EmailAddress, BirthDate)
	select FirstName, LastName, EmailAddress, BirthDate
	from @Users
end

GO

create procedure dbo.UsersFullFindById
(
	@UserId int
)
as
begin
	select * from dbo.Users
	where UserId = @UserId

	select * from dbo.UserMessages
	where ToUserId = @UserId
end

GO

create procedure dbo.DataTypesTest
(
	@BitValue bit = null,
	@TinyIntValue tinyint = null,
	@SmallIntValue smallint = null,
	@IntValue int = null,
	@BigIntValue bigint = null,
	@RealValue real = null,
	@FloatValue float = null,
	@DecimalValue decimal(18,3) = null,
	@DateTimeValue datetime = null,
	@StringValue nvarchar(128) = null,
	@GuidValue uniqueidentifier = null
)
as
begin
	select @BitValue as BitValue,
		@TinyIntValue as TinyIntValue,
		@SmallIntValue as SmallIntValue,
		@IntValue as IntValue,
		@BigIntValue as BigIntValue,
		@RealValue as RealValue,
		@FloatValue as FloatValue,
		@DecimalValue as DecimalValue,
		@DateTimeValue as DateTimeValue,
		@StringValue as StringValue,
		@GuidValue as GuidValue	
end