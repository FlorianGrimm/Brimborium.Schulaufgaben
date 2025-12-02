// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SADocument : IEquatable<SADocument> {
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string KindInteraction { get; set; } = string.Empty;

    public List<SAMedia> ListMedia { get; set; } = [];

    public SANode? Decoration { get; set; }

    public SANode? Interaction { get; set; }

    /* 1200 x 800 as default. */
    public SAScalarUnit? Width { get; set; }

    public SAScalarUnit? Height { get; set; }

    public List<SAExpression>? ListExpression { get; set; }

    public List<SAScalarUnit>? DefinedHorizontal { get; set; }

    public List<SAScalarUnit>? DefinedVertical { get; set; }

    public List<SAColor>? DefinedColor { get; set; }

    public List<SAScalarUnit>? RulerHorizontal { get; set; }

    public List<SAScalarUnit>? RulerVertical { get; set; }

    public bool Equals(SADocument? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        if (this.Id != other.Id) return false;
        if (this.Name != other.Name) return false;
        if (this.Description != other.Description) return false;
        if (this.KindInteraction != other.KindInteraction) return false;
        if (!Equals(this.Width, other.Width)) return false;
        if (!Equals(this.Height, other.Height)) return false;
        if (!Equals(this.Decoration, other.Decoration)) return false;
        if (!Equals(this.Interaction, other.Interaction)) return false;
        if (!this.ListMedia.SequenceEqual(other.ListMedia)) return false;
        if (!SequenceEqualNullable(this.ListExpression, other.ListExpression)) return false;
        if (!SequenceEqualNullable(this.DefinedHorizontal, other.DefinedHorizontal)) return false;
        if (!SequenceEqualNullable(this.DefinedVertical, other.DefinedVertical)) return false;
        if (!SequenceEqualNullable(this.DefinedColor, other.DefinedColor)) return false;
        if (!SequenceEqualNullable(this.RulerHorizontal, other.RulerHorizontal)) return false;
        if (!SequenceEqualNullable(this.RulerVertical, other.RulerVertical)) return false;
        return true;
    }

    private static bool SequenceEqualNullable<T>(List<T>? list1, List<T>? list2) {
        if (ReferenceEquals(list1, list2)) return true;
        if (list1 is null || list2 is null) return false;
        return list1.SequenceEqual(list2);
    }

    public override bool Equals(object? obj) => this.Equals(obj as SADocument);

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(this.Id);
        hash.Add(this.Name);
        hash.Add(this.Description);
        hash.Add(this.KindInteraction);
        hash.Add(this.Width);
        hash.Add(this.Height);
        hash.Add(this.Decoration);
        hash.Add(this.Interaction);
        foreach (var item in this.ListMedia) {
            hash.Add(item);
        }
        return hash.ToHashCode();
    }
}
