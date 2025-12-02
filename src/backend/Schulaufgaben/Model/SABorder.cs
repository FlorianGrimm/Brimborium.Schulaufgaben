// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SABorder : IEquatable<SABorder> {
    // Border width (can be set individually or all at once)
    public SAScalarUnit? BorderWidth { get; set; }
    public SAScalarUnit? BorderTopWidth { get; set; }
    public SAScalarUnit? BorderRightWidth { get; set; }
    public SAScalarUnit? BorderBottomWidth { get; set; }
    public SAScalarUnit? BorderLeftWidth { get; set; }

    // Border style
    public string? BorderStyle { get; set; }        // "none", "solid", "dashed", "dotted", "double"
    public string? BorderTopStyle { get; set; }
    public string? BorderRightStyle { get; set; }
    public string? BorderBottomStyle { get; set; }
    public string? BorderLeftStyle { get; set; }

    // Border color
    public SAColor? BorderColor { get; set; }
    public SAColor? BorderTopColor { get; set; }
    public SAColor? BorderRightColor { get; set; }
    public SAColor? BorderBottomColor { get; set; }
    public SAColor? BorderLeftColor { get; set; }

    // Border radius (for rounded corners)
    public SAScalarUnit? BorderRadius { get; set; }
    public SAScalarUnit? BorderTopLeftRadius { get; set; }
    public SAScalarUnit? BorderTopRightRadius { get; set; }
    public SAScalarUnit? BorderBottomRightRadius { get; set; }
    public SAScalarUnit? BorderBottomLeftRadius { get; set; }

    public bool Equals(SABorder? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return Equals(this.BorderWidth, other.BorderWidth) &&
               Equals(this.BorderTopWidth, other.BorderTopWidth) &&
               Equals(this.BorderRightWidth, other.BorderRightWidth) &&
               Equals(this.BorderBottomWidth, other.BorderBottomWidth) &&
               Equals(this.BorderLeftWidth, other.BorderLeftWidth) &&
               this.BorderStyle == other.BorderStyle &&
               this.BorderTopStyle == other.BorderTopStyle &&
               this.BorderRightStyle == other.BorderRightStyle &&
               this.BorderBottomStyle == other.BorderBottomStyle &&
               this.BorderLeftStyle == other.BorderLeftStyle &&
               Equals(this.BorderColor, other.BorderColor) &&
               Equals(this.BorderTopColor, other.BorderTopColor) &&
               Equals(this.BorderRightColor, other.BorderRightColor) &&
               Equals(this.BorderBottomColor, other.BorderBottomColor) &&
               Equals(this.BorderLeftColor, other.BorderLeftColor) &&
               Equals(this.BorderRadius, other.BorderRadius) &&
               Equals(this.BorderTopLeftRadius, other.BorderTopLeftRadius) &&
               Equals(this.BorderTopRightRadius, other.BorderTopRightRadius) &&
               Equals(this.BorderBottomRightRadius, other.BorderBottomRightRadius) &&
               Equals(this.BorderBottomLeftRadius, other.BorderBottomLeftRadius);
    }

    public override bool Equals(object? obj) => this.Equals(obj as SABorder);

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(this.BorderWidth);
        hash.Add(this.BorderTopWidth);
        hash.Add(this.BorderRightWidth);
        hash.Add(this.BorderBottomWidth);
        hash.Add(this.BorderLeftWidth);
        hash.Add(this.BorderStyle);
        hash.Add(this.BorderTopStyle);
        hash.Add(this.BorderRightStyle);
        hash.Add(this.BorderBottomStyle);
        hash.Add(this.BorderLeftStyle);
        hash.Add(this.BorderColor);
        hash.Add(this.BorderTopColor);
        hash.Add(this.BorderRightColor);
        hash.Add(this.BorderBottomColor);
        hash.Add(this.BorderLeftColor);
        hash.Add(this.BorderRadius);
        hash.Add(this.BorderTopLeftRadius);
        hash.Add(this.BorderTopRightRadius);
        hash.Add(this.BorderBottomRightRadius);
        hash.Add(this.BorderBottomLeftRadius);
        return hash.ToHashCode();
    }
}