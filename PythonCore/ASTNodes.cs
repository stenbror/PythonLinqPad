namespace PythonCore;

public record Node(uint StartPos, uint EndPos);
public record ExpressionNode(uint StartPos, uint EndPos) : Node(StartPos, EndPos);
public record StatementNode(uint StartPos, uint EndPos) : Node(StartPos, EndPos);

/* Expression nodes */
public sealed record NameLiteralNode(uint StartPos, uint EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record NumberLiteralNode(uint StartPos, uint EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record StringLiteralNode(uint StartPos, uint EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record NoneLiteralNode(uint StartPos, uint EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record FalseLiteralNode(uint StartPos, uint EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record TrueLiteralNode(uint StartPos, uint EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record ElipsisLiteralNode(uint StartPos, uint EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);