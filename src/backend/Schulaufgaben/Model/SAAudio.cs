// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

[JsonDerivedType(typeof(SAAudio), typeDiscriminator: "SAAudio")]
public class SAAudio {
    /// <summary>
    /// Reference to <see cref="SAMedia"/> in <see cref="SADocument.ListMedia"/>.
    /// </summary>
    public Guid Media { get; set; }
}
