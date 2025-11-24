// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

[JsonDerivedType(typeof(SABox), typeDiscriminator: "SABox")]
public class SABox {
    public SAScalarUnit? Left { get; set; }
    public SAScalarUnit? Top { get; set; }
    public SAScalarUnit? Width { get; set; }
    public SAScalarUnit? Height { get; set; }
    public SAScalarUnit? Right { get; set; }
    public SAScalarUnit? Bottom { get; set; }
}
