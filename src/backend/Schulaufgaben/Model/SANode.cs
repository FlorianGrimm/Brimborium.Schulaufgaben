// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

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
