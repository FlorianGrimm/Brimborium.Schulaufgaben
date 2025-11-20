// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

[JsonDerivedType(typeof(SANode), typeDiscriminator: "SANode")]
[JsonDerivedType(typeof(SAGroup), typeDiscriminator: "SAGroup")]
[JsonDerivedType(typeof(SAList), typeDiscriminator: "SAList")]
[JsonDerivedType(typeof(SAImage), typeDiscriminator: "SAImage")]
[JsonDerivedType(typeof(SAText), typeDiscriminator: "SAText")]
public class SANode {
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Kind { get; set; }
}


public class SAGroup : SANode {
    public List<SANode> ListItem { get; set; } = [];
}

public class SAList : SANode {
    public List<SANode> ListItem { get; set; } = [];
}

public class SAImage : SANode {
    public Guid Media { get; set; }
}
public class SAAudio : SANode {
    public Guid Media { get; set; }
}

public class SAText : SANode {
    public string Value { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
}

