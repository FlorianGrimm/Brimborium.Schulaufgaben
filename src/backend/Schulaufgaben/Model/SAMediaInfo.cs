// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

/// <summary>
/// The media is part of the media gallery.
/// It is not a <see cref="SAMedia"/>, but it might be used to copy the media in the document..
/// </summary>
public class SAMediaInfo : IEquatable<SAMediaInfo> {
    /// <summary>
    /// The relative Path (using "/" insteas of "\")
    /// </summary>
    public string Path { get; set; } = string.Empty;

    public string MediaType { get; set; } = string.Empty;

    public long Size { get; set; } = 0;

    public DateTime LastWriteTimeUtc { get; set; } = DateTime.UnixEpoch;

    public DateTime LastScan { get; set; } = DateTime.UnixEpoch;

    public bool Equals(SAMediaInfo? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Path == other.Path &&
               this.MediaType == other.MediaType &&
               this.Size == other.Size &&
               this.LastWriteTimeUtc == other.LastWriteTimeUtc &&
               this.LastScan == other.LastScan;
    }

    public override bool Equals(object? obj) => this.Equals(obj as SAMediaInfo);

    public override int GetHashCode() {
        return HashCode.Combine(this.Path, this.MediaType, this.Size, this.LastWriteTimeUtc, this.LastScan);
    }
}