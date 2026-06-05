using System;
using DataAccessPlayer.Models;
using Microsoft.EntityFrameworkCore;
using BussinessLayer.Interfaces;

namespace BussinessLayer.Services
{
    public class SeedService : ISeedService
    {
        private readonly RagchatbotDbContext _db;

        public SeedService(RagchatbotDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            // 0. Auto-migration: Add user_id to documents table if not exists
            try
            {
                await _db.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (
                        SELECT * FROM sys.columns 
                        WHERE object_id = OBJECT_ID('documents') AND name = 'user_id'
                    )
                    BEGIN
                        ALTER TABLE documents ADD user_id BIGINT NULL;
                    END
                ");
                Console.WriteLine("[Seed] Verified user_id column in documents table");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Seed] Error adding user_id column: " + ex.Message);
            }

            // 1. ChunkingStrategy id=1
            if (!await _db.ChunkingStrategies.AnyAsync(s => s.Id == 1))
            {
                await _db.Database.ExecuteSqlRawAsync(@"
                    SET IDENTITY_INSERT chunking_strategies ON;
                    INSERT INTO chunking_strategies (id, name, chunk_size, chunk_overlap, description)
                    VALUES (1, 'Fixed-size (1000 tokens)', 1000, 100, 'Default fixed-size chunking strategy');
                    SET IDENTITY_INSERT chunking_strategies OFF;");
                Console.WriteLine("[Seed] Created default ChunkingStrategy (id=1)");
            }

            // 2. EmbeddingModel id=1
            if (!await _db.EmbeddingModels.AnyAsync(m => m.Id == 1))
            {
                await _db.Database.ExecuteSqlRawAsync(@"
                    SET IDENTITY_INSERT embedding_models ON;
                    INSERT INTO embedding_models (id, name, provider, dimension, is_free, description)
                    VALUES (1, 'text-embedding-3-small', 'OpenAI', 1536, 0, 'OpenAI text-embedding-3-small (1536 dims)');
                    SET IDENTITY_INSERT embedding_models OFF;");
                Console.WriteLine("[Seed] Created default EmbeddingModel (id=1)");
            }

            // 3. User id=1
            if (!await _db.Users.AnyAsync(u => u.Id == 1))
            {
                string hashed = PasswordHelper.HashPassword("admin123");
                await _db.Database.ExecuteSqlRawAsync($@"
                    SET IDENTITY_INSERT users ON;
                    INSERT INTO users (id, full_name, email, role, password_hash, is_active, created_at)
                    VALUES (1, 'Demo User', 'demo@ragassistant.local', 'admin', '{hashed}', 1, GETDATE());
                    SET IDENTITY_INSERT users OFF;");
                Console.WriteLine("[Seed] Created demo User (id=1) with hashed password admin123");
            }
        }
    }
}
