/*
  Données de test (SQL Server) — taxi / EF Core schema.
  - 5 BOSS, 10 EMPLOYEE (15 Users)
  - 12 Taxis (plaque ###TUN####), répartis entre les patrons
  - 12 Assignments (employés ↔ taxis)
  - 60+ Rides (réparties par taxi et par employé)
  - 12 Expenses
  - 10 Commissions (2 lignes par patron — voir note ci‑dessous)

  PRÉREQUIS : base vide OU exécuter le bloc DELETE ci‑dessous avant insertion.
  NOTE Commissions : l’API utilise une commission par patron (FirstOrDefault).
        Les doublons par BossId servent uniquement au volume de test ; en prod un boss = une ligne.

  Usage (exemple) :
    sqlcmd -S localhost,1433 -d TaxiDb -U sa -P '...' -i scripts/seed-test-data.sql
*/

SET NOCOUNT ON;
SET DATEFORMAT ymd;

/* ---------- Nettoyage (ordre FK) ---------- */
IF OBJECT_ID(N'dbo.Rides', N'U') IS NOT NULL DELETE FROM dbo.Rides;
IF OBJECT_ID(N'dbo.Expenses', N'U') IS NOT NULL DELETE FROM dbo.Expenses;
IF OBJECT_ID(N'dbo.Assignments', N'U') IS NOT NULL DELETE FROM dbo.Assignments;
IF OBJECT_ID(N'dbo.Commissions', N'U') IS NOT NULL DELETE FROM dbo.Commissions;
IF OBJECT_ID(N'dbo.Taxis', N'U') IS NOT NULL DELETE FROM dbo.Taxis;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DELETE FROM dbo.Users;

/* ---------- Users : 5 BOSS + 10 EMPLOYEE (mot de passe plain max 8 car.) ---------- */
SET IDENTITY_INSERT dbo.Users ON;

INSERT INTO dbo.Users (Id, FirstName, LastName, Password, CIN, PhoneNumber, Role) VALUES
 (1,  N'Sami',     N'BossTun',    N'password', N'000TUN0001', N'+21671001001', N'BOSS'),
 (2,  N'Leila',    N'BossSfax',   N'password', N'000TUN0002', N'+21671001002', N'BOSS'),
 (3,  N'Karim',    N'BossSousse', N'password', N'000TUN0003', N'+21671001003', N'BOSS'),
 (4,  N'Amira',    N'BossNabeul', N'password', N'000TUN0004', N'+21671001004', N'BOSS'),
 (5,  N'Hedi',     N'BossBizerte',N'password', N'000TUN0005', N'+21671001005', N'BOSS'),
 (6,  N'Ali',      N'Chauffeur1', N'password', N'010TUN0001', N'+21672001001', N'EMPLOYEE'),
 (7,  N'Sarra',    N'Chauffeur2', N'password', N'010TUN0002', N'+21672001002', N'EMPLOYEE'),
 (8,  N'Mehdi',    N'Chauffeur3', N'password', N'010TUN0003', N'+21672001003', N'EMPLOYEE'),
 (9,  N'Fatma',    N'Chauffeur4', N'password', N'010TUN0004', N'+21672001004', N'EMPLOYEE'),
 (10, N'Omar',     N'Chauffeur5', N'password', N'010TUN0005', N'+21672001005', N'EMPLOYEE'),
 (11, N'Youssef',  N'Chauffeur6', N'password', N'010TUN0006', N'+21672001006', N'EMPLOYEE'),
 (12, N'Nadia',    N'Chauffeur7', N'password', N'010TUN0007', N'+21672001007', N'EMPLOYEE'),
 (13, N'Bilel',    N'Chauffeur8', N'password', N'010TUN0008', N'+21672001008', N'EMPLOYEE'),
 (14, N'Ines',     N'Chauffeur9', N'password', N'010TUN0009', N'+21672001009', N'EMPLOYEE'),
 (15, N'Walid',    N'Chauffeur10',N'password', N'010TUN0010', N'+21672001010', N'EMPLOYEE');

SET IDENTITY_INSERT dbo.Users OFF;

/* ---------- Taxis : plaques ###TUN#### ---------- */
SET IDENTITY_INSERT dbo.Taxis ON;

INSERT INTO dbo.Taxis (Id, PlateNumber, Governorate, BossId) VALUES
 (1,  N'001TUN1001', N'Tunis',    1),
 (2,  N'002TUN1002', N'Tunis',    1),
 (3,  N'003TUN1003', N'Ariana',   1),
 (4,  N'004TUN1004', N'Sfax',     2),
 (5,  N'005TUN1005', N'Sfax',     2),
 (6,  N'006TUN1006', N'Sousse',   3),
 (7,  N'007TUN1007', N'Monastir', 3),
 (8,  N'008TUN1008', N'Nabeul',   4),
 (9,  N'009TUN1009', N'Hammamet', 4),
 (10, N'010TUN1010', N'Bizerte',  5),
 (11, N'011TUN1011', N'Bizerte',  5),
 (12, N'012TUN1012', N'Tunis',    5);

SET IDENTITY_INSERT dbo.Taxis OFF;

/* ---------- Commissions : 10 lignes (2 par patron) ---------- */
SET IDENTITY_INSERT dbo.Commissions ON;

INSERT INTO dbo.Commissions (Id, BossId, Percentage) VALUES
 (1, 1, 30.00), (2, 1, 32.50),
 (3, 2, 28.00), (4, 2, 30.00),
 (5, 3, 35.00), (6, 3, 35.00),
 (7, 4, 25.00), (8, 4, 27.50),
 (9, 5, 33.00), (10, 5, 34.00);

SET IDENTITY_INSERT dbo.Commissions OFF;

/* ---------- Assignments : employés affectés aux taxis (EndDate NULL = actif) ---------- */
SET IDENTITY_INSERT dbo.Assignments ON;

INSERT INTO dbo.Assignments (Id, TaxiId, EmployeeId, StartDate, EndDate) VALUES
 (1,  1,  6, N'2025-06-01T08:00:00', NULL),
 (2,  2,  7, N'2025-06-01T08:00:00', NULL),
 (3,  3,  8, N'2025-06-01T08:00:00', NULL),
 (4,  4,  9, N'2025-06-01T08:00:00', NULL),
 (5,  5, 10, N'2025-06-01T08:00:00', NULL),
 (6,  6, 11, N'2025-06-01T08:00:00', NULL),
 (7,  7, 12, N'2025-06-01T08:00:00', NULL),
 (8,  8, 13, N'2025-06-01T08:00:00', NULL),
 (9,  9, 14, N'2025-06-01T08:00:00', NULL),
 (10, 10, 15, N'2025-06-01T08:00:00', NULL),
 (11, 11, 6,  N'2025-09-01T08:00:00', NULL),
 (12, 12, 7,  N'2025-09-01T08:00:00', NULL);

SET IDENTITY_INSERT dbo.Assignments OFF;

/* ---------- Rides : beaucoup de courses — rotation taxi 1..12 et employé 6..15 ---------- */
SET IDENTITY_INSERT dbo.Rides ON;

;WITH seq AS (
  SELECT n = ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
  FROM (VALUES (0),(0),(0),(0),(0),(0),(0),(0)) a(n)
  CROSS JOIN (VALUES (0),(0),(0),(0),(0),(0),(0),(0)) b(n)
)
INSERT INTO dbo.Rides (Id, StartDate, EndDate, DistanceKm, Amount, TaxiId, EmployeeId)
SELECT
  s.n,
  DATEADD(HOUR, (s.n % 14) + 6, DATEADD(DAY, (s.n % 120) - 60, CAST(N'2026-01-01' AS datetime2))),
  DATEADD(MINUTE, 25 + (s.n % 90), DATEADD(HOUR, (s.n % 14) + 6, DATEADD(DAY, (s.n % 120) - 60, CAST(N'2026-01-01' AS datetime2)))),
  CAST(4 + (s.n % 35) AS decimal(18,2)),
  CAST(8.50 + (s.n % 50) * 1.25 AS decimal(18,2)),
  ((s.n - 1) % 12) + 1,
  6 + ((s.n - 1) % 10)
FROM seq s
WHERE s.n BETWEEN 1 AND 60;

SET IDENTITY_INSERT dbo.Rides OFF;

/* ---------- Expenses ---------- */
SET IDENTITY_INSERT dbo.Expenses ON;

INSERT INTO dbo.Expenses (Id, Type, Amount, ExpenseDate, Description, TaxiId, PlateNumber) VALUES
 (1,  N'FUEL',       120.00, N'2026-01-05', N'Plein station centre',        1, N'001TUN1001'),
 (2,  N'REPAIR',     450.00, N'2026-01-08', N'Freins avant',                2, N'002TUN1002'),
 (3,  N'INSURANCE',  890.00, N'2026-01-10', N'Annuelle',                    3, N'003TUN1003'),
 (4,  N'VIGNETTE',   45.00,  N'2026-01-12', N'Vignette',                    4, N'004TUN1004'),
 (5,  N'FUEL',       95.50,  N'2026-02-02', N'Autoroute',                   5, N'005TUN1005'),
 (6,  N'OTHER',      35.00,  N'2026-02-06', N'Lavage',                      6, N'006TUN1006'),
 (7,  N'FUEL',       110.00, N'2026-02-14', N'Plein',                       7, N'007TUN1007'),
 (8,  N'REPAIR',     220.00, N'2026-02-18', N'Pneu',                        8, N'008TUN1008'),
 (9,  N'FUEL',       88.00,  N'2026-03-01', N'Plein',                       9, N'009TUN1009'),
 (10, N'OTHER',      60.00,  N'2026-03-04', N'Révision rapide',            10, N'010TUN1010'),
 (11, N'FUEL',       102.00, N'2026-03-09', N'Plein',                      11, N'011TUN1011'),
 (12, N'INSURANCE',  120.00, N'2026-03-11', N'Complément',                 12, N'012TUN1012');

SET IDENTITY_INSERT dbo.Expenses OFF;

/* ---------- Resynchroniser les identités ---------- */
DBCC CHECKIDENT (N'dbo.Users', RESEED, 15);
DBCC CHECKIDENT (N'dbo.Taxis', RESEED, 12);
DBCC CHECKIDENT (N'dbo.Commissions', RESEED, 10);
DBCC CHECKIDENT (N'dbo.Assignments', RESEED, 12);
DBCC CHECKIDENT (N'dbo.Rides', RESEED, 60);
DBCC CHECKIDENT (N'dbo.Expenses', RESEED, 12);

PRINT N'Seed OK — Users:15 (5 BOSS + 10 EMPLOYEE), Taxis:12, Assignments:12, Rides:60, Expenses:12, Commissions:10.';
GO
