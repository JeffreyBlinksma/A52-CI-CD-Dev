CREATE DATABASE Area52Entertainment;
GO

USE Area52Entertainment;
GO

-- Tabel: AnnuleringsBeleid
CREATE TABLE AnnuleringsBeleid (
    AnnuleringsBeleidId INT IDENTITY(1,1) PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Waarde DECIMAL(10,2) NOT NULL
);
GO

-- Tabel: Activiteit
CREATE TABLE Activiteit (
    ActiviteitId INT IDENTITY(1,1) PRIMARY KEY,
    Naam NVARCHAR(200) NOT NULL,
    DuurInMinuten INT NOT NULL,
    BasisPrijs DECIMAL(10,2) NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    MinimumLeeftijd INT NULL,
    MaximumLeeftijd INT NULL,
    LeeftijdsToeslag DECIMAL(5,2) NULL,
    MaxCapaciteit INT NULL,
    AnnuleringsBeleidId INT NOT NULL,
    CONSTRAINT FK_Activiteit_AnnuleringsBeleid FOREIGN KEY (AnnuleringsBeleidId)
        REFERENCES AnnuleringsBeleid(AnnuleringsBeleidId)
);
GO

-- Tabel: Deelnemer
CREATE TABLE Deelnemer (
    DeelnemerId INT IDENTITY(1,1) PRIMARY KEY,
    Naam NVARCHAR(200) NOT NULL,
    Leeftijd INT NOT NULL,
    Email NVARCHAR(200) NOT NULL
);
GO

-- Tabel: Reservering
CREATE TABLE Reservering (
    ReserveringId INT IDENTITY(1,1) PRIMARY KEY,
    DeelnemerId INT NOT NULL,
    ActiviteitId INT NOT NULL,
    StartTijd DATETIME NOT NULL,
    AantalPersonen INT NOT NULL,
    TotaalPrijs DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Reservering_Deelnemer FOREIGN KEY (DeelnemerId) REFERENCES Deelnemer(DeelnemerId),
    CONSTRAINT FK_Reservering_Activiteit FOREIGN KEY (ActiviteitId) REFERENCES Activiteit(ActiviteitId)
);
GO

-- Seed data AnnuleringsBeleid
INSERT INTO AnnuleringsBeleid (Type, Waarde) VALUES
('gratis', 0.00),      -- Id 1
('vast', 10.00),       -- Id 2
('percentage', 25.00); -- Id 3 (25%)
GO

-- Seed data Activiteit
INSERT INTO Activiteit (Naam, DuurInMinuten, BasisPrijs, Type, MinimumLeeftijd, MaximumLeeftijd, LeeftijdsToeslag, MaxCapaciteit, AnnuleringsBeleidId)
VALUES
('Space Laser Show', 60, 25.00, 'Show', NULL, NULL, NULL, 120, 1),

('Jonge Ruimtevaarders Workshop', 90, 15.00, 'Workshop', 8, 12, 10.00, NULL, 2),

('Galactic Robotics Workshop', 120, 22.50, 'Workshop', 12, 18, 20.00, NULL, 3);
GO