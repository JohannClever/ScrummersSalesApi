-- ==========================
-- Table: Orders
-- ==========================
CREATE TABLE dbo.Orders (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    Status INT NOT NULL, -- maps to OrderStatus enum
    TotalAmount DECIMAL(18,2) NOT NULL CHECK (TotalAmount >= 0),
    CreateDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UpdateDate DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

-- ==========================
-- Table: OrderItems
-- ==========================
CREATE TABLE dbo.OrderItems (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    OrderId UNIQUEIDENTIFIER NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18,2) NOT NULL CHECK (UnitPrice > 0),

    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId)
        REFERENCES dbo.Orders(Id) ON DELETE CASCADE
);
