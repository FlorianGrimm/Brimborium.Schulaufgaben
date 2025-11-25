// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SAImage {
    /// <summary>
    /// Reference to <see cref="SAMedia"/> in <see cref="SADocument.ListMedia"/>.
    /// </summary>
    public Guid Media { get; set; }
}
