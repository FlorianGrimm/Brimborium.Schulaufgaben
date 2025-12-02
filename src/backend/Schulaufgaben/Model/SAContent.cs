// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public class SAContent : IEquatable<SAContent> {
    public string? MatchingValue { get; set; }
    public SAExpression? MatchingExpression { get; set; }
    public SAColor? Background { get; set; }
    public SABorder? Border { get; set; }
    public List<SATransform>? ListTransform { get; set; }
    public SAText? Text { get; set; }
    public SAImage? Image { get; set; }
    public SAAudio? Audio { get; set; }
    public SAVideo? Video { get; set; }

    public bool Equals(SAContent? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.MatchingValue == other.MatchingValue &&
               Equals(this.MatchingExpression, other.MatchingExpression) &&
               Equals(this.Background, other.Background) &&
               Equals(this.Border, other.Border) &&
               SequenceEqualNullable(this.ListTransform, other.ListTransform) &&
               Equals(this.Text, other.Text) &&
               Equals(this.Image, other.Image) &&
               Equals(this.Audio, other.Audio) &&
               Equals(this.Video, other.Video);
    }

    private static bool SequenceEqualNullable<T>(List<T>? list1, List<T>? list2) {
        if (ReferenceEquals(list1, list2)) return true;
        if (list1 is null || list2 is null) return false;
        return list1.SequenceEqual(list2);
    }

    public override bool Equals(object? obj) => this.Equals(obj as SAContent);

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(this.MatchingValue);
        hash.Add(this.MatchingExpression);
        hash.Add(this.Background);
        hash.Add(this.Border);
        if (this.ListTransform is not null) {
            foreach (var item in this.ListTransform) {
                hash.Add(item);
            }
        }
        hash.Add(this.Text);
        hash.Add(this.Image);
        hash.Add(this.Audio);
        hash.Add(this.Video);
        return hash.ToHashCode();
    }
}
