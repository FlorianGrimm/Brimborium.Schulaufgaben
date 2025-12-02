// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SAText : IEquatable<SAText> {
    public string Value { get; set; } = string.Empty;
    //public string Style { get; set; } = string.Empty;

    // Core typography
    public string? FontFamily { get; set; }
    public SAScalarUnit? FontSize { get; set; }
    public string? FontWeight { get; set; } // "normal", "bold", "100"-"900"
    public string? FontStyle { get; set; } // "normal", "italic"

    // Text appearance
    public SAColor? Color { get; set; }
    public string? TextAlign { get; set; } // "left", "center", "right", "justify"
    public string? TextDecoration { get; set; } // "none", "underline", "line-through"

    public bool Equals(SAText? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Value == other.Value &&
               this.FontFamily == other.FontFamily &&
               Equals(this.FontSize, other.FontSize) &&
               this.FontWeight == other.FontWeight &&
               this.FontStyle == other.FontStyle &&
               Equals(this.Color, other.Color) &&
               this.TextAlign == other.TextAlign &&
               this.TextDecoration == other.TextDecoration;
    }

    public override bool Equals(object? obj) => this.Equals(obj as SAText);

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(this.Value);
        hash.Add(this.FontFamily);
        hash.Add(this.FontSize);
        hash.Add(this.FontWeight);
        hash.Add(this.FontStyle);
        hash.Add(this.Color);
        hash.Add(this.TextAlign);
        hash.Add(this.TextDecoration);
        return hash.ToHashCode();
    }
}
