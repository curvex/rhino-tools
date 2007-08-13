-- DISCLAIMER - this is a DB that is meant to challange an ETL tool, not a good DB

if object_id('Users_Source') is not null drop table Users_Source
if object_id('Users_Destination') is not null drop table Users_Destination
if object_id('Users2Org') is not null drop table Users2Org
if object_id('Orders') is not null drop table Orders
if object_id('OrdersWareHousing') is not null drop table OrdersWareHousing

CREATE TABLE Users_Source
(
	[Id] int identity primary key,
	[Name] nvarchar(50) not null,
	[Email] nvarchar(255) not null
)

CREATE TABLE [dbo].[OrdersWareHousing](
	[OrderId] [int] NOT NULL,
	[CompanyName] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ContactName] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE Users2Org
(
	[UserId] nvarchar(25),
	[organization id] int
)

CREATE TABLE Users_Destination
(
	UserId nvarchar(20) not null primary key,
	[First Name] nvarchar(25),
	[Last Name] nvarchar(25),
	Email nvarchar(255),
	Orgnaization int 
)

CREATE TABLE [dbo].[Orders](
	[OrderID] [int] NOT NULL,
	[CustomerID] [nchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EmployeeID] [int] NULL,
	[OrderDate] [datetime] NULL,
	[RequiredDate] [datetime] NULL,
	[ShippedDate] [datetime] NULL,
	[ShipVia] [int] NULL,
	[Freight] [money] NULL CONSTRAINT [DF_Orders_Freight]  DEFAULT ((0)),
	[ShipName] [nvarchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShipAddress] [nvarchar](60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShipCity] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShipRegion] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShipPostalCode] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShipCountry] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO "Orders"
("OrderID","CustomerID","EmployeeID","OrderDate","RequiredDate",
	"ShippedDate","ShipVia","Freight","ShipName","ShipAddress",
	"ShipCity","ShipRegion","ShipPostalCode","ShipCountry")
VALUES (10248,N'VINET',5,'7/4/1996','8/1/1996','7/16/1996',3,32.38,
	N'Vins et alcools Chevalier',N'59 rue de l''Abbaye',N'Reims',
	NULL,N'51100',N'France')
INSERT INTO "Orders"
("OrderID","CustomerID","EmployeeID","OrderDate","RequiredDate",
	"ShippedDate","ShipVia","Freight","ShipName","ShipAddress",
	"ShipCity","ShipRegion","ShipPostalCode","ShipCountry")
VALUES (10249,N'TOMSP',6,'7/5/1996','8/16/1996','7/10/1996',1,11.61,
	N'Toms Spezialitäten',N'Luisenstr. 48',N'Münster',
	NULL,N'44087',N'Germany')
INSERT INTO "Orders"
("OrderID","CustomerID","EmployeeID","OrderDate","RequiredDate",
	"ShippedDate","ShipVia","Freight","ShipName","ShipAddress",
	"ShipCity","ShipRegion","ShipPostalCode","ShipCountry")
VALUES (10250,N'HANAR',4,'7/8/1996','8/5/1996','7/12/1996',2,65.83,
	N'Hanari Carnes',N'Rua do Paço, 67',N'Rio de Janeiro',
	N'RJ',N'05454-876',N'Brazil')

INSERT INTO Users_Source
SELECT 'Nancy Davolio', 'Seattle@USA.com' UNION ALL
SELECT 'Andrew Fuller', 'Tacoma@USA.com' UNION ALL
SELECT 'Janet Leverling', 'Kirkland@USA.com' UNION ALL
SELECT 'Margaret Peacock', 'Bad Email' UNION ALL
SELECT 'Steven Buchanan', 'London@UK.com' UNION ALL
SELECT 'Michael Suyama', 'London@UK.com' UNION ALL
SELECT 'Robert King', 'London@UK.com' UNION ALL
SELECT 'Laura Callahan', 'Seattle@USA.com' UNION ALL
SELECT 'Anne Dodsworth', 'London@UK.com'

INSERT INTO Users2Org
SELECT '1', 432 UNION ALL
SELECT '2', 332 UNION ALL
SELECT '3', 232 UNION ALL
SELECT '4', 132 

