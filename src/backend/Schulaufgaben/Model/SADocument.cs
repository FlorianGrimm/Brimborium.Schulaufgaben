// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

[JsonDerivedType(typeof(SADocument), typeDiscriminator: "Document")]
public class SADocument : IEquatable<SADocument> {
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string KindInteraction { get; set; } = string.Empty;

    public List<SAMedia> ListMedia { get; set; } = [];

    public SANode? Decoration { get; set; }

    public SANode? Interaction { get; set; }

    public bool Equals(SADocument? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        if (this.Id != other.Id) return false;
        if (this.Name != other.Name) return false;
        if (this.Description != other.Description) return false;
        if (this.KindInteraction != other.KindInteraction) return false;
        // TODO ListMedia, Decoration, Interaction
        return true;
    }
}
