// MIT - Florian Grimm

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneDirectory = Lucene.Net.Store.Directory;
using OpenMode = Lucene.Net.Index.OpenMode;
using Document = Lucene.Net.Documents.Document;
using System.Diagnostics;
using Lucene.Net.Queries;

namespace Brimborium.Schulaufgaben.Service;

public class EditingLucanetCache : IDisposable {
    private readonly System.Threading.Lock _Lock = new();
    private string _CacheFolder;
    private const LuceneVersion _LuceneVersion = LuceneVersion.LUCENE_48;
    private const string FieldName = "name";
    private const string FieldLastWriteTimeUtc = "lastWriteTimeUtc";
    private const string FieldSize = "size";
    private const string FieldMediaGalleryId = "mediaGalleryId";
    private const string FieldMediaType = "mediaType";
    private const string FieldRelativePath = "relativePath";
    private const string FieldLastScan = "lastScan";
    private const string FieldThumbnail = "thumbnail";
    private LuceneDirectory? _IndexDir;
    private Analyzer? _Analyzer;
    private IndexWriterConfig? _IndexConfig;
    private OpenMode _OpenMode = OpenMode.CREATE;
    private IndexWriter? _Writer;
    private int _Uncommited;


    public EditingLucanetCache(string cacheFolder) {
        this._CacheFolder = cacheFolder;
        System.GC.SuppressFinalize(this);
    }

    public LuceneDirectory GetLuceneDirectory() {
        {
            if (this._IndexDir is { } result) { return result; }
        }
        using (this._Lock.EnterScope()) {
            {
                if (this._IndexDir is { } result) { return result; }
            }

            {
                var indexDir = FSDirectory.Open(this._CacheFolder);
                System.GC.ReRegisterForFinalize(this);
                this._IndexDir = indexDir;
                return indexDir;
            }
        }
    }
    public IndexWriter GetIndexWriter() {
        {
            if (this._Writer is { } result) { return result; }
        }
        var indexDir = this.GetLuceneDirectory();
        using (this._Lock.EnterScope()) {
            {
                if (this._Writer is { } result) { return result; }
            }
            {
                //Create an index writer
                Analyzer analyzer = this._Analyzer ??= new StandardAnalyzer(_LuceneVersion);
                IndexWriterConfig indexConfig = new IndexWriterConfig(_LuceneVersion, analyzer);
                indexConfig.OpenMode = this._OpenMode;
                this._OpenMode = OpenMode.CREATE_OR_APPEND;

                this._IndexConfig = indexConfig;
                IndexWriter writer = new IndexWriter(_IndexDir, indexConfig);
                this._Writer = writer;
                return writer;
            }
        }
        //using DirectoryReader reader = writer.GetReader(applyAllDeletes: true);
    }

    public static readonly FieldType Int64StoredNotIndexed = new FieldType {
        IsIndexed = false,
        IsTokenized = false,
        OmitNorms = false,
        IndexOptions = IndexOptions.DOCS_ONLY,
        NumericType = Lucene.Net.Documents.NumericType.INT64,
        IsStored = true
    }.Freeze();
    public static readonly FieldType StringStoredNotIndexed = new FieldType {
        IsIndexed = false,
        OmitNorms = false,
        IndexOptions = IndexOptions.DOCS_ONLY,
        IsStored = true,
        IsTokenized = false
    }.Freeze();
    public static readonly FieldType StringNotStoredIndexed = new FieldType {
        IsIndexed = true,
        OmitNorms = true,
        IndexOptions = IndexOptions.DOCS_AND_FREQS_AND_POSITIONS,
        IsStored = false,
        IsTokenized = true
    }.Freeze();
    public static readonly FieldType StringStoredIndexed = new FieldType {
        IsIndexed = true,
        OmitNorms = false,
        IndexOptions = IndexOptions.DOCS_AND_FREQS_AND_POSITIONS,
        IsStored = true,
        IsTokenized = true
    }.Freeze();
    


    public void Write(
        string mediaGalleryId,
        string mediaType,
        string relativePath,
        DateTime lastWriteTimeUtc,
        long size,
        DateTime lastScan
        ) {
        //var pos = relativePath.LastIndexOf('/');
        //var name = (0 < pos) ? relativePath.Substring(pos) : relativePath;
        Document doc = new Document();
        doc.Add(new Field(FieldName, relativePath, StringNotStoredIndexed));
        doc.Add(new Field(FieldMediaGalleryId, mediaGalleryId, StringStoredNotIndexed));
        doc.Add(new Field(FieldMediaType, mediaType, StringStoredIndexed));
        doc.Add(new Field(FieldRelativePath, relativePath, StringStoredNotIndexed));
        doc.Add(new Int64Field(FieldSize, size, Int64StoredNotIndexed));
        doc.Add(new Int64Field(FieldLastWriteTimeUtc, lastWriteTimeUtc.Ticks, Int64StoredNotIndexed));
        doc.Add(new Int64Field(FieldLastScan, lastScan.Ticks, Field.Store.YES));
        
        var writer = this.GetIndexWriter();
        writer.AddDocument(doc);
        if (64 < ++this._Uncommited) {
            //Flush and commit the index data to the directory
            this.Commit();
        }
    }

    public List<SAMediaInfo> Search(
        SAMediaSearchRequest request
        ) {
        var writer = this.GetIndexWriter();
        if (0 < this._Uncommited) { this.Commit(); }
        using DirectoryReader reader = writer.GetReader(applyAllDeletes: true);
        //using DirectoryReader reader = DirectoryReader.Open(this.GetLuceneDirectory());
        IndexSearcher searcher = new IndexSearcher(reader);
        Analyzer analyzer = this._Analyzer ??= new StandardAnalyzer(_LuceneVersion);

        //var queryParser = new Lucene.Net.QueryParsers.Classic.QueryParser(_LuceneVersion, FieldName, analyzer);
        //Query queryTerm = queryParser.Parse(request.Value);
        //var queryParser = new Lucene.Net.QueryParsers.Simple.SimpleQueryParser(analyzer, FieldName);
        //Query queryTerm = queryParser.Parse(request.Value);
        var queryTerm = new PhraseQuery();
        queryTerm.Add(new Term(FieldName, request.Value));
        queryTerm.Slop = 5;

        //Filter filter = new TermFilter(new Term(FieldMediaType, "image"));
        TopDocs topDocs = searcher.Search(
            query: queryTerm,
            filter: null,
            n: 100);
        int countMatchingDocs = topDocs.TotalHits;
        List<SAMediaInfo> result = new(countMatchingDocs);
        foreach (var doc in topDocs.ScoreDocs) {
            Document resultDoc = searcher.Doc(doc.Doc);
            var mediaGalleryId = resultDoc.Get(FieldMediaGalleryId);
            if (mediaGalleryId is null) { continue; }
            var relativePath = resultDoc.Get(FieldRelativePath);
            if (relativePath is null) { continue; }
            var mediaType = resultDoc.Get(FieldMediaType);
            var lastWriteTimeUtcTicks = resultDoc.GetField(FieldLastWriteTimeUtc).GetInt64Value().GetValueOrDefault();
            var size = resultDoc.GetField(FieldSize).GetInt64Value().GetValueOrDefault();

            result.Add(
                new SAMediaInfo() {
                    Path = $"{mediaGalleryId}/{relativePath}",
                    MediaType = mediaType,
                    LastWriteTimeUtc = new DateTime(lastWriteTimeUtcTicks, DateTimeKind.Utc),
                    Size = size
                });
        }
        return result;
    }

    public byte[]? GetThumbnail(string mediaGalleryId, string relativeName) {
        using DirectoryReader reader = DirectoryReader.Open(this.GetLuceneDirectory());
        IndexSearcher searcher = new IndexSearcher(reader);
        var query = new BooleanQuery();
        query.Add(new TermQuery(new Term(FieldMediaGalleryId, mediaGalleryId)), Occur.MUST);
        query.Add(new TermQuery(new Term(FieldRelativePath, relativeName)), Occur.MUST);
        var topDocs = searcher.Search(query, 1);
        if (topDocs.ScoreDocs.Length < 1) { return null; }
        var document = searcher.Doc(topDocs.ScoreDocs[0].Doc);
        if (document is null) { return null; }
        var result = document.GetBinaryValue(FieldThumbnail).Bytes;
        return result;
    }

    public void Cleanup(DateTime lastScan) {
        var lastScanTicks = lastScan.Ticks;
        var writer = this.GetIndexWriter();
        IndexReader indexReader = writer.GetReader(true);
        using (this._Lock.EnterScope()) {
            for (int documentIndex = 0; documentIndex < indexReader.MaxDoc; documentIndex++) {
                Document? doc = indexReader.Document(documentIndex);
                if (doc is null) { continue; }
                var lastScanField = doc.GetField(FieldLastScan);
                var lastScanDocument = lastScanField.GetInt64Value().GetValueOrDefault();
                if (lastScanDocument < lastScanTicks) {
                    try {
                        var query = new TermQuery(new Term(FieldLastScan, lastScanDocument.ToString()));
                        writer.DeleteDocuments(query);
                        return;
                    } catch (Exception /*error*/) {
                        // todo handle error8
                        this._Writer = null;
                        writer.Dispose();
                    }
                }
            }
        }
    }
    public void Commit() {
        if (0 == this._Uncommited) { return; }
        if (this._Writer is null) { return; }

        using (this._Lock.EnterScope()) {
            if (this._Writer is { } writer) {
                this._Uncommited = 0;
                try {
                    writer.Commit();
                } catch (OutOfMemoryException) {
                    this._Writer = null;
                    writer.Dispose();
                }
            }
        }
    }

    protected virtual void Dispose(bool disposing) {
        using (var d = this._IndexDir) {
            using (var w = this._Writer) {
                if (disposing) {
                    this._IndexDir = null;
                    this._Writer = null;
                    System.GC.SuppressFinalize(this);
                }
            }
        }
    }

    ~EditingLucanetCache() {
        this.Dispose(disposing: false);
    }

    public void Dispose() {
        this.Dispose(disposing: true);
    }
}