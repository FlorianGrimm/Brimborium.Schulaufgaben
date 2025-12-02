// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SAAudio : IEquatable<SAAudio> {
    /// <summary>
    /// Reference to <see cref="SAMedia"/> in <see cref="SADocument.ListMedia"/>.
    /// </summary>
    public Guid Media { get; set; }

    public bool Equals(SAAudio? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Media == other.Media;
    }

    public override bool Equals(object? obj) => this.Equals(obj as SAAudio);

    public override int GetHashCode() => this.Media.GetHashCode();
}
