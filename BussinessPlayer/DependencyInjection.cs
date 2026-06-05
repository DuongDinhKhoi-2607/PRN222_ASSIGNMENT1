using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DataAccessLayer;
using BussinessLayer.Interfaces;
using BussinessLayer.Services;

namespace BussinessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, string connectionString, IConfiguration configuration)
        {
            // Configure DataAccess layer
            services.AddDataAccess(connectionString);
            // Register business services
            services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<ITextExtractionService, TextExtractionService>();
            services.AddScoped<IChunkingService, ChunkingService>();
            services.AddScoped<IEmbeddingService, EmbeddingService>();
            services.AddScoped<IRetrievalService, RetrievalService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<ILLMService, LLMService>();
            services.AddScoped<IMessageCitationService, CitationService>();
            services.AddScoped<ISeedService, SeedService>();
            services.AddScoped<IChapterService, ChapterService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPermissionService, PermissionService>();
            return services;
        }
    }
}