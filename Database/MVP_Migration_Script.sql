-- =============================================
-- SoulBeats MVP Database Migration Script
-- Version: 1.0 - Simple CREATE TABLE statements
-- Description: Creates all necessary tables for MVP functionality
-- =============================================

-- Tabla de usuarios (base ya existente)
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirebaseUid NVARCHAR(128) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL,
    DisplayName NVARCHAR(100) NULL,
    Age INT NULL,
    Bio NVARCHAR(500) NULL,
    FavoriteGenres NVARCHAR(300) NULL,
    ProfilePictureUrl NVARCHAR(500) NULL,
    RegisteredAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- Campos adicionales para geolocalización (ALTER para tabla existente)
ALTER TABLE Users ADD 
    Latitude DECIMAL(10, 8) NULL,
    Longitude DECIMAL(11, 8) NULL,
    LocationCity NVARCHAR(100) NULL,
    LocationCountry NVARCHAR(100) NULL,
    MaxDistanceKm INT DEFAULT 50,
    LocationUpdatedAt DATETIME NULL;

-- Tabla de historial de usuarios
CREATE TABLE UserHistory (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    FirebaseUid NVARCHAR(128) NOT NULL,
    Action NVARCHAR(100) NOT NULL,
    ActionDate DATETIME NOT NULL DEFAULT GETDATE(),
    Details NVARCHAR(500) NULL
);

-- Tabla de géneros musicales
CREATE TABLE Genres (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL,
    IconUrl NVARCHAR(500) NULL,
    DisplayOrder INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Tabla de artistas
CREATE TABLE Artists (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    SpotifyId NVARCHAR(50) NULL,
    ImageUrl NVARCHAR(500) NULL,
    GenreId INT NULL,
    Popularity INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Tabla de preferencias de géneros del usuario
CREATE TABLE UserMusicPreferences (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    FirebaseUid NVARCHAR(128) NOT NULL,
    GenreId INT NOT NULL,
    PreferenceLevel INT NOT NULL CHECK (PreferenceLevel BETWEEN 1 AND 5),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    UNIQUE (FirebaseUid, GenreId)
);

-- Tabla de preferencias de artistas del usuario
CREATE TABLE UserArtistPreferences (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    FirebaseUid NVARCHAR(128) NOT NULL,
    ArtistId INT NOT NULL,
    PreferenceLevel INT NOT NULL CHECK (PreferenceLevel BETWEEN 1 AND 5),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    UNIQUE (FirebaseUid, ArtistId)
);

-- Tabla de swipes entre usuarios
CREATE TABLE UserSwipes (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    SwiperUid NVARCHAR(128) NOT NULL,
    SwipedUid NVARCHAR(128) NOT NULL,
    SwipeType NVARCHAR(20) NOT NULL CHECK (SwipeType IN ('LIKE', 'PASS', 'SUPER_LIKE')),
    SwipedAt DATETIME DEFAULT GETDATE(),
    UNIQUE (SwiperUid, SwipedUid)
);

-- Tabla de matches entre usuarios
CREATE TABLE UserMatches (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    User1Uid NVARCHAR(128) NOT NULL,
    User2Uid NVARCHAR(128) NOT NULL,
    MatchedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    CompatibilityScore DECIMAL(5,2) NULL,
    LastInteractionAt DATETIME NULL,
    CONSTRAINT CK_UserMatches_DifferentUsers CHECK (User1Uid != User2Uid),
    CONSTRAINT UQ_UserMatches_Pair UNIQUE (User1Uid, User2Uid)
);

-- Tabla de cola de descubrimiento
CREATE TABLE UserDiscoveryQueue (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ForUserUid NVARCHAR(128) NOT NULL,
    CandidateUid NVARCHAR(128) NOT NULL,
    CompatibilityScore DECIMAL(5,2) DEFAULT 0.00,
    Distance DECIMAL(8,2) NULL,
    QueuePosition INT NOT NULL,
    IsShown BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    ExpiresAt DATETIME NULL,
    UNIQUE (ForUserUid, CandidateUid)
);

-- =============================================
-- ÍNDICES BÁSICOS PARA RENDIMIENTO
-- =============================================

CREATE INDEX IX_Users_FirebaseUid ON Users(FirebaseUid);
CREATE INDEX IX_UserSwipes_Swiper ON UserSwipes(SwiperUid, SwipedAt);
CREATE INDEX IX_UserMatches_User1 ON UserMatches(User1Uid, IsActive);
CREATE INDEX IX_UserMatches_User2 ON UserMatches(User2Uid, IsActive);
CREATE INDEX IX_UserMusicPreferences_User ON UserMusicPreferences(FirebaseUid);
CREATE INDEX IX_UserArtistPreferences_User ON UserArtistPreferences(FirebaseUid);

-- =============================================
-- SPOTIFY INTEGRATION TABLES
-- =============================================

-- Tabla para almacenar tokens de Spotify por usuario
CREATE TABLE SpotifyTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirebaseUid NVARCHAR(128) NOT NULL,
    AccessToken NVARCHAR(500) NOT NULL,
    RefreshToken NVARCHAR(500) NULL,
    ExpiresAt DATETIME NOT NULL,
    TokenType NVARCHAR(50) DEFAULT 'Bearer',
    Scope NVARCHAR(500) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_SpotifyTokens_Users FOREIGN KEY (FirebaseUid) REFERENCES Users(FirebaseUid) ON DELETE CASCADE,
    CONSTRAINT UQ_SpotifyTokens_User UNIQUE (FirebaseUid)
);

-- Índice para búsquedas por usuario
CREATE INDEX IX_SpotifyTokens_FirebaseUid ON SpotifyTokens(FirebaseUid);