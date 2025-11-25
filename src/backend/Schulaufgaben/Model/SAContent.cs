// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SAContent {
    public string? MatchingValue { get; set; }
    public SAColor? Background { get; set; }
    public SABorder? Border { get; set; }
    public SAText? Text { get; set; }
    public SAImage? Image { get; set; }
    public SAAudio? Audio { get; set; }
    public SAVideo? Video { get; set; }
}
