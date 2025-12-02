// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SABox : IEquatable<SABox> {
    public SAScalarUnit? Left { get; set; }
    public SAScalarUnit? Top { get; set; }
    public SAScalarUnit? Width { get; set; }
    public SAScalarUnit? Height { get; set; }
    public SAScalarUnit? Right { get; set; }
    public SAScalarUnit? Bottom { get; set; }

    public bool Equals(SABox? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return Equals(this.Left, other.Left) &&
               Equals(this.Top, other.Top) &&
               Equals(this.Width, other.Width) &&
               Equals(this.Height, other.Height) &&
               Equals(this.Right, other.Right) &&
               Equals(this.Bottom, other.Bottom);
    }

    public override bool Equals(object? obj) => this.Equals(obj as SABox);

    public override int GetHashCode() {
        return HashCode.Combine(this.Left, this.Top, this.Width, this.Height, this.Right, this.Bottom);
    }
}
