// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SABorder {
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
}