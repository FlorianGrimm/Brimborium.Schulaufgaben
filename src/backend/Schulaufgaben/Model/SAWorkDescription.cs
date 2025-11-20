// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

[JsonDerivedType(typeof(SAWorkDescription), typeDiscriminator: "SAWorkDescription")]
public class SAWorkDescription {
    public Guid Id { get; set; }

    public string Folder { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ContentHash { get; set; } = string.Empty;
}
