// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

/// <summary>
/// The media is part of the media gallery.
/// It is not a <see cref="SAMedia"/>, but it might be used to copy the media in the document..
/// </summary>
[JsonDerivedType(typeof(SAMediaInfo), typeDiscriminator: "MediaInfo")]
public class SAMediaInfo {
    /// <summary>
    /// The relative Path (using "/" insteas of "\")
    /// </summary>
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; } = 0;
}