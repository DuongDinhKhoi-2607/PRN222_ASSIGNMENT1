using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DataAccessPlayer.Models;

namespace DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
        {
            // Configure DbContext
            services.AddDbContext<RagchatbotDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register Repositories
            services.AddScoped<Repositories.DocumentRepository>();
            services.AddScoped<Repositories.DocumentChunkRepository>();
            services.AddScoped<Repositories.ChunkEmbeddingRepository>();
            services.AddScoped<Repositories.SubjectRepository>();
            services.AddScoped<Repositories.ChatSessionRepository>();
            services.AddScoped<Repositories.ChatMessageRepository>();
            services.AddScoped<Repositories.MessageCitationRepository>();
            services.AddScoped<Repositories.UserRepository>();
            services.AddScoped<Repositories.LecturerUploadPermissionRepository>();

            return services;
        }
    }
}