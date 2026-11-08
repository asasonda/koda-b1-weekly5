create DATABASE Ewallet

CREATE TABLE Users(
    Id int IDENTITY(1,1) PRIMARY key,
    Name NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) not null,
    Phone NVARCHAR(20) not NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
)

CREATE Table Wallets(
    Id int IDENTITY(1,1) PRIMARY KEY,
    UsersId int not null,
    WalletNumber NVARCHAR(50) NOT NULL UNIQUE,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    Pin NVARCHAR(8) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()

    CONSTRAINT FK_Wallets_Users Foreign Key (UsersId) REFERENCES Users(Id)
)

CREATE Table Transactions(
    Id int IDENTITY(1,1) PRIMARY KEY,
    SenderWalletId int,
    ReceiverWalletId int NOT NULL,
    Type NVARCHAR(50) not NULL,
    Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Description TEXT,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Transactions_Sender Foreign Key (SenderWalletId) REFERENCES Users(Id),
    CONSTRAINT FK_Transactions_Receiver Foreign Key (ReceiverWalletId) REFERENCES Users(Id)
)