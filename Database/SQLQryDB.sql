CREATE DATABASE [dbCoba]
GO

USE [dbCoba]

--USE [master]
--DROP DATABASE [dbCoba]

GO

CREATE TABLE [dbo].[PurchaseOrderDetails](
	[DetailID] [nvarchar](5) NOT NULL,
	[PurchaseOrderID] [nvarchar](5) NOT NULL,
	[ItemName] [nvarchar](100) NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[LineTotal]  AS ([Quantity]*[UnitPrice]) PERSISTED,
PRIMARY KEY CLUSTERED 
(
	[DetailID] ASC,
	[PurchaseOrderID] ASC
))
GO

CREATE TABLE [dbo].[PurchaseOrders](
	[PurchaseOrderID] [nvarchar](5) NOT NULL,
	[SupplierName] [nvarchar](100) NOT NULL,
	[OrderDate] [date] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderID] ASC
))
GO

CREATE TABLE [dbo].[tbl_ProductGroups]
(
	[ProductGrp] [nvarchar](10) NOT NULL,
	[ProductGrp_name] [nvarchar](100) NOT NULL
Primary key clustered
(
	[ProductGrp] ASC
))
GO

CREATE TABLE [dbo].[tbl_Products](
	[ProductID] [nvarchar](6) NOT NULL,
	[ProductName] [nvarchar](100) NOT NULL,
	[ProductQty] [decimal](18, 2) NOT NULL,
	[ProductUnit] [nvarchar](20) NULL,
	[ProductPrice] [decimal](18, 2) NULL,
	[ProductGrp] [nvarchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
))
GO

ALTER TABLE [dbo].[PurchaseOrderDetails]  WITH CHECK ADD FOREIGN KEY([PurchaseOrderID])
REFERENCES [dbo].[PurchaseOrders] ([PurchaseOrderID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tbl_Products] ADD FOREIGN KEY([ProductGrp])
REFERENCES [dbo].[tbl_ProductGroups] ([ProductGrp])
GO