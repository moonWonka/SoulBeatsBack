-- =============================================
-- SoulBeats MVP Database Migration Script
-- Version: 1.0 - Clean structure with PK and FK
-- Description: Shows table structure with Primary Keys and Foreign Keys
-- =============================================

-- Eliminar tablas si existen (en orden correcto para respetar FK)
DROP TABLE IF EXISTS SpotifyTokens;
DROP TABLE IF EXISTS UserDiscoveryQueue;
DROP TABLE IF EXISTS UserMatches;
DROP TABLE IF EXISTS UserSwipes;
DROP TABLE IF EXISTS UserArtistPreferences;
DROP TABLE IF EXISTS UserMusicPreferences;
DROP TABLE IF EXISTS Artistss;
DROP TABLE IF EXISTS Genres;
DROP TABLE IF EXISTS UserHistory;
DROP TABLE IF EXISTS Users;

-- Tabla de usuarios (base)
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,  -- PK
    FirebaseUid NVARCHAR(128) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL,
    DisplayName NVARCHAR(100) NULL,
    Age INT NULL,
    Bio NVARCHAR(500) NULL,
    FavoriteGenres NVARCHAR(300) NULL,
    ProfilePictureUrl NVARCHAR(500) NULL,
    RegisteredAt DATETIME NOT NULL DEFAULT GETDATE(),
    Latitude DECIMAL(10, 8) NULL,
    Longitude DECIMAL(11, 8) NULL,
    LocationCity NVARCHAR(100) NULL,
    LocationCountry NVARCHAR(100) NULL,
    MaxDistanceKm INT DEFAULT 50,
    LocationUpdatedAt DATETIME NULL
);

-- Tabla de historial de usuarios
CREATE TABLE UserHistory (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,  -- PK
    FirebaseUid NVARCHAR(128) NOT NULL,  -- FK hacia Users
    Action NVARCHAR(100) NOT NULL,
    ActionDate DATETIME NOT NULL DEFAULT GETDATE(),
    Details NVARCHAR(500) NULL,
    CONSTRAINT FK_UserHistory_Users FOREIGN KEY (FirebaseUid) REFERENCES Users(FirebaseUid) ON DELETE CASCADE
);

-- Tabla de géneros musicales
CREATE TABLE Genres (
    Id INT IDENTITY(1,1) PRIMARY KEY,  -- PK
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL,
    IconUrl NVARCHAR(500) NULL,
    DisplayOrder INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Tabla de artistas
CREATE TABLE Artistss (
    Id INT IDENTITY(1,1) PRIMARY KEY,  -- PK
    Name NVARCHAR(100) NOT NULL,
    SpotifyId NVARCHAR(50) NULL,
    ImageUrl NVARCHAR(500) NULL,
    GenreId INT NULL,  -- FK hacia Genres
    Popularity INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Artists_Genres FOREIGN KEY (GenreId) REFERENCES Genres(Id)
);

-- Tabla de preferencias de géneros del usuario
CREATE TABLE UserMusicPreferences (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,  -- PK
    FirebaseUid NVARCHAR(128) NOT NULL,  -- FK hacia Users
    GenreId INT NOT NULL,  -- FK hacia Genres
    PreferenceLevel INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_UserMusicPreferences_Users FOREIGN KEY (FirebaseUid) REFERENCES Users(FirebaseUid) ON DELETE CASCADE,
    CONSTRAINT FK_UserMusicPreferences_Genres FOREIGN KEY (GenreId) REFERENCES Genres(Id),
    CONSTRAINT UQ_UserMusicPreferences_User_Genre UNIQUE (FirebaseUid, GenreId)
);

-- Tabla de preferencias de artistas del usuario
CREATE TABLE UserArtistPreferences (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,  -- PK
    FirebaseUid NVARCHAR(128) NOT NULL,  -- FK hacia Users
    ArtistId INT NOT NULL,  -- FK hacia Artists
    PreferenceLevel INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_UserArtistPreferences_Users FOREIGN KEY (FirebaseUid) REFERENCES Users(FirebaseUid) ON DELETE CASCADE,
    CONSTRAINT FK_UserArtistPreferences_Artists FOREIGN KEY (ArtistId) REFERENCES Artists(Id),
    CONSTRAINT UQ_UserArtistPreferences_User_Artist UNIQUE (FirebaseUid, ArtistId)
);

-- Tabla de swipes entre usuarios
CREATE TABLE UserSwipes (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,  -- PK
    SwiperUid NVARCHAR(128) NOT NULL,  -- FK hacia Users
    SwipedUid NVARCHAR(128) NOT NULL,  -- FK hacia Users
    SwipeType NVARCHAR(20) NOT NULL,
    SwipedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_UserSwipes_Swiper FOREIGN KEY (SwiperUid) REFERENCES Users(FirebaseUid) ON DELETE CASCADE,
    CONSTRAINT FK_UserSwipes_Swiped FOREIGN KEY (SwipedUid) REFERENCES Users(FirebaseUid),
    CONSTRAINT UQ_UserSwipes_Pair UNIQUE (SwiperUid, SwipedUid)
);

-- Tabla de matches entre usuarios
CREATE TABLE UserMatches (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,  -- PK
    User1Uid NVARCHAR(128) NOT NULL,  -- FK hacia Users
    User2Uid NVARCHAR(128) NOT NULL,  -- FK hacia Users
    MatchedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    CompatibilityScore DECIMAL(5,2) NULL,
    LastInteractionAt DATETIME NULL,
    CONSTRAINT FK_UserMatches_User1 FOREIGN KEY (User1Uid) REFERENCES Users(FirebaseUid) ON DELETE CASCADE,
    CONSTRAINT FK_UserMatches_User2 FOREIGN KEY (User2Uid) REFERENCES Users(FirebaseUid),
    CONSTRAINT UQ_UserMatches_Pair UNIQUE (User1Uid, User2Uid)
);

-- Tabla de cola de descubrimiento
CREATE TABLE UserDiscoveryQueue (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,  -- PK
    ForUserUid NVARCHAR(128) NOT NULL,  -- FK hacia Users
    CandidateUid NVARCHAR(128) NOT NULL,  -- FK hacia Users
    CompatibilityScore DECIMAL(5,2) DEFAULT 0.00,
    Distance DECIMAL(8,2) NULL,
    QueuePosition INT NOT NULL,
    IsShown BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    ExpiresAt DATETIME NULL,
    CONSTRAINT FK_UserDiscoveryQueue_ForUser FOREIGN KEY (ForUserUid) REFERENCES Users(FirebaseUid) ON DELETE CASCADE,
    CONSTRAINT FK_UserDiscoveryQueue_Candidate FOREIGN KEY (CandidateUid) REFERENCES Users(FirebaseUid),
    CONSTRAINT UQ_UserDiscoveryQueue_Pair UNIQUE (ForUserUid, CandidateUid)
);

-- =============================================
-- SPOTIFY INTEGRATION TABLES
-- =============================================

-- Tabla para almacenar tokens de Spotify por usuario
CREATE TABLE SpotifyTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,  -- PK
    FirebaseUid NVARCHAR(128) NOT NULL,  -- FK hacia Users
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