// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

// https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Values/transform-function
public class SATransform : IEquatable<SATransform> {
    public Guid Id { get; set; }

    public string? TransformFunction { get; set; }

    public List<SAScalarUnit>? ListArgument { get; set; }

    public bool Equals(SATransform? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Id == other.Id &&
               this.TransformFunction == other.TransformFunction &&
               SequenceEqualNullable(this.ListArgument, other.ListArgument);
    }

    private static bool SequenceEqualNullable<T>(List<T>? list1, List<T>? list2) {
        if (ReferenceEquals(list1, list2)) return true;
        if (list1 is null || list2 is null) return false;
        return list1.SequenceEqual(list2);
    }

    public override bool Equals(object? obj) => this.Equals(obj as SATransform);

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(this.Id);
        hash.Add(this.TransformFunction);
        if (this.ListArgument is not null) {
            foreach (var item in this.ListArgument) {
                hash.Add(item);
            }
        }
        return hash.ToHashCode();
    }
}