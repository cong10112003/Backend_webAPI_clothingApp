CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(255) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Description NVARCHAR(MAX),
    Rate DECIMAL(2, 1) CHECK (Rate >= 0 AND Rate <= 5)
);

CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(255) NOT NULL
);

-- Bảng trung gian để liên kết nhiều-nhiều giữa Categories và Products
CREATE TABLE ProductCategories (
    CategoryID INT FOREIGN KEY REFERENCES Categories(CategoryID) ON DELETE CASCADE,
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID) ON DELETE CASCADE,
    PRIMARY KEY (CategoryID, ProductID)
);

CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    CustomerName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(15) NOT NULL,
    BirthYear INT CHECK (BirthYear > 1900 AND BirthYear <= YEAR(GETDATE())),
    Email NVARCHAR(255)
);

CREATE TABLE Accounts (
    AccountID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID) ON DELETE CASCADE -- Mỗi tài khoản liên kết với 1 khách hàng
);

-- Bảng giỏ hàng cho mỗi khách hàng, mỗi khách hàng chỉ có một giỏ hàng duy nhất
CREATE TABLE Carts (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT UNIQUE FOREIGN KEY REFERENCES Customers(CustomerID) ON DELETE CASCADE, -- Mỗi khách hàng có 1 giỏ hàng
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Bảng trung gian để liên kết nhiều-nhiều giữa Carts và Products
CREATE TABLE CartProducts (
    CartID INT FOREIGN KEY REFERENCES Carts(CartID) ON DELETE CASCADE,
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID) ON DELETE CASCADE,
    Quantity INT NOT NULL CHECK (Quantity > 0), -- Số lượng của sản phẩm trong giỏ hàng
    PRIMARY KEY (CartID, ProductID)
);

-- Bảng lịch sử đơn hàng của từng khách hàng
CREATE TABLE OrderHistory (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID) ON DELETE CASCADE, -- Mỗi khách hàng có lịch sử đơn hàng riêng
    OrderDate DATETIME DEFAULT GETDATE()
);

-- Bảng trung gian để liên kết nhiều-nhiều giữa OrderHistory và Products
CREATE TABLE OrderProducts (
    OrderID INT FOREIGN KEY REFERENCES OrderHistory(OrderID) ON DELETE CASCADE,
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID) ON DELETE CASCADE,
    Quantity INT NOT NULL CHECK (Quantity > 0), -- Số lượng sản phẩm trong đơn hàng
    PriceAtPurchase DECIMAL(18, 2) NOT NULL, -- Lưu giá tại thời điểm đặt hàng
    PRIMARY KEY (OrderID, ProductID)
);