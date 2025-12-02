// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;
public class SAColor : IEquatable<SAColor> {
    /// <summary>
    /// An RGBA color value.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// In <see cref="SADocument"/>.DefinedColor the name is defined.
    /// If used in <see cref="SAContent"/> the value of the DefinedColor is used.
    /// </summary>
    public string? Name { get; set; }

    public bool Equals(SAColor? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Value == other.Value &&
               this.Name == other.Name;
    }

    public override bool Equals(object? obj) => this.Equals(obj as SAColor);

    public override int GetHashCode() {
        return HashCode.Combine(this.Value, this.Name);
    }
}