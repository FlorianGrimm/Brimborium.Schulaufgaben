// MIT - Florian Grimm

using Microsoft.Data.Sqlite;
using Microsoft.IO;

namespace Brimborium.Schulaufgaben.Data;

public class MediaCacheContext {
    public readonly Lock Lock = new();
    private SqliteConnection? _SqliteConnection;

    public string DbPath { get; }

    public MediaCacheContext() {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        this.DbPath = System.IO.Path.Join(path, "mediacache.db");
    }

    public MediaCacheContext(string dbPath) {
        this.DbPath = dbPath;
    }

    public SqliteConnection GetConnection() {
        //if (this._SqliteConnection is { } result) {
        //    result.State == 
        //    result.Open();
        //    return result;
        //}
        {
            Microsoft.Data.Sqlite.SqliteConnectionStringBuilder csb = new();
            csb.DataSource = this.DbPath;
            csb.Mode = SqliteOpenMode.ReadWriteCreate;
            csb.Cache = SqliteCacheMode.Shared;
            SqliteConnection sqliteConnection = new SqliteConnection(csb.ConnectionString);
            sqliteConnection.Open();
            //return this._SqliteConnection = sqliteConnection;
            return sqliteConnection;
        }
    }

    public void EnsureCreated() {

        const string sqlStatement = """
            CREATE TABLE IF NOT EXISTS ListFile (
                MediaGalleryId   TEXT    NOT NULL,
                Path             TEXT    NOT NULL,
                MediaType        TEXT    NOT NULL,
                Folder           TEXT    NOT NULL,
                Name             TEXT    NOT NULL,
                LastWriteTimeUtc TEXT    NOT NULL,
                Size             INTEGER NOT NULL,
                LastScan         TEXT    NOT NULL,
                Thumbnail        BLOB    NULL,
                CONSTRAINT PK_ListFile PRIMARY KEY (
                    MediaGalleryId,
                    Path
                )
            );
            """;

        using (var connection = this.GetConnection()) {
            var tables = connection.GetSchema();

            var cmd = connection.CreateCommand();
            cmd.CommandText = sqlStatement;
            cmd.ExecuteNonQuery();
        }
    }

    public void EnsureDeleted() {
        if (System.IO.File.Exists(this.DbPath)) {
            System.IO.File.Delete(this.DbPath);
        }
    }

    public string CreateSelectCommandText(
        string condition,
        int limit,
        bool includeThumbnail
        ) {
            var sb = new StringBuilder();
        if (includeThumbnail) {
            sb.AppendLine("SELECT rowid, MediaGalleryId, MediaType, Folder, Path, Name, LastWriteTimeUtc, Size, LastScan, Thumbnail");
        } else {
            sb.AppendLine("SELECT rowid, MediaGalleryId, MediaType, Folder, Path, Name, LastWriteTimeUtc, Size, LastScan");
        }
        sb.AppendLine("FROM ListFile");
        if (condition is {Length: > 0 }) {
            sb.Append("WHERE ").AppendLine(condition);
        }
        if (0<limit) {
            sb.Append("LIMIT ").AppendLine(limit.ToString());
        }
        return sb.ToString();
    }

    public void Upsert(DbFile dbFile, SqliteConnection connection) {
        // Read existing row
        string selectSql = this.CreateSelectCommandText(
           "MediaGalleryId = @MediaGalleryId AND Path = @Path",
           1,
           false);

        List<DbFile> existingRows;
        using (var selectCmd = connection.CreateCommand()) {
            selectCmd.CommandText = selectSql;
            selectCmd.Parameters.AddWithValue("@MediaGalleryId", dbFile.MediaGalleryId);
            selectCmd.Parameters.AddWithValue("@Path", dbFile.Path);
            existingRows = this.Query(selectCmd, false);
        }
        if (existingRows.Count > 1) {
            throw new InvalidOperationException("Multiple rows found");
        } else if (existingRows.Count == 1) {
            var existingRow = existingRows[0];
            bool needsUpdate
                = existingRow.MediaType != dbFile.MediaType
                || existingRow.Folder != dbFile.Folder
                || existingRow.Name != dbFile.Name
                || existingRow.LastWriteTimeUtc != dbFile.LastWriteTimeUtc
                || existingRow.Size != dbFile.Size
                || existingRow.LastScan != dbFile.LastScan;
            if (needsUpdate) {
                // Update existing row
                const string updateSql = """
                    UPDATE ListFile
                    SET MediaType = @MediaType,
                        Folder = @Folder,
                        Name = @Name,
                        LastWriteTimeUtc = @LastWriteTimeUtc,
                        Size = @Size,
                        LastScan = @LastScan,
                        Thumbnail = zeroblob($length)
                    WHERE MediaGalleryId = @MediaGalleryId AND Path = @Path
                    """;

                using (var updateCmd = connection.CreateCommand()) {
                    updateCmd.CommandText = updateSql;
                    updateCmd.Parameters.AddWithValue("@MediaGalleryId", dbFile.MediaGalleryId);
                    updateCmd.Parameters.AddWithValue("@Path", dbFile.Path);
                    updateCmd.Parameters.AddWithValue("@MediaType", dbFile.MediaType);
                    updateCmd.Parameters.AddWithValue("@Folder", dbFile.Folder);
                    updateCmd.Parameters.AddWithValue("@Name", dbFile.Name);
                    updateCmd.Parameters.AddWithValue("@LastWriteTimeUtc", dbFile.LastWriteTimeUtc.ToString("o"));
                    updateCmd.Parameters.AddWithValue("@Size", dbFile.Size);
                    updateCmd.Parameters.AddWithValue("@LastScan", dbFile.LastScan.ToString("o"));
                    updateCmd.Parameters.AddWithValue("$length", dbFile.Thumbnail?.Length ?? 0);

                    updateCmd.ExecuteNonQuery();
                    if (dbFile.Thumbnail is { } thumbnail) {
                        using (var writeStream = new SqliteBlob(connection, "ListFile", "Thumbnail", existingRow.RowId)) {
                            thumbnail.CopyTo(writeStream);
                        }
                    }
                }
            }
        } else {
            const string insertSql = """
                INSERT INTO ListFile (MediaGalleryId, Path, MediaType, Folder, Name, LastWriteTimeUtc, Size, LastScan, Thumbnail)
                VALUES (@MediaGalleryId, @Path, @MediaType, @Folder, @Name, @LastWriteTimeUtc, @Size, @LastScan, zeroblob($length));
                SELECT last_insert_rowid();
                """;

            

            using (var insertCmd = connection.CreateCommand()) {
                insertCmd.CommandText = insertSql;
                insertCmd.Parameters.AddWithValue("@MediaGalleryId", dbFile.MediaGalleryId);
                insertCmd.Parameters.AddWithValue("@Path", dbFile.Path);
                insertCmd.Parameters.AddWithValue("@MediaType", dbFile.MediaType);
                insertCmd.Parameters.AddWithValue("@Folder", dbFile.Folder);
                insertCmd.Parameters.AddWithValue("@Name", dbFile.Name);
                insertCmd.Parameters.AddWithValue("@LastWriteTimeUtc", dbFile.LastWriteTimeUtc.ToString("o"));
                insertCmd.Parameters.AddWithValue("@Size", dbFile.Size);
                insertCmd.Parameters.AddWithValue("@LastScan", dbFile.LastScan.ToString("o"));
                insertCmd.Parameters.AddWithValue("$length", dbFile.Thumbnail?.Length ?? 0);
                

                if (insertCmd.ExecuteScalar() is long rowId) {
                    if (dbFile.Thumbnail is { } thumbnail) {
                        using (var writeStream = new SqliteBlob(connection, "ListFile", "Thumbnail", rowId)) {
                            thumbnail.CopyTo(writeStream);
                        }
                    }
                }
            }
        }
    }

    public void Delete(string mediaGalleryId, string path) {
        using (this.Lock.EnterScope()) {
            using (var connection = this.GetConnection()) {
                using (var deleteCommand = connection.CreateCommand()) {
                    deleteCommand.CommandText = @"""
                        DELETE FROM ListFile 
                        WHERE MediaGalleryId = @MediaGalleryId AND Path = @Path
                        LIMIT 1;
                        """;
                    deleteCommand.Parameters.AddWithValue("@MediaGalleryId", mediaGalleryId);
                    deleteCommand.Parameters.AddWithValue("@Path", path);
                    deleteCommand.ExecuteNonQuery();
                }
            }
        }
    }

    public void Cleanup(DateTime lastScan) {
        using (var connection = this.GetConnection()) {
            using (var deleteCommand = connection.CreateCommand()) {
                deleteCommand.CommandText = """
                    DELETE FROM ListFile WHERE LastScan < @LastScan;
                    """;
                deleteCommand.Parameters.AddWithValue("@LastScan", lastScan);
                deleteCommand.ExecuteNonQuery();
            }
        }
    }

    public List<DbFile> Query(
        SqliteCommand command,
        bool readThumbnail) {
        List<DbFile> result = new();
        using (var reader = command.ExecuteReader()) {
            while (reader.Read()) {
                DbFile item = new() {
                    RowId = reader.GetInt64(0),
                    MediaGalleryId = reader.GetString(1),
                    MediaType = reader.GetString(2),
                    Folder = reader.GetString(3),
                    Path = reader.GetString(4),
                    Name = reader.GetString(5),
                    LastWriteTimeUtc = reader.GetDateTime(6),
                    Size = reader.GetInt64(7),
                    LastScan = reader.GetDateTime(8),
                    Thumbnail = null,
                };
                if (readThumbnail) {
                    var streamThumbnail = RecyclableMemory.Instance.GetStream();
                    using (var streamDB = reader.GetStream(9)) {
                        streamDB.CopyTo(streamThumbnail);
                        streamThumbnail.Position = 0;
                    }
                }
                result.Add(item);
            }
        }
        return result;
    }

    public DateTime GetLastWriteTimeUtc(
        string mediaGalleryId,
        string path) {
        try {
            using (var connection = this.GetConnection()) {
                using (var selectCommand = connection.CreateCommand()) {
                    selectCommand.CommandText = """
                    SELECT LastWriteTimeUtc
                    FROM ListFile
                    WHERE MediaGalleryId = @MediaGalleryId AND Path = @Path
                    LIMIT 1;
                    """;
                    selectCommand.Parameters.AddWithValue("@MediaGalleryId", mediaGalleryId);
                    selectCommand.Parameters.AddWithValue("@Path", path);
                    if (selectCommand.ExecuteScalar() is DateTime lastWriteTimeUtc) {
                        return lastWriteTimeUtc;
                    } else {
                        return DateTime.UnixEpoch;
                    }
                }
            }
        } catch (Exception ex) {
            System.Console.Error.WriteLine(ex.ToString());
            throw;
        }
    }

    private static readonly char[] _ListSeparator = new[] { '\t', ' ' };

    public List<DbFile> Search(SAMediaSearchRequest searchRequest, Func<string, string, bool> checkIfExits) {
        using (this.Lock.EnterScope()) {
            using (var connection = this.GetConnection()) {
                using (var selectCommand = connection.CreateCommand()) {
                    var sbCondition = new StringBuilder();
                    if (searchRequest.MediaType != MediaType.Unkown) {
                        var mediaTypeString = FilePersistenceUtility.ConvertMediaTypeToString(searchRequest.MediaType);
                        selectCommand.Parameters.AddWithValue("@MediaType", mediaTypeString);
                        sbCondition.AppendLine($"(MediaType = @MediaType)");
                    }

                    // If search value is provided, filter by name
                    if (searchRequest.Value is { Length: > 0 }) {
                        var listSearchFor = searchRequest.Value.Replace('\\', '/')
                            .Split(_ListSeparator, StringSplitOptions.RemoveEmptyEntries);

                        if (sbCondition.Length > 0) {
                            sbCondition.AppendLine(" AND ");
                        }

                        sbCondition.AppendLine("(");
                        for (int indexParameter = 0; indexParameter < listSearchFor.Length; indexParameter++) {
                            if (indexParameter > 0) {
                                sbCondition.AppendLine("OR");
                            }
                            var paramName = $"@SearchTerm{indexParameter}";
                            selectCommand.Parameters.AddWithValue(paramName, listSearchFor[indexParameter]);
                            sbCondition.AppendLine($"(Name LIKE concat('%', {paramName}, '%'))");
                        }
                        sbCondition.AppendLine(")");
                    }


                    string selectSql = this.CreateSelectCommandText(
                        sbCondition.ToString(),
                        0,
                        false);
                    selectCommand.CommandText = selectSql;
                    return this.Query(selectCommand, false);
                }
            }
        }
    }

    public (bool found, RecyclableMemoryStream? thumbnail) GetThumbnail(string mediaGalleryId, string path) {
        using (this.Lock.EnterScope()) {
            using (var connection = this.GetConnection()) {
                using (var selectCommand = connection.CreateCommand()) {
                    selectCommand.CommandText = this.CreateSelectCommandText(
                        "(MediaGalleryId = @MediaGalleryId) AND (Path = @Path)",
                        1,
                        true);
                    selectCommand.Parameters.AddWithValue("@MediaGalleryId", mediaGalleryId);
                    selectCommand.Parameters.AddWithValue("@Path", path);
                    var listDB = this.Query(selectCommand, true);
                    if (listDB.Count == 1) {
                        return (found: true, thumbnail: listDB[0].Thumbnail);
                    } else {
                        return (found: false, thumbnail: null);
                    }
                }
            }
        }
    }
}

//public class DbFolder {
//    public required string MediaGalleryId { get; set; }
//    public required string Folder { get; set; }
//}

public class DbFile {
    public long RowId { get; set; }
    public required string MediaGalleryId { get; set; }
    public required string MediaType { get; set; }
    public required string Folder { get; set; }
    public required string Path { get; set; }
    public required string Name { get; set; }
    public required DateTime LastWriteTimeUtc { get; set; }
    public required long Size { get; set; }
    public required DateTime LastScan { get; set; }
    public RecyclableMemoryStream? Thumbnail { get; set; }
}