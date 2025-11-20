// MIT - Florian Grimm

namespace Briumborium.Schulaufgaben.Model;

[JsonDerivedType(typeof(SAElement), typeDiscriminator: "SAWork")]
public class SAWork {
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string WorkMode { get; set; } = string.Empty;

    public SANode? Decoration { get; set; }

    public SANode? Data { get; set; }
}

[JsonDerivedType(typeof(SAElement), typeDiscriminator: "SANode")]
[JsonDerivedType(typeof(SANodeElement), typeDiscriminator: "SANodeElement")]
[JsonDerivedType(typeof(SAListNode), typeDiscriminator: "SAListNode")]
[JsonDerivedType(typeof(SAListElement), typeDiscriminator: "SAListElement")]
public class SANode {
    public string? NodeMode { get; set; }
    //public SAElement? Element { get; set; }
    //public List<SANode>? ListNode { get; set; }
    //public List<SAElement>? ListElement { get; set; }
}

public class SANodeElement {
    public SAElement? Element { get; set; }
}
public class SAListNode {
    public List<SANode>? ListNode { get; set; }
}

public class SAListElement {
    public List<SAElement>? ListElement { get; set; }
}

[JsonDerivedType(typeof(SAElement), typeDiscriminator: "SAElement")]
[JsonDerivedType(typeof(SAImageElement), typeDiscriminator: "SAImageElement")]
[JsonDerivedType(typeof(SATextElement), typeDiscriminator: "SATextElement")]
public class SAElement {
    public string? Ìd { get; set; }
}

public class SAImageElement {
    public string ImageUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public class SATextElement {
    public string Text { get; set; } = string.Empty;
    public string HtmlClass { get; set; } = string.Empty;
}
