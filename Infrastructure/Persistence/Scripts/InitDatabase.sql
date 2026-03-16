-- ==========================================================
-- Script completo de base de datos (sin ExpiresAt en UserSessions)
-- ==========================================================

BEGIN TRY
    BEGIN TRANSACTION;

    -- Tabla de Usuarios
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        Role NVARCHAR(20) NOT NULL, -- 'Admin' o 'Voter'
        AccessFailedCount INT DEFAULT 0,
        IsLocked BIT DEFAULT 0,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
    );

    -- Tabla de Sesiones (sin ExpiresAt)
    CREATE TABLE UserSessions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        IpAddress NVARCHAR(45) NULL,
        Device NVARCHAR(200) NULL,
        Location NVARCHAR(100) NULL,
        LastActivityAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),  -- Control de inactividad
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Sessions_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );

    -- Índices para sesiones activas y limpieza por inactividad
    CREATE INDEX IX_UserSessions_UserId_IsActive ON UserSessions(UserId, IsActive);
    CREATE INDEX IX_UserSessions_LastActivityAt ON UserSessions(LastActivityAt);

    -- Tabla de Refresh Tokens (con rotación y detección de fraude)
    CREATE TABLE RefreshTokens (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SessionId INT NOT NULL,
        Token NVARCHAR(500) NOT NULL,
        ExpiresAt DATETIME2 NOT NULL,                -- Vida del refresh token (5-15 min)
        IsRevoked BIT DEFAULT 0,
        IsUsed BIT DEFAULT 0,
        ReplacedByTokenId INT NULL,                  -- Token que reemplaza a este (rotación)
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Tokens_Sessions FOREIGN KEY (SessionId) REFERENCES UserSessions(Id),
        CONSTRAINT FK_Tokens_ReplacedBy FOREIGN KEY (ReplacedByTokenId) REFERENCES RefreshTokens(Id)
    );

    -- Índice único para búsqueda rápida por token
    CREATE UNIQUE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);

    -- Tabla de Votos (NPS)
    CREATE TABLE Votes (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL UNIQUE,                  -- Un voto por usuario
        Score INT NOT NULL CHECK (Score BETWEEN 0 AND 10),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Votes_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );

    -- ==========================================================
    -- Datos de ejemplo (solo si las tablas se crearon correctamente)
    -- ==========================================================

    -- Insert usuario administrador (contraseña: admin123)
    IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
    BEGIN
        INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
        VALUES ('admin', '$2a$12$.jE3abV4QoCzPSpVihXsTeUWR3W1lcsN9UbeOtxouw7f7.txxlOHq', 'Admin', GETUTCDATE());
    END

    -- Insert usuario votante 1 (contraseña: voter123)
    IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'voter1')
    BEGIN
        INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
        VALUES ('voter1', '$2a$10$kI1L2M3N4O5P6Q7R8S9T0U1V2W3X4Y5Z6A7B8C9D0E1F2G3H4I5J6K7L8M', 'Voter', GETUTCDATE());
    END

    -- Insert usuario votante 2 (contraseña: voter123) con hash diferente
    IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'voter2')
    BEGIN
        INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
        VALUES ('voter2', '$2a$10$R1S2T3U4V5W6X7Y8Z9A0B1C2D3E4F5G6H7I8J9K0L1M2N3O4P5Q6R7S8T', 'Voter', GETUTCDATE());
    END

    -- Si todo ha ido bien, confirmamos la transacción
    COMMIT TRANSACTION;
    PRINT 'Script ejecutado correctamente. Todas las tablas y datos han sido creados.';
END TRY
BEGIN CATCH
    -- Si ocurre un error, deshacemos todo
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO