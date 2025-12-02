// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SANode : IEquatable<SANode> {
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Kind { get; set; }
    public List<SANode> ListItem { get; set; } = [];
    public SABox? Position { get; set; }
    public SAContent? Normal { get; set; }
    public SAContent? Flipped { get; set; }
    public SAContent? Selected { get; set; }

    public bool Equals(SANode? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Id == other.Id &&
               this.Name == other.Name &&
               this.Kind == other.Kind &&
               this.ListItem.SequenceEqual(other.ListItem) &&
               Equals(this.Position, other.Position) &&
               Equals(this.Normal, other.Normal) &&
               Equals(this.Flipped, other.Flipped) &&
               Equals(this.Selected, other.Selected);
    }

    public override bool Equals(object? obj) => this.Equals(obj as SANode);

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(this.Id);
        hash.Add(this.Name);
        hash.Add(this.Kind);
        foreach (var item in this.ListItem) {
            hash.Add(item);
        }
        hash.Add(this.Position);
        hash.Add(this.Normal);
        hash.Add(this.Flipped);
        hash.Add(this.Selected);
        return hash.ToHashCode();
    }
}
