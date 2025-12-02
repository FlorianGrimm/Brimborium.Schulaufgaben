// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SAVideo : IEquatable<SAVideo> {
    /// <summary>
    /// Reference to <see cref="SAMedia"/> in <see cref="SADocument.ListMedia"/>.
    /// </summary>
    public Guid Media { get; set; }

    public bool Equals(SAVideo? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Media == other.Media;
    }

    public override bool Equals(object? obj) => this.Equals(obj as SAVideo);

    public override int GetHashCode() => this.Media.GetHashCode();
}
