// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model;

public sealed class SAExpression : IEquatable<SAExpression> {
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public SAExpressionNode? Root { get; set; }

    public bool Equals(SAExpression? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.Id == other.Id &&
               this.Name == other.Name &&
               Equals(this.Root, other.Root);
    }

    public override bool Equals(object? obj) => this.Equals(obj as SAExpression);

    public override int GetHashCode() {
        return HashCode.Combine(this.Id, this.Name, this.Root);
    }
}

public sealed class SAExpressionNode : IEquatable<SAExpressionNode> {
    public string? FunctionName { get; set; }

    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }

    public string? ValueText { get; set; }

    public List<SAExpressionNode>? ListArgument { get; set; }

    public bool Equals(SAExpressionNode? other) {
        if (ReferenceEquals(other, null)) { return false; }
        if (ReferenceEquals(other, this)) { return true; }
        return this.FunctionName == other.FunctionName &&
               this.ReferenceType == other.ReferenceType &&
               this.ReferenceId == other.ReferenceId &&
               this.ValueText == other.ValueText &&
               SequenceEqualNullable(this.ListArgument, other.ListArgument);
    }

    private static bool SequenceEqualNullable<T>(List<T>? list1, List<T>? list2) {
        if (ReferenceEquals(list1, list2)) return true;
        if (list1 is null || list2 is null) return false;
        return list1.SequenceEqual(list2);
    }

    public override bool Equals(object? obj) => this.Equals(obj as SAExpressionNode);

    public override int GetHashCode() {
        return HashCode.Combine(this.FunctionName, this.ReferenceType, this.ReferenceId, this.ValueText, this.ListArgument);
    }
}