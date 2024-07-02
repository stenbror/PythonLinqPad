
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
public sealed record TupleLiteralNode(uint StartPos, uint EndPos, Symbol Symbol1, ExpressionNode[] Elements, Symbol[] Separators, Node? Generator, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record ListLiteralNode(uint StartPos, uint EndPos, Symbol Symbol1, ExpressionNode[] Elements, Symbol[] Separators, Node? Generator, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record SetLiteralNode(uint StartPos, uint EndPos, Symbol Symbol1, ExpressionNode[] Elements, Symbol[] Separators, Node? Generator, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record DictionaryLiteralNode(uint StartPos, uint EndPos, Symbol Symbol1, ExpressionNode[] Elements, Symbol[] Separators, Node? Generator, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record DictionaryElementNode(uint StartPos, uint EndPos, ExpressionNode Key, Symbol Separator, ExpressionNode Value) : ExpressionNode(StartPos, EndPos);