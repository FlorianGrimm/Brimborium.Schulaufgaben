// MIT - Florian Grimm

using Microsoft.EntityFrameworkCore;

namespace Brimborium.Schulaufgaben.Data;

public class MediaCacheContext : DbContext {
    public DbSet<DbFile> ListFile { get; set; }
    public DbSet<DbFolder> ListFolder { get; set; }

    public string DbPath { get; }

    public MediaCacheContext() {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        this.DbPath = System.IO.Path.Join(path, "mediacache.db");
    }

    public MediaCacheContext(string dbPath) {
        this.DbPath = dbPath;
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // Configure composite primary key for DbFile
        modelBuilder.Entity<DbFile>()
            .HasKey(f => new { f.MediaGalleryId, f.Path });

        // Configure composite primary key for DbFolder
        modelBuilder.Entity<DbFolder>()
            .HasKey(f => new { f.MediaGalleryId, f.Folder });

        // Add indexes for better search performance
        modelBuilder.Entity<DbFile>()
            .HasIndex(f => f.Name);

        modelBuilder.Entity<DbFile>()
            .HasIndex(f => f.LastScan);
    }
}

public class DbFolder {
    public required string MediaGalleryId { get; set; }
    public required string Folder { get; set; }
}

public class DbFile {
    public required string MediaGalleryId { get; set; }
    public required string MediaType { get; set; }
    public required string Folder { get; set; }
    public required string Path { get; set; }
    public required string Name { get; set; }
    public required DateTime LastWriteTimeUtc { get; set; }
    public required long Size { get; set; }
    public required DateTime LastScan { get; set; }
    public byte[]? Thumbnail { get; set; }
}