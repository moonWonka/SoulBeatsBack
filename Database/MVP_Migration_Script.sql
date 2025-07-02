-- =============================================
-- SoulBeats MVP Database Migration Script
-- Version: 1.0
-- Description: Creates all necessary tables for MVP functionality
-- =============================================

-- Verificar si la tabla Users existe, si no crearla
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,                           -- Identificador interno autoincremental
        FirebaseUid NVARCHAR(128) NOT NULL UNIQUE,                 -- ID de usuario desde Firebase (Google)
        Email NVARCHAR(255) NOT NULL,                              -- Correo electrónico del usuario
        DisplayName NVARCHAR(100) NULL,                            -- Nombre para mostrar (editable)
        Age INT NULL,                                              -- Edad del usuario
        Bio NVARCHAR(500) NULL,                                    -- Biografía o descripción breve
        FavoriteGenres NVARCHAR(300) NULL,                         -- Géneros musicales favoritos (separados por comas)
        ProfilePictureUrl NVARCHAR(500) NULL,                      -- URL de la foto de perfil
        RegisteredAt DATETIME NOT NULL DEFAULT GETDATE()           -- Fecha y hora de registro
    );
    PRINT 'Tabla Users creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla Users ya existe';
END

-- Agregar campos de geolocalización a tabla Users existente
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'Latitude')
BEGIN
    ALTER TABLE Users ADD 
        Latitude DECIMAL(10, 8) NULL,                           -- Latitud para geolocalización
        Longitude DECIMAL(11, 8) NULL,                          -- Longitud para geolocalización
        LocationCity NVARCHAR(100) NULL,                        -- Ciudad del usuario
        LocationCountry NVARCHAR(100) NULL,                     -- País del usuario
        MaxDistanceKm INT DEFAULT 50,                           -- Distancia máxima para matches (en km)
        LocationUpdatedAt DATETIME NULL;                        -- Última actualización de ubicación
    PRINT 'Campos de geolocalización agregados a tabla Users';
END
ELSE
BEGIN
    PRINT 'Campos de geolocalización ya existen en tabla Users';
END

-- Crear tabla de historial de usuarios si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserHistory' AND xtype='U')
BEGIN
    CREATE TABLE UserHistory (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        FirebaseUid NVARCHAR(128) NOT NULL,
        Action NVARCHAR(100) NOT NULL,
        ActionDate DATETIME NOT NULL DEFAULT GETDATE(),
        Details NVARCHAR(500) NULL,
        FOREIGN KEY (FirebaseUid) REFERENCES Users(FirebaseUid)
    );
    PRINT 'Tabla UserHistory creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla UserHistory ya existe';
END

-- =============================================
-- NUEVAS TABLAS PARA MVP
-- =============================================

-- Tabla de Géneros Musicales
CREATE TABLE Genres (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL,
    IconUrl NVARCHAR(500) NULL,
    DisplayOrder INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Tabla de Artistas
CREATE TABLE Artists (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    SpotifyId NVARCHAR(50) NULL,                                -- Para futuro uso con Spotify API
    ImageUrl NVARCHAR(500) NULL,
    GenreId INT NULL,
    Popularity INT DEFAULT 0,                                   -- Popularidad del artista (0-100)
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (GenreId) REFERENCES Genres(Id)
);

-- Tabla de Preferencias Musicales del Usuario por Género
CREATE TABLE UserMusicPreferences (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    FirebaseUid NVARCHAR(128) NOT NULL,
    GenreId INT NOT NULL,
    PreferenceLevel INT NOT NULL CHECK (PreferenceLevel BETWEEN 1 AND 5), -- 1=No me gusta, 5=Me encanta
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (FirebaseUid) REFERENCES Users(FirebaseUid),
    FOREIGN KEY (GenreId) REFERENCES Genres(Id),
    UNIQUE (FirebaseUid, GenreId)
);

-- Tabla de Preferencias de Artistas del Usuario
CREATE TABLE UserArtistPreferences (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    FirebaseUid NVARCHAR(128) NOT NULL,
    ArtistId INT NOT NULL,
    PreferenceLevel INT NOT NULL CHECK (PreferenceLevel BETWEEN 1 AND 5), -- 1=No me gusta, 5=Me encanta
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (FirebaseUid) REFERENCES Users(FirebaseUid),
    FOREIGN KEY (ArtistId) REFERENCES Artists(Id),
    UNIQUE (FirebaseUid, ArtistId)
);

-- Tabla de Swipes/Acciones de Usuario
CREATE TABLE UserSwipes (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    SwiperUid NVARCHAR(128) NOT NULL,                          -- Usuario que hace el swipe
    SwipedUid NVARCHAR(128) NOT NULL,                          -- Usuario que recibe el swipe
    SwipeType NVARCHAR(20) NOT NULL CHECK (SwipeType IN ('LIKE', 'PASS', 'SUPER_LIKE')),
    SwipedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (SwiperUid) REFERENCES Users(FirebaseUid),
    FOREIGN KEY (SwipedUid) REFERENCES Users(FirebaseUid),
    UNIQUE (SwiperUid, SwipedUid)                              -- Un usuario solo puede hacer swipe una vez a otro
);

-- Tabla de Matches entre Usuarios
CREATE TABLE UserMatches (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    User1Uid NVARCHAR(128) NOT NULL,
    User2Uid NVARCHAR(128) NOT NULL,
    MatchedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    CompatibilityScore DECIMAL(5,2) NULL,                      -- Porcentaje de compatibilidad musical (0.00-100.00)
    LastInteractionAt DATETIME NULL,
    FOREIGN KEY (User1Uid) REFERENCES Users(FirebaseUid),
    FOREIGN KEY (User2Uid) REFERENCES Users(FirebaseUid),
    CONSTRAINT CK_UserMatches_DifferentUsers CHECK (User1Uid != User2Uid),
    CONSTRAINT UQ_UserMatches_Pair UNIQUE (User1Uid, User2Uid)
);

-- Tabla de Cola de Descubrimiento (Optimización)
CREATE TABLE UserDiscoveryQueue (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ForUserUid NVARCHAR(128) NOT NULL,                         -- Usuario para quien es la cola
    CandidateUid NVARCHAR(128) NOT NULL,                       -- Usuario candidato
    CompatibilityScore DECIMAL(5,2) DEFAULT 0.00,             -- Score de compatibilidad
    Distance DECIMAL(8,2) NULL,                                -- Distancia en kilómetros
    QueuePosition INT NOT NULL,
    IsShown BIT DEFAULT 0,                                     -- Si ya fue mostrado al usuario
    CreatedAt DATETIME DEFAULT GETDATE(),
    ExpiresAt DATETIME NULL,                                   -- Opcional: expiración de la cola
    FOREIGN KEY (ForUserUid) REFERENCES Users(FirebaseUid),
    FOREIGN KEY (CandidateUid) REFERENCES Users(FirebaseUid),
    UNIQUE (ForUserUid, CandidateUid)
);

-- =============================================
-- ÍNDICES PARA OPTIMIZACIÓN
-- =============================================

-- Índices para tabla Users
CREATE INDEX IX_Users_FirebaseUid ON Users(FirebaseUid);
CREATE INDEX IX_Users_Location ON Users(Latitude, Longitude) WHERE Latitude IS NOT NULL AND Longitude IS NOT NULL;
CREATE INDEX IX_Users_Active ON Users(RegisteredAt) WHERE Age IS NOT NULL;

-- Índices para tabla UserSwipes
CREATE INDEX IX_UserSwipes_Swiper ON UserSwipes(SwiperUid, SwipedAt);
CREATE INDEX IX_UserSwipes_Swiped ON UserSwipes(SwipedUid, SwipedAt);
CREATE INDEX IX_UserSwipes_Type ON UserSwipes(SwipeType, SwipedAt);

-- Índices para tabla UserMatches
CREATE INDEX IX_UserMatches_User1 ON UserMatches(User1Uid, IsActive);
CREATE INDEX IX_UserMatches_User2 ON UserMatches(User2Uid, IsActive);
CREATE INDEX IX_UserMatches_Active ON UserMatches(IsActive, MatchedAt);

-- Índices para tabla UserDiscoveryQueue
CREATE INDEX IX_UserDiscoveryQueue_User ON UserDiscoveryQueue(ForUserUid, QueuePosition, IsShown);
CREATE INDEX IX_UserDiscoveryQueue_Candidate ON UserDiscoveryQueue(CandidateUid);
CREATE INDEX IX_UserDiscoveryQueue_Score ON UserDiscoveryQueue(CompatibilityScore DESC);

-- Índices para preferencias musicales
CREATE INDEX IX_UserMusicPreferences_User ON UserMusicPreferences(FirebaseUid);
CREATE INDEX IX_UserArtistPreferences_User ON UserArtistPreferences(FirebaseUid);

-- =============================================
-- FUNCIONES AUXILIARES
-- =============================================

-- Función para calcular distancia entre dos puntos (Haversine formula)
CREATE FUNCTION dbo.CalculateDistance(
    @lat1 DECIMAL(10,8),
    @lon1 DECIMAL(11,8),
    @lat2 DECIMAL(10,8),
    @lon2 DECIMAL(11,8)
)
RETURNS DECIMAL(8,2)
AS
BEGIN
    DECLARE @R DECIMAL(8,2) = 6371.0; -- Radio de la Tierra en km
    DECLARE @dLat DECIMAL(18,15) = RADIANS(@lat2 - @lat1);
    DECLARE @dLon DECIMAL(18,15) = RADIANS(@lon2 - @lon1);
    DECLARE @a DECIMAL(18,15) = 
        SIN(@dLat/2) * SIN(@dLat/2) + 
        COS(RADIANS(@lat1)) * COS(RADIANS(@lat2)) * 
        SIN(@dLon/2) * SIN(@dLon/2);
    DECLARE @c DECIMAL(18,15) = 2 * ATN2(SQRT(@a), SQRT(1-@a));
    RETURN @R * @c;
END;

-- =============================================
-- TRIGGERS
-- =============================================

-- Trigger para actualizar UpdatedAt en UserMusicPreferences
CREATE TRIGGER TR_UserMusicPreferences_UpdatedAt
ON UserMusicPreferences
AFTER UPDATE
AS
BEGIN
    UPDATE UserMusicPreferences 
    SET UpdatedAt = GETDATE()
    FROM UserMusicPreferences u
    INNER JOIN inserted i ON u.Id = i.Id;
END;

-- Trigger para actualizar UpdatedAt en UserArtistPreferences
CREATE TRIGGER TR_UserArtistPreferences_UpdatedAt
ON UserArtistPreferences
AFTER UPDATE
AS
BEGIN
    UPDATE UserArtistPreferences 
    SET UpdatedAt = GETDATE()
    FROM UserArtistPreferences u
    INNER JOIN inserted i ON u.Id = i.Id;
END;

-- Trigger para crear match automáticamente cuando hay like mutuo
CREATE TRIGGER TR_UserSwipes_CreateMatch
ON UserSwipes
AFTER INSERT
AS
BEGIN
    INSERT INTO UserMatches (User1Uid, User2Uid, MatchedAt)
    SELECT 
        CASE 
            WHEN i.SwiperUid < i.SwipedUid THEN i.SwiperUid 
            ELSE i.SwipedUid 
        END,
        CASE 
            WHEN i.SwiperUid < i.SwipedUid THEN i.SwipedUid 
            ELSE i.SwiperUid 
        END,
        GETDATE()
    FROM inserted i
    WHERE i.SwipeType IN ('LIKE', 'SUPER_LIKE')
    AND EXISTS (
        SELECT 1 FROM UserSwipes us
        WHERE us.SwiperUid = i.SwipedUid 
        AND us.SwipedUid = i.SwiperUid
        AND us.SwipeType IN ('LIKE', 'SUPER_LIKE')
    )
    AND NOT EXISTS (
        SELECT 1 FROM UserMatches um
        WHERE (um.User1Uid = i.SwiperUid AND um.User2Uid = i.SwipedUid)
        OR (um.User1Uid = i.SwipedUid AND um.User2Uid = i.SwiperUid)
    );
END;

-- =============================================
-- VALIDACIONES FINALES
-- =============================================

PRINT '====================================';
PRINT 'RESUMEN DE CREACIÓN DE TABLAS:';
PRINT '====================================';

SELECT 
    'Users' as TableName,
    COUNT(*) as RecordCount
FROM Users
UNION ALL
SELECT 'UserHistory', COUNT(*) FROM UserHistory
UNION ALL
SELECT 'Genres', COUNT(*) FROM Genres
UNION ALL
SELECT 'Artists', COUNT(*) FROM Artists
UNION ALL
SELECT 'UserMusicPreferences', COUNT(*) FROM UserMusicPreferences
UNION ALL
SELECT 'UserArtistPreferences', COUNT(*) FROM UserArtistPreferences
UNION ALL
SELECT 'UserSwipes', COUNT(*) FROM UserSwipes
UNION ALL
SELECT 'UserMatches', COUNT(*) FROM UserMatches
UNION ALL
SELECT 'UserDiscoveryQueue', COUNT(*) FROM UserDiscoveryQueue;

PRINT '====================================';
PRINT 'MIGRACIÓN COMPLETADA EXITOSAMENTE';
PRINT '====================================';