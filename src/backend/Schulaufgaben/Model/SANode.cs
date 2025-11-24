// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

[JsonDerivedType(typeof(SANode), typeDiscriminator: "SANode")]
public class SANode {
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Kind { get; set; }
    public List<SANode> ListItem { get; set; } = [];
    public SABox? Position { get; set; }
    public SAContent? Normal { get; set; }
    public SAContent? Flipped { get; set; }
    public SAContent? Selected { get; set; }
}

[JsonDerivedType(typeof(SAContent), typeDiscriminator: "SAContent")]
public class SAContent {
    public string? MatchingValue { get; set; }
    public SAColor? Background { get; set; }
    public SABorder? Border { get; set; }
    public SAText? Text { get; set; }
    public SAImage? Image { get; set; }
    public SAAudio? Audio { get; set; }
    public SAVideo? Video { get; set; }
}

public enum SAUnit {
    [JsonStringEnumMemberName("Percent")]
    Percent,
    [JsonStringEnumMemberName("Pixel")]
    Pixel 
}

[JsonDerivedType(typeof(SAScalarUnit), typeDiscriminator: "SAScalarUnit")]
public class SAScalarUnit {
    public required float Value { get; set; }
    public required SAUnit Unit { get; set; }
    public string? Name { get; set; }
}

public class SABox {
    public SAScalarUnit? Left { get; set; }
    public SAScalarUnit? Top { get; set; }
    public SAScalarUnit? Width { get; set; }
    public SAScalarUnit? Height { get; set; }
    public SAScalarUnit? Right { get; set; }
    public SAScalarUnit? Bottom { get; set; }
}


[JsonDerivedType(typeof(SAImage), typeDiscriminator: "SAImage")]
public class SAImage {
    public Guid Media { get; set; }
}

[JsonDerivedType(typeof(SAVideo), typeDiscriminator: "SAVideo")]
public class SAVideo {
    public Guid Media { get; set; }
}

[JsonDerivedType(typeof(SAAudio), typeDiscriminator: "SAAudio")]
public class SAAudio {
    public Guid Media { get; set; }
}

[JsonDerivedType(typeof(SAText), typeDiscriminator: "SAText")]
public class SAText {
    public string Value { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
}

#if false
[JsonDerivedType(typeof(SANode), typeDiscriminator: "SANode")]
[JsonDerivedType(typeof(SAGroup), typeDiscriminator: "SAGroup")]
[JsonDerivedType(typeof(SAList), typeDiscriminator: "SAList")]
[JsonDerivedType(typeof(SAImage), typeDiscriminator: "SAImage")]
[JsonDerivedType(typeof(SAAudio), typeDiscriminator: "SAAudio")]
[JsonDerivedType(typeof(SAVideo), typeDiscriminator: "SAVideo")]
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
public class SAVideo : SANode {
    public Guid Media { get; set; }
}

public class SAAudio : SANode {
    public Guid Media { get; set; }
}

public class SAText : SANode {
    public string Value { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
}

#endif