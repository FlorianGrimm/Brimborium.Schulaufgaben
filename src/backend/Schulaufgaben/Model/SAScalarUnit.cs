// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SAScalarUnit {
    public required double Value { get; set; }
    public required SAUnit Unit { get; set; }

    /// <summary>
    /// In <see cref="SADocument"/>.DefinedHorizontal or .DefinedVertical the name is defined.
    /// If used in <see cref="SAContent"/> the value of the DefinedHorizontal or DefinedVertical is used.
    /// </summary>
    public string? Name { get; set; }
}
