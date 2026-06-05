using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccessPlayer.Models;

public partial class RagchatbotDbContext : DbContext
{
    public RagchatbotDbContext()
    {
    }

    public RagchatbotDbContext(DbContextOptions<RagchatbotDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<ChatSession> ChatSessions { get; set; }

    public virtual DbSet<ChunkEmbedding> ChunkEmbeddings { get; set; }

    public virtual DbSet<ChunkingStrategy> ChunkingStrategies { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<DocumentChunk> DocumentChunks { get; set; }

    public virtual DbSet<EmbeddingModel> EmbeddingModels { get; set; }

    public virtual DbSet<EvaluationResult> EvaluationResults { get; set; }

    public virtual DbSet<Experiment> Experiments { get; set; }

    public virtual DbSet<ExperimentRun> ExperimentRuns { get; set; }

    public virtual DbSet<ExperimentRunMetric> ExperimentRunMetrics { get; set; }

    public virtual DbSet<LecturerUploadPermission> LecturerUploadPermissions { get; set; }

    public virtual DbSet<LlmModel> LlmModels { get; set; }

    public virtual DbSet<MessageCitation> MessageCitations { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<TestQuestion> TestQuestions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Database=RAGChatbotDB;User Id=sa;Password=12345;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__chapters__3213E83F782FE85F");

            entity.ToTable("chapters");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Subject).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_chapters_subjects");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__chat_mes__3213E83F7418D67E");

            entity.ToTable("chat_messages");

            entity.HasIndex(e => e.SessionId, "IX_chat_messages_session_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.LatencyMs).HasColumnName("latency_ms");
            entity.Property(e => e.LlmModelId).HasColumnName("llm_model_id");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.TokenUsage).HasColumnName("token_usage");

            entity.HasOne(d => d.LlmModel).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.LlmModelId)
                .HasConstraintName("FK_chat_messages_llm_models");

            entity.HasOne(d => d.Session).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_chat_messages_sessions");
        });

        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__chat_ses__3213E83F407D3E6A");

            entity.ToTable("chat_sessions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Subject).WithMany(p => p.ChatSessions)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_chat_sessions_subjects");

            entity.HasOne(d => d.User).WithMany(p => p.ChatSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_chat_sessions_users");
        });

        modelBuilder.Entity<ChunkEmbedding>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__chunk_em__3213E83F0F99E05F");

            entity.ToTable("chunk_embeddings");

            entity.HasIndex(e => e.ChunkId, "IX_chunk_embeddings_chunk_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChunkId).HasColumnName("chunk_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Dimension).HasColumnName("dimension");
            entity.Property(e => e.EmbeddingModelId).HasColumnName("embedding_model_id");
            entity.Property(e => e.Vector).HasColumnName("vector");

            entity.HasOne(d => d.Chunk).WithMany(p => p.ChunkEmbeddings)
                .HasForeignKey(d => d.ChunkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_chunk_embeddings_chunks");

            entity.HasOne(d => d.EmbeddingModel).WithMany(p => p.ChunkEmbeddings)
                .HasForeignKey(d => d.EmbeddingModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_chunk_embeddings_embedding_models");
        });

        modelBuilder.Entity<ChunkingStrategy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__chunking__3213E83FD2ADE4EA");

            entity.ToTable("chunking_strategies");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChunkOverlap).HasColumnName("chunk_overlap");
            entity.Property(e => e.ChunkSize).HasColumnName("chunk_size");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Params).HasColumnName("params");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__document__3213E83FF82979FB");

            entity.ToTable("documents");

            entity.HasIndex(e => e.SubjectId, "IX_documents_subject_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChapterId).HasColumnName("chapter_id");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(500)
                .HasColumnName("file_path");
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.FileType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("file_type");
            entity.Property(e => e.IndexedAt).HasColumnName("indexed_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_documents_chapters");

            entity.HasOne(d => d.Subject).WithMany(p => p.Documents)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_documents_subjects");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Documents)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK_documents_uploaded_by");
        });

        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__document__3213E83F596EE117");

            entity.ToTable("document_chunks");

            entity.HasIndex(e => e.DocumentId, "IX_document_chunks_document_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChunkIndex).HasColumnName("chunk_index");
            entity.Property(e => e.ChunkingStrategyId).HasColumnName("chunking_strategy_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.Metadata).HasColumnName("metadata");
            entity.Property(e => e.PageNumber).HasColumnName("page_number");
            entity.Property(e => e.TokenCount).HasColumnName("token_count");

            entity.HasOne(d => d.ChunkingStrategy).WithMany(p => p.DocumentChunks)
                .HasForeignKey(d => d.ChunkingStrategyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_document_chunks_chunking_strategies");

            entity.HasOne(d => d.Document).WithMany(p => p.DocumentChunks)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_document_chunks_documents");
        });

        modelBuilder.Entity<EmbeddingModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__embeddin__3213E83FCEA11ABF");

            entity.ToTable("embedding_models");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Dimension).HasColumnName("dimension");
            entity.Property(e => e.IsFree)
                .HasDefaultValue(true)
                .HasColumnName("is_free");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Provider)
                .HasMaxLength(100)
                .HasColumnName("provider");
        });

        modelBuilder.Entity<EvaluationResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__evaluati__3213E83FC3477564");

            entity.ToTable("evaluation_results");

            entity.HasIndex(e => e.ExperimentRunId, "IX_evaluation_results_run_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerCorrectness).HasColumnName("answer_correctness");
            entity.Property(e => e.AnswerRelevancy).HasColumnName("answer_relevancy");
            entity.Property(e => e.ContextPrecision).HasColumnName("context_precision");
            entity.Property(e => e.ContextRecall).HasColumnName("context_recall");
            entity.Property(e => e.ExperimentRunId).HasColumnName("experiment_run_id");
            entity.Property(e => e.Faithfulness).HasColumnName("faithfulness");
            entity.Property(e => e.GeneratedAnswer).HasColumnName("generated_answer");
            entity.Property(e => e.LatencyMs).HasColumnName("latency_ms");
            entity.Property(e => e.RetrievedContexts).HasColumnName("retrieved_contexts");
            entity.Property(e => e.TestQuestionId).HasColumnName("test_question_id");

            entity.HasOne(d => d.ExperimentRun).WithMany(p => p.EvaluationResults)
                .HasForeignKey(d => d.ExperimentRunId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_evaluation_results_runs");

            entity.HasOne(d => d.TestQuestion).WithMany(p => p.EvaluationResults)
                .HasForeignKey(d => d.TestQuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_evaluation_results_questions");
        });

        modelBuilder.Entity<Experiment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__experime__3213E83FF34790E9");

            entity.ToTable("experiments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<ExperimentRun>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__experime__3213E83FCB0BF4BC");

            entity.ToTable("experiment_runs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChunkingStrategyId).HasColumnName("chunking_strategy_id");
            entity.Property(e => e.EmbeddingModelId).HasColumnName("embedding_model_id");
            entity.Property(e => e.ExperimentId).HasColumnName("experiment_id");
            entity.Property(e => e.FinishedAt).HasColumnName("finished_at");
            entity.Property(e => e.LlmModelId).HasColumnName("llm_model_id");
            entity.Property(e => e.Params).HasColumnName("params");
            entity.Property(e => e.RunName)
                .HasMaxLength(255)
                .HasColumnName("run_name");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("started_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.ChunkingStrategy).WithMany(p => p.ExperimentRuns)
                .HasForeignKey(d => d.ChunkingStrategyId)
                .HasConstraintName("FK_experiment_runs_chunking_strategies");

            entity.HasOne(d => d.EmbeddingModel).WithMany(p => p.ExperimentRuns)
                .HasForeignKey(d => d.EmbeddingModelId)
                .HasConstraintName("FK_experiment_runs_embedding_models");

            entity.HasOne(d => d.Experiment).WithMany(p => p.ExperimentRuns)
                .HasForeignKey(d => d.ExperimentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_experiment_runs_experiments");

            entity.HasOne(d => d.LlmModel).WithMany(p => p.ExperimentRuns)
                .HasForeignKey(d => d.LlmModelId)
                .HasConstraintName("FK_experiment_runs_llm_models");
        });

        modelBuilder.Entity<ExperimentRunMetric>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__experime__3213E83F6422B2D7");

            entity.ToTable("experiment_run_metrics");

            entity.HasIndex(e => e.ExperimentRunId, "UQ__experime__F6927B65EA598F54").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvgAnswerCorrectness).HasColumnName("avg_answer_correctness");
            entity.Property(e => e.AvgAnswerRelevancy).HasColumnName("avg_answer_relevancy");
            entity.Property(e => e.AvgContextPrecision).HasColumnName("avg_context_precision");
            entity.Property(e => e.AvgContextRecall).HasColumnName("avg_context_recall");
            entity.Property(e => e.AvgFaithfulness).HasColumnName("avg_faithfulness");
            entity.Property(e => e.AvgLatencyMs).HasColumnName("avg_latency_ms");
            entity.Property(e => e.ExperimentRunId).HasColumnName("experiment_run_id");
            entity.Property(e => e.TotalQuestions).HasColumnName("total_questions");

            entity.HasOne(d => d.ExperimentRun).WithOne(p => p.ExperimentRunMetric)
                .HasForeignKey<ExperimentRunMetric>(d => d.ExperimentRunId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_experiment_run_metrics_runs");
        });

        modelBuilder.Entity<LecturerUploadPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__lecturer__3213E83F9ECC3317");

            entity.ToTable("lecturer_upload_permissions");

            entity.HasIndex(e => new { e.LecturerId, e.SubjectId }, "UQ_lup_lecturer_subject").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CanUpload)
                .HasDefaultValue(true)
                .HasColumnName("can_upload");
            entity.Property(e => e.GrantedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("granted_at");
            entity.Property(e => e.GrantedBy).HasColumnName("granted_by");
            entity.Property(e => e.LecturerId).HasColumnName("lecturer_id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.GrantedByNavigation).WithMany(p => p.LecturerUploadPermissionGrantedByNavigations)
                .HasForeignKey(d => d.GrantedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_lup_admin");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.LecturerUploadPermissionLecturers)
                .HasForeignKey(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_lup_lecturer");

            entity.HasOne(d => d.Subject).WithMany(p => p.LecturerUploadPermissions)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_lup_subject");
        });

        modelBuilder.Entity<LlmModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__llm_mode__3213E83F46C3AEF1");

            entity.ToTable("llm_models");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BaseModel)
                .HasMaxLength(255)
                .HasColumnName("base_model");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Provider)
                .HasMaxLength(100)
                .HasColumnName("provider");
            entity.Property(e => e.Type)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<MessageCitation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__message___3213E83F92D4A97D");

            entity.ToTable("message_citations");

            entity.HasIndex(e => e.MessageId, "IX_message_citations_message_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChunkId).HasColumnName("chunk_id");
            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.RelevanceScore).HasColumnName("relevance_score");
            entity.Property(e => e.Snippet).HasColumnName("snippet");

            entity.HasOne(d => d.Chunk).WithMany(p => p.MessageCitations)
                .HasForeignKey(d => d.ChunkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_message_citations_chunks");

            entity.HasOne(d => d.Document).WithMany(p => p.MessageCitations)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_message_citations_documents");

            entity.HasOne(d => d.Message).WithMany(p => p.MessageCitations)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_message_citations_messages");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__subjects__3213E83FBA8594F9");

            entity.ToTable("subjects");

            entity.HasIndex(e => e.Code, "UQ__subjects__357D4CF946ECB98D").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TestQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__test_que__3213E83FE4FFB103");

            entity.ToTable("test_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Difficulty)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("difficulty");
            entity.Property(e => e.GroundTruth).HasColumnName("ground_truth");
            entity.Property(e => e.Question).HasColumnName("question");
            entity.Property(e => e.ReferenceContext).HasColumnName("reference_context");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Subject).WithMany(p => p.TestQuestions)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_test_questions_subjects");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FB6717778");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E616427A7A071").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
