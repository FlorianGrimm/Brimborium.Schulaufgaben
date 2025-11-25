// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SAText {
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
}
