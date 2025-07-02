-- =============================================
-- SoulBeats - Datos Seed para Géneros y Artistas
-- Version: 1.0 - Simple INSERT statements
-- Description: Pobla las tablas Genres y Artists con datos iniciales para MVP
-- =============================================

-- =============================================
-- INSERTAR GÉNEROS MUSICALES
-- =============================================

INSERT INTO Genres (Name, Description, IconUrl, DisplayOrder, IsActive) VALUES
('Pop', 'Música popular comercial y pegadiza', 'https://example.com/icons/pop.png', 1, 1),
('Rock', 'Rock clásico y moderno', 'https://example.com/icons/rock.png', 2, 1),
('Hip Hop', 'Rap y cultura urbana', 'https://example.com/icons/hiphop.png', 3, 1),
('Electronic', 'Música electrónica y EDM', 'https://example.com/icons/electronic.png', 4, 1),
('R&B', 'Rhythm and Blues', 'https://example.com/icons/rnb.png', 5, 1),
('Jazz', 'Jazz clásico y moderno', 'https://example.com/icons/jazz.png', 6, 1),
('Country', 'Música country y folk', 'https://example.com/icons/country.png', 7, 1),
('Latin', 'Música latina y reggaeton', 'https://example.com/icons/latin.png', 8, 1),
('Indie', 'Música independiente', 'https://example.com/icons/indie.png', 9, 1),
('Classical', 'Música clásica', 'https://example.com/icons/classical.png', 10, 1),
('Reggae', 'Reggae y música jamaicana', 'https://example.com/icons/reggae.png', 11, 1),
('Blues', 'Blues tradicional y moderno', 'https://example.com/icons/blues.png', 12, 1),
('Funk', 'Funk y soul', 'https://example.com/icons/funk.png', 13, 1),
('Punk', 'Punk rock y hardcore', 'https://example.com/icons/punk.png', 14, 1),
('Metal', 'Heavy metal y subgéneros', 'https://example.com/icons/metal.png', 15, 1);

-- =============================================
-- INSERTAR ARTISTAS POR GÉNERO
-- =============================================

-- Artistas de Pop (GenreId = 1)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Taylor Swift', '06HL4z0CvFAxyc27GXpf02', 'https://i.scdn.co/image/ab6761610000e5eb859e4c14fa59296c8649e0e4', 1, 95, 1),
('Ariana Grande', '66CXWjxzNUsdJxJ2JdwvnR', 'https://i.scdn.co/image/ab6761610000e5ebb4e9c2b8ee8e5c4d9f0c9c1c', 1, 93, 1),
('Ed Sheeran', '6eUKZXaKkcviH0Ku9w2n3V', 'https://i.scdn.co/image/ab6761610000e5eb1e3b8b7a8f5c2d1e2f1c1d2a', 1, 92, 1),
('Dua Lipa', '6M2wZ9GZgrQXHCFfjv46we', 'https://i.scdn.co/image/ab6761610000e5eb8f8b8c1a9e5d3c2b4f6e1a3d', 1, 91, 1),
('Billie Eilish', '6qqNVTkY8uBg9cP3Jd8DAH', 'https://i.scdn.co/image/ab6761610000e5eb3f7a2b5e8c9d1f4e6b3a7c8e', 1, 90, 1),
('The Weeknd', '1Xyo4u8uXC1ZmMpatF05PJ', 'https://i.scdn.co/image/ab6761610000e5eb2a8b3c5d7f9e1a4c6b8d3f2a', 1, 94, 1),
('Harry Styles', '6KImCVD70vtIoJWnq6nGn3', 'https://i.scdn.co/image/ab6761610000e5eb4c8f9a2d5e3b7a1f6c8d9e4b', 1, 89, 1),
('Olivia Rodrigo', '1McMsnEElThX1knmY9oliG', 'https://i.scdn.co/image/ab6761610000e5eb5f3a2c8d9e1b4f7a6c3d8e2b', 1, 88, 1);

-- Artistas de Rock (GenreId = 2)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Queen', '1dfeR4HaWDbWqFHLkxsg1d', 'https://i.scdn.co/image/ab6761610000e5eb3c8b7a5d2f9e1a6c4b8d3f7a', 2, 96, 1),
('The Beatles', '3WrFJ7ztbogyGnTHbHJFl2', 'https://i.scdn.co/image/ab6761610000e5eb2f8c9a4d6e1b5a7c3f6d8e9b', 2, 97, 1),
('Led Zeppelin', '36QJpDe2go2KgaRleHCDTp', 'https://i.scdn.co/image/ab6761610000e5eb5a3c8f9d2e1b7a4c6f8d3e2b', 2, 95, 1),
('Pink Floyd', '0k17h0D3J5VfsdmQ1iZtE9', 'https://i.scdn.co/image/ab6761610000e5eb7f2a5c8d9e3b1a6c4f8d7e5b', 2, 94, 1),
('AC/DC', '711MCceyCBcFnzjGY4Q7Un', 'https://i.scdn.co/image/ab6761610000e5eb4c8f7a2d5e9b3a1c6f8d4e7b', 2, 93, 1),
('The Rolling Stones', '22bE4uQ6baNwSHPVcDxLCe', 'https://i.scdn.co/image/ab6761610000e5eb2f5a8c9d3e1b6a4c7f8d2e5b', 2, 92, 1),
('Nirvana', '6olE6TJLqED3rqDCT0FyPh', 'https://i.scdn.co/image/ab6761610000e5eb8f3a2c5d9e1b4a7c6f8d3e2b', 2, 91, 1),
('Foo Fighters', '7jy3rLJdDQY21OgRLCZ9sD', 'https://i.scdn.co/image/ab6761610000e5eb5c3f8a2d9e1b7a4c6f8d5e3b', 2, 90, 1);

-- Artistas de Hip Hop (GenreId = 3)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Drake', '3TVXtAsR1Inumwj472S9r4', 'https://i.scdn.co/image/ab6761610000e5eb4f8a2c5d9e3b1a7c6f8d4e2b', 3, 96, 1),
('Kendrick Lamar', '2YZyLoL8N0Wb9xBt1NhZWg', 'https://i.scdn.co/image/ab6761610000e5eb2c5f8a9d3e1b4a7c6f8d2e5b', 3, 95, 1),
('Kanye West', '5K4W6rqBFWDnAN6FQUkS6x', 'https://i.scdn.co/image/ab6761610000e5eb8a3c5f9d2e1b7a4c6f8d3e2b', 3, 94, 1),
('Eminem', '7dGJo4pcD2V6oG8kP0tJRR', 'https://i.scdn.co/image/ab6761610000e5eb5f3a8c2d9e1b4a7c6f8d5e3b', 3, 93, 1),
('Jay-Z', '3nFkdlSjzX9mRTtwJOzDYB', 'https://i.scdn.co/image/ab6761610000e5eb3c8f5a2d9e1b7a4c6f8d3e2b', 3, 92, 1),
('Travis Scott', '0Y5tJX1MQlPlqiwlOH1tJY', 'https://i.scdn.co/image/ab6761610000e5eb8f2a5c9d3e1b4a7c6f8d2e5b', 3, 91, 1),
('J. Cole', '6l3HvQ5sa6mXTsMTB19rO5', 'https://i.scdn.co/image/ab6761610000e5eb2f5a8c3d9e1b7a4c6f8d2e5b', 3, 90, 1),
('Lil Wayne', '55Aa2cqylxrFIXC767Z865', 'https://i.scdn.co/image/ab6761610000e5eb5c3f8a9d2e1b4a7c6f8d5e3b', 3, 89, 1);

-- Artistas de Electronic (GenreId = 4)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Calvin Harris', '7CajNmpbOovFoOoasH2HaY', 'https://i.scdn.co/image/ab6761610000e5eb8f3a2c5d9e1b4a7c6f8d3e2b', 4, 89, 1),
('David Guetta', '1Cs0zKBU1kc0i8ypK3B9ai', 'https://i.scdn.co/image/ab6761610000e5eb2c5f8a9d3e1b7a4c6f8d2e5b', 4, 88, 1),
('Skrillex', '5he5w2lnU9x7JFhnwcekXX', 'https://i.scdn.co/image/ab6761610000e5eb5f3a8c2d9e1b4a7c6f8d5e3b', 4, 87, 1),
('Deadmau5', '2CIMQHirSU0MQqyYHq0eOx', 'https://i.scdn.co/image/ab6761610000e5eb3c8f5a2d9e1b7a4c6f8d3e2b', 4, 86, 1),
('Tiësto', '2o5jDhtHVPhrJdv3cEQ99Z', 'https://i.scdn.co/image/ab6761610000e5eb8f2a5c9d3e1b4a7c6f8d2e5b', 4, 85, 1),
('Avicii', '1vCWHaC5f2uS3yhpwWbIA6', 'https://i.scdn.co/image/ab6761610000e5eb2f5a8c3d9e1b7a4c6f8d2e5b', 4, 84, 1),
('Marshmello', '64KEffDW9EtZ1y2vBYgq8T', 'https://i.scdn.co/image/ab6761610000e5eb5c3f8a9d2e1b4a7c6f8d5e3b', 4, 83, 1),
('Diplo', '5fMUXHkw8R8eOP2RNVYEZX', 'https://i.scdn.co/image/ab6761610000e5eb8f3a2c5d9e1b4a7c6f8d3e2b', 4, 82, 1);

-- Artistas de R&B (GenreId = 5)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Beyoncé', '6vWDO969PvNqNYHIOW5v0m', 'https://i.scdn.co/image/ab6761610000e5eb2c5f8a9d3e1b7a4c6f8d2e5b', 5, 96, 1),
('Rihanna', '5pKCCKE2ajJHZ9KAiaK11H', 'https://i.scdn.co/image/ab6761610000e5eb5f3a8c2d9e1b4a7c6f8d5e3b', 5, 95, 1),
('Frank Ocean', '2h93pZq0e7k5yf4dywlkpM', 'https://i.scdn.co/image/ab6761610000e5eb8f2a5c9d3e1b4a7c6f8d2e5b', 5, 93, 1),
('Alicia Keys', '3DiDSECUqqY1AuBP8qtaIa', 'https://i.scdn.co/image/ab6761610000e5eb2f5a8c3d9e1b7a4c6f8d2e5b', 5, 92, 1),
('John Legend', '5y2Xq6xcjJb2jVM54GHK3t', 'https://i.scdn.co/image/ab6761610000e5eb5c3f8a9d2e1b4a7c6f8d5e3b', 5, 91, 1),
('Usher', '23zg3TcAtWQy7J6upgbUnj', 'https://i.scdn.co/image/ab6761610000e5eb8f3a2c5d9e1b4a7c6f8d3e2b', 5, 90, 1),
('Bruno Mars', '0du5cEVh5yTK9QJze8zA0C', 'https://i.scdn.co/image/ab6761610000e5eb2c5f8a9d3e1b7a4c6f8d2e5b', 5, 89, 1);

-- Artistas de Jazz (GenreId = 6)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Miles Davis', '0kbYTNQb4Pb1rPbbaF0pT4', 'https://i.scdn.co/image/ab6761610000e5eb5f3a8c2d9e1b4a7c6f8d5e3b', 6, 85, 1),
('John Coltrane', '2hGh5VOeeqimQFxqXvZZKX', 'https://i.scdn.co/image/ab6761610000e5eb3c8f5a2d9e1b7a4c6f8d3e2b', 6, 84, 1),
('Louis Armstrong', '2mpMx8vJDJwwBrmLZsIbQJ', 'https://i.scdn.co/image/ab6761610000e5eb8f2a5c9d3e1b7a4c6f8d2e5b', 6, 83, 1),
('Ella Fitzgerald', '5l7VQObBaYLJGQWqnTqsGo', 'https://i.scdn.co/image/ab6761610000e5eb2f5a8c3d9e1b7a4c6f8d2e5b', 6, 82, 1),
('Duke Ellington', '4F7Q5NV6h5TSwCainz3o6u', 'https://i.scdn.co/image/ab6761610000e5eb5c3f8a9d2e1b4a7c6f8d5e3b', 6, 81, 1),
('Billie Holiday', '4TKTii6gnOnUXQHyuo9gzO', 'https://i.scdn.co/image/ab6761610000e5eb8f3a2c5d9e1b4a7c6f8d3e2b', 6, 80, 1);

-- Artistas de Country (GenreId = 7)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Johnny Cash', '6kACVPfCOnqzgfEF5ryl0x', 'https://i.scdn.co/image/ab6761610000e5eb5f3a8c2d9e1b4a7c6f8d5e3b', 7, 87, 1),
('Dolly Parton', '32vWCbZh0xZ4o9gkz4PsEU', 'https://i.scdn.co/image/ab6761610000e5eb3c8f5a2d9e1b7a4c6f8d3e2b', 7, 86, 1),
('Keith Urban', '2yqAFIdE7vcVL2sMtNTAhf', 'https://i.scdn.co/image/ab6761610000e5eb8f2a5c9d3e1b4a7c6f8d2e5b', 7, 85, 1),
('Blake Shelton', '1UTPBmNbXNTittyMJrNkvw', 'https://i.scdn.co/image/ab6761610000e5eb2f5a8c3d9e1b7a4c6f8d2e5b', 7, 84, 1),
('Carrie Underwood', '4xFUf1FHVy696Q1JQZMTRj', 'https://i.scdn.co/image/ab6761610000e5eb5c3f8a9d2e1b4a7c6f8d5e3b', 7, 83, 1);

-- Artistas de Latin (GenreId = 8)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Bad Bunny', '4q3ewBCX7sLwd24euuV69X', 'https://i.scdn.co/image/ab6761610000e5eb8f3a2c5d9e1b4a7c6f8d3e2b', 8, 92, 1),
('J Balvin', '1vyhD5VmyZ7KMfW5gqLgo5', 'https://i.scdn.co/image/ab6761610000e5eb2c5f8a9d3e1b7a4c6f8d2e5b', 8, 91, 1),
('Shakira', '0EmeFodog0BfCgMzAIvKQp', 'https://i.scdn.co/image/ab6761610000e5eb5f3a8c2d9e1b4a7c6f8d5e3b', 8, 90, 1),
('Daddy Yankee', '6LuN9FCkKOj5PcnpouEgny', 'https://i.scdn.co/image/ab6761610000e5eb3c8f5a2d9e1b7a4c6f8d3e2b', 8, 89, 1),
('Ozuna', '1i8SpTcr7yvPOmcqrbnVXY', 'https://i.scdn.co/image/ab6761610000e5eb8f2a5c9d3e1b4a7c6f8d2e5b', 8, 88, 1),
('Maluma', '1r4hJ1h58CWwUQe3MxPuau', 'https://i.scdn.co/image/ab6761610000e5eb2f5a8c3d9e1b7a4c6f8d2e5b', 8, 87, 1);

-- Artistas de Indie (GenreId = 9)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Arctic Monkeys', '7Ln80lUS6He07XvHI8qqHH', 'https://i.scdn.co/image/ab6761610000e5eb5c3f8a9d2e1b4a7c6f8d5e3b', 9, 88, 1),
('Tame Impala', '5INjqkS1o8h1imAzPqGZBb', 'https://i.scdn.co/image/ab6761610000e5eb8f3a2c5d9e1b4a7c6f8d3e2b', 9, 87, 1),
('The Strokes', '0epOFNiUfyON9EYx7Tpr6V', 'https://i.scdn.co/image/ab6761610000e5eb2c5f8a9d3e1b7a4c6f8d2e5b', 9, 86, 1),
('Vampire Weekend', '5BvJzeQpmsdsFp4HGUYUEx', 'https://i.scdn.co/image/ab6761610000e5eb5f3a8c2d9e1b4a7c6f8d5e3b', 9, 85, 1),
('Radiohead', '4Z8W4fKeB5YxbusRsdQVPb', 'https://i.scdn.co/image/ab6761610000e5eb3c8f5a2d9e1b7a4c6f8d3e2b', 9, 84, 1),
('Bon Iver', '4LEiUm1SRbFMgfqnQTwUbQ', 'https://i.scdn.co/image/ab6761610000e5eb8f2a5c9d3e1b7a4c6f8d2e5b', 9, 83, 1);

-- Artistas de Classical (GenreId = 10)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Ludwig van Beethoven', '2wOqMjp9TyABvtHdOSOTUS', 'https://i.scdn.co/image/ab6761610000e5eb2f5a8c3d9e1b7a4c6f8d2e5b', 10, 90, 1),
('Wolfgang Amadeus Mozart', '4NJhFmfw43RLBLjQvxDuRS', 'https://i.scdn.co/image/ab6761610000e5eb5c3f8a9d2e1b4a7c6f8d5e3b', 10, 89, 1),
('Johann Sebastian Bach', '5aIqB5nVVvmFsvSdExz408', 'https://i.scdn.co/image/ab6761610000e5eb8f3a2c5d9e1b4a7c6f8d3e2b', 10, 88, 1),
('Frédéric Chopin', '7y97mc3bZRFXzT2szRM4L4', 'https://i.scdn.co/image/ab6761610000e5eb2c5f8a9d3e1b7a4c6f8d2e5b', 10, 87, 1);

-- Artistas de Reggae (GenreId = 11)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Bob Marley', '2QsynagSdAqZj3U9HgDzjD', 'https://i.scdn.co/image/ab6761610000e5eb5f3a8c2d9e1b4a7c6f8d5e3b', 11, 92, 1),
('Jimmy Buffett', '4A4o6EA2TkWDldPQNE3sTX', 'https://i.scdn.co/image/ab6761610000e5eb3c8f5a2d9e1b7a4c6f8d3e2b', 11, 80, 1),
('Sean Paul', '3Isy6kedDrgPYoTS1dazA9', 'https://i.scdn.co/image/ab6761610000e5eb8f2a5c9d3e1b4a7c6f8d2e5b', 11, 79, 1);

-- Artistas de Blues (GenreId = 12)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('B.B. King', '5WGpBtjYU56oPaFClzJUUZ', 'https://i.scdn.co/image/ab6761610000e5eb2f5a8c3d9e1b7a4c6f8d2e5b', 12, 85, 1),
('Eric Clapton', '6PAt558ZEZl0DmdXlnjMgD', 'https://i.scdn.co/image/ab6761610000e5eb5c3f8a9d2e1b4a7c6f8d5e3b', 12, 84, 1),
('Muddy Waters', '2cCB5RwPVjvCOnz9nNK7bN', 'https://i.scdn.co/image/ab6761610000e5eb8f3a2c5d9e1b4a7c6f8d3e2b', 12, 83, 1);

-- Artistas de Funk (GenreId = 13)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('James Brown', '7fObpalQze2gLP9Rae5Q5A', 'https://i.scdn.co/image/ab6761610000e5eb2c5f8a9d3e1b7a4c6f8d2e5b', 13, 88, 1),
('Parliament-Funkadelic', '2bGKBXJLO8D8d5Qj7zYklN', 'https://i.scdn.co/image/ab6761610000e5eb5f3a8c2d9e1b4a7c6f8d5e3b', 13, 82, 1),
('Prince', '5a2EaR3hamoenG9rDuVn8j', 'https://i.scdn.co/image/ab6761610000e5eb3c8f5a2d9e1b7a4c6f8d3e2b', 13, 87, 1);

-- Artistas de Punk (GenreId = 14)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Green Day', '7oPftvlwr6VrsViSDV7fJY', 'https://i.scdn.co/image/ab6761610000e5eb8f2a5c9d3e1b4a7c6f8d2e5b', 14, 85, 1),
('The Clash', '3RGLhK1IP9jnYFH4BRFJBS', 'https://i.scdn.co/image/ab6761610000e5eb2f5a8c3d9e1b7a4c6f8d2e5b', 14, 84, 1),
('Ramones', '1co4F2pPNH8JjTutZkmgSm', 'https://i.scdn.co/image/ab6761610000e5eb5c3f8a9d2e1b4a7c6f8d5e3b', 14, 83, 1);

-- Artistas de Metal (GenreId = 15)
INSERT INTO Artists (Name, SpotifyId, ImageUrl, GenreId, Popularity, IsActive) VALUES
('Metallica', '2ye2Wgw4gimLv2eAKyk1NB', 'https://i.scdn.co/image/ab6761610000e5eb8f3a2c5d9e1b4a7c6f8d3e2b', 15, 90, 1),
('Iron Maiden', '6mdiAmATAx73kdxrNrnlao', 'https://i.scdn.co/image/ab6761610000e5eb2c5f8a9d3e1b7a4c6f8d2e5b', 15, 89, 1),
('Black Sabbath', '5M52tdBnJaKSvOpJGz8mfZ', 'https://i.scdn.co/image/ab6761610000e5eb5f3a8c2d9e1b4a7c6f8d5e3b', 15, 88, 1),
('Slayer', '1IQ2nN4QIVJljfOK3CaZim', 'https://i.scdn.co/image/ab6761610000e5eb3c8f5a2d9e1b7a4c6f8d3e2b', 15, 87, 1);

-- =============================================
-- VERIFICAR DATOS INSERTADOS
-- =============================================

SELECT 
    'Géneros insertados' as Descripcion,
    COUNT(*) as Cantidad
FROM Genres
WHERE IsActive = 1

UNION ALL

SELECT 
    'Artistas insertados',
    COUNT(*)
FROM Artists
WHERE IsActive = 1;