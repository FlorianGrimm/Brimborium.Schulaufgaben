// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SAScalarUnit : IEquatable<SAScalarUnit> {
    public required double Value { get; set; }
    public required SAUnit Unit { get; set; }

    /// <summary>
    /// In <see cref="SADocument"/>.DefinedHorizontal or .DefinedVertical the name is defined.
    /// If used in <see cref="SAContent"/> the value of the DefinedHorizontal or DefinedVertical is used.
    /// </summary>
    public string? Name { get; set; }

    public bool Equals(SAScalarUnit? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Value == other.Value &&
               this.Unit == other.Unit &&
               this.Name == other.Name;
    }

    public override bool Equals(object? obj) => this.Equals(obj as SAScalarUnit);

    public override int GetHashCode() {
        return HashCode.Combine(this.Value, this.Unit, this.Name);
    }
}
