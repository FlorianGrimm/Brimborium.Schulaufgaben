// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

/// <summary>
/// The media is used in the document.
/// The media is stored in the document folder.
/// </summary>
public class SAMedia {    
    public required Guid Id { get; set; }
    public required string Path { get; set; }
    public required string MediaType { get; set; }
    public required string ContentType { get; set; }
}
