// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public sealed class PascalCaseJsonNamingPolicy : JsonNamingPolicy {
    public override string ConvertName(string name) {
        if (string.IsNullOrEmpty(name)) {
            return name;
        }
        if (char.IsLower(name[0])) {
            Span<char> chars = name.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);
            return new string(chars);
        } else {
            return name;
        }
    }
}