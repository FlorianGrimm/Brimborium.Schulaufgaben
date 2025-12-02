// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SADocumentDescription : IEquatable<SADocumentDescription> {
    public Guid Id { get; set; }

    public string Folder { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ContentHash { get; set; } = string.Empty;

    public bool Equals(SADocumentDescription? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Id == other.Id &&
               this.Folder == other.Folder &&
               this.Name == other.Name &&
               this.Description == other.Description &&
               this.ContentHash == other.ContentHash;
    }

    public override bool Equals(object? obj) => this.Equals(obj as SADocumentDescription);

    public override int GetHashCode() {
        return HashCode.Combine(this.Id, this.Folder, this.Name, this.Description, this.ContentHash);
    }
}
