// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

/// <summary>
/// The media is used in the document.
/// The media is stored in the document folder.
/// </summary>
public class SAMedia : IEquatable<SAMedia> {
    public required Guid Id { get; set; }
    public required string Path { get; set; }
    public required string MediaType { get; set; }
    public required string ContentType { get; set; }

    public bool Equals(SAMedia? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Id == other.Id &&
               this.Path == other.Path &&
               this.MediaType == other.MediaType &&
               this.ContentType == other.ContentType;
    }

    public override bool Equals(object? obj) => this.Equals(obj as SAMedia);

    public override int GetHashCode() {
        return HashCode.Combine(this.Id, this.Path, this.MediaType, this.ContentType);
    }
}
