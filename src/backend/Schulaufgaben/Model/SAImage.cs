// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

[JsonDerivedType(typeof(SAImage), typeDiscriminator: "SAImage")]
public class SAImage {
    /// <summary>
    /// Reference to <see cref="SAMedia"/> in <see cref="SADocument.ListMedia"/>.
    /// </summary>
    public Guid Media { get; set; }
}
