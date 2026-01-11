CREATE DATABASE SystemBankowy;
GO
USE SystemBankowy;
CREATE TABLE TypyKont (
    IDTypu INT PRIMARY KEY IDENTITY(1,1),
    NazwaTypu NVARCHAR(50) NOT NULL UNIQUE,
    Oprocentowanie DECIMAL(5,2) DEFAULT 0
);
CREATE TABLE TypyKart (
    IDTypu INT PRIMARY KEY IDENTITY(1,1),
    NazwaTypu NVARCHAR(50) NOT NULL,
    CzyZblizeniowa BIT DEFAULT 1
);
CREATE TABLE Adresy (
    IDAdresu INT PRIMARY KEY IDENTITY(1,1),
    Ulica NVARCHAR(100) NOT NULL,
    Miasto NVARCHAR(50) NOT NULL,
    KodPocztowy NVARCHAR(10) NOT NULL
);
CREATE TABLE Klienci (
    IDKlienta INT PRIMARY KEY IDENTITY(1,1),
    Imie NVARCHAR(50) NOT NULL,
    Nazwisko NVARCHAR(50) NOT NULL,
    PESEL NVARCHAR(11) UNIQUE NOT NULL,
    IDAdresu INT,
    FOREIGN KEY (IDAdresu) REFERENCES Adresy(IDAdresu)
);
CREATE TABLE Konta (
    IDKonta INT PRIMARY KEY IDENTITY(100,1),
    NumerKonta NVARCHAR(26) UNIQUE NOT NULL,
    Saldo DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IDKlienta INT NOT NULL,
    IDTypuKonta INT NOT NULL,
    FOREIGN KEY (IDKlienta) REFERENCES Klienci(IDKlienta),
    FOREIGN KEY (IDTypuKonta) REFERENCES TypyKont(IDTypu)
);
CREATE TABLE Karty (
    IDKarty INT PRIMARY KEY IDENTITY(1,1),
    NumerKarty NVARCHAR(16) UNIQUE NOT NULL,
    PINHash NVARCHAR(255) NOT NULL,
    IDKonta INT NOT NULL,
    IDTypuKarty INT NOT NULL,
    DataWaznosci DATE NOT NULL,
    FOREIGN KEY (IDKonta) REFERENCES Konta(IDKonta),
    FOREIGN KEY (IDTypuKarty) REFERENCES TypyKart(IDTypu)
);
CREATE TABLE Transakcje (
    IDTransakcji INT PRIMARY KEY IDENTITY(1,1),
    DataTransakcji DATETIME DEFAULT GETDATE(),
    Kwota DECIMAL(18,2) NOT NULL,
    TypTransakcji NVARCHAR(20) NOT NULL,
    IDKarty INT NOT NULL,
    FOREIGN KEY (IDKarty) REFERENCES Karty(IDKarty)
);
GO
INSERT INTO Adresy (Ulica, Miasto, KodPocztowy) VALUES ('Wiejska 1', 'Warszawa', '00-001');
INSERT INTO Klienci (Imie, Nazwisko, PESEL, IDAdresu) VALUES ('Jan', 'Wójcik', '80010112345', 1);
INSERT INTO	TypyKont (NazwaTypu) VALUES ('Osobiste');
INSERT INTO Konta (NumerKonta, Saldo, IDKlienta, IDTypuKonta) VALUES ('12345678901234567890123456', 10000.00, 1, 1);
INSERT INTO TypyKart (Nazwatypu) VALUES ('American Express');
INSERT INTO Karty (NumerKarty, PINHash, IDKonta, IDTypuKarty, DataWaznosci)
VALUES ('370000000000000', '0000', 100, 1, '2028-12-31'); 

INSERT INTO Adresy (Ulica, Miasto, KodPocztowy) VALUES ('Kwiatowa 5', 'Kraków', '31-001');
INSERT INTO Klienci (Imie, Nazwisko, PESEL, IDAdresu) VALUES ('Anna', 'Kowalska', '90020254321', 2);
INSERT INTO	TypyKont (NazwaTypu) VALUES ('Osobiste');
INSERT INTO Konta (NumerKonta, Saldo, IDKlienta, IDTypuKonta) VALUES ('4567123456789012', 1500.00, 2, 1);
INSERT INTO TypyKart (Nazwatypu) VALUES ('Visa');
INSERT INTO Karty (NumerKarty, PINHash, IDKonta, IDTypuKarty, DataWaznosci)
VALUES ('4567123456789012', '1234', 101, 2, '2027-01-01'); 

INSERT INTO Adresy (Ulica, Miasto, KodPocztowy) VALUES ('D³uga 10', 'Wroc³aw', '50-001');
INSERT INTO Klienci (Imie, Nazwisko, PESEL, IDAdresu) VALUES ('Piotr', 'Nowak', '85030398765', 3);
INSERT INTO	TypyKont (NazwaTypu) VALUES ('Osobiste');
INSERT INTO Konta (NumerKonta, Saldo, IDKlienta, IDTypuKonta) VALUES ('5555432187654321', 500.00, 3, 1);
INSERT INTO TypyKart (Nazwatypu) VALUES ('Mastercard');
INSERT INTO Karty (NumerKarty, PINHash, IDKonta, IDTypuKarty, DataWaznosci)
VALUES ('5555432187654321', '4321', 102, 3, '2026-06-01'); 
