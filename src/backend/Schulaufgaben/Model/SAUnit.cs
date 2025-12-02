// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public enum SAUnit {
    [JsonStringEnumMemberName("Percent")]
    Percent,
    [JsonStringEnumMemberName("Pixel")]
    Pixel,
    [JsonStringEnumMemberName("Degree")]
    Degree,
    [JsonStringEnumMemberName("Scalar")]
    Scalar
}
