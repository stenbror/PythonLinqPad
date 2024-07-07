
namespace PythonCore;

public record Node(int StartPos, int EndPos);
public record ExpressionNode(int StartPos, int EndPos) : Node(StartPos, EndPos);
public record StatementNode(int StartPos, int EndPos) : Node(StartPos, EndPos);

/* Expression nodes */
public sealed record NameLiteralNode(int StartPos, int EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record NumberLiteralNode(int StartPos, int EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record StringLiteralNode(int StartPos, int EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record NoneLiteralNode(int StartPos, int EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record FalseLiteralNode(int StartPos, int EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record TrueLiteralNode(int StartPos, int EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record ElipsisLiteralNode(int StartPos, int EndPos, Symbol Element) : ExpressionNode(StartPos, EndPos);
public sealed record TupleLiteralNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode[] Elements, Symbol[] Separators, Node? Generator, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record ListLiteralNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode[] Elements, Symbol[] Separators, Node? Generator, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record SetLiteralNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode[] Elements, Symbol[] Separators, Node? Generator, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record DictionaryLiteralNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode[] Elements, Symbol[] Separators, Node? Generator, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record DictionaryElementNode(int StartPos, int EndPos, ExpressionNode Key, Symbol Separator, ExpressionNode Value) : ExpressionNode(StartPos, EndPos);