
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
public sealed record DictionaryLiteralNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode[] Elements, Symbol[] Separators, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record PrimaryExpressionNode(int StartPos, int EndPos, ExpressionNode Left, ExpressionNode[] Nodes) : ExpressionNode(StartPos, EndPos);
public sealed record DotNameNode(int StartPos, int EndPos, Symbol Dot, Symbol Name) : ExpressionNode(StartPos, EndPos);
public sealed record CallNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode? Right, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record IndexNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode? Right, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);
public sealed record AwaitExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode? Right) : ExpressionNode(StartPos, EndPos);
public sealed record SlicesNode(int StartPos, int EndPos, ExpressionNode[] Elements, Symbol[] Separators) : ExpressionNode(StartPos, EndPos);
public sealed record SliceNode(int StartPos, int EndPos, ExpressionNode? Left, Symbol? Symbol1, ExpressionNode? Right, Symbol? Symbol2, ExpressionNode? Next) : ExpressionNode(StartPos, EndPos);
public sealed record PowerExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record UnaryPlusExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record UnaryMinusExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record UnaryBitInvertExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record MulExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record DivExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record FloorDivExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record ModuloExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record MatriceExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record PlusExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record MinusExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record ShiftLeftExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record ShiftRightExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record BitwiseAndExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record BitwiseXorExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record BitwiseOrExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record NotInExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, Symbol Symbol2, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record IsNotExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, Symbol Symbol2, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record IsExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record LessExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record LessEqualExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record EqualExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record GreaterEqualExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record GreaterExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record NotEqualExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record NotExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record AndExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record OrExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record YieldExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record YieldFromExpressionNode(int StartPos, int EndPos, Symbol Symbol1, Symbol Symbol2, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record StarExpressionsNode(int StartPos, int EndPos, ExpressionNode[] Elements, Symbol[] Separators) : ExpressionNode(StartPos, EndPos);
public sealed record StarExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record NamedExpressionNode(int StartPos, int EndPos, Symbol Name, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record StarNamedExpressionsNode(int StartPos, int EndPos, ExpressionNode[] Elements, Symbol[] Separators) : ExpressionNode(StartPos, EndPos);
public sealed record TestExpressionNode(int StartPos, int EndPos, ExpressionNode Left, Symbol Symbol1, ExpressionNode Right, Symbol Symbol2, ExpressionNode Next) : ExpressionNode(StartPos, EndPos);
public sealed record ExpressionsNode(int StartPos, int EndPos, ExpressionNode[] Elements, Symbol[] Separators) : ExpressionNode(StartPos, EndPos);
public sealed record NamedExpression(int StartPos, int EndPos, NameLiteralNode Left, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record LambdaExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode? Args, Symbol Symbol2, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record GeneratorGroupExpressionNode(int StartPos, int EndPos, ExpressionNode[] Elements) : ExpressionNode(StartPos, EndPos);
public sealed record GeneratorForExpressionNode(int StartPos, int EndPos, Symbol For, ExpressionNode Left, Symbol In, ExpressionNode Right, ExpressionNode[] Elements) : ExpressionNode(StartPos, EndPos);
public sealed record GeneratorAsyncForExpressionNode(int StartPos, int EndPos, Symbol Async, Symbol For, ExpressionNode Left, Symbol In, ExpressionNode Right, ExpressionNode[] Elements) : ExpressionNode(StartPos, EndPos);
public sealed record GeneratorIfExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record KeyValueElementExpressionNode(int StartPos, int EndPos, ExpressionNode Key, Symbol Colon, ExpressionNode Value) : ExpressionNode(StartPos, EndPos);
public sealed record DoubleStarDictionaryExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Right) : ExpressionNode(StartPos, EndPos);
public sealed record SetLiteralExpressionNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode[] Elements, Symbol[] Separators, Symbol Symbol2) : ExpressionNode(StartPos, EndPos);





public sealed record StmtsNode(int StartPos, int EndPos, StatementNode[] Elements) : StatementNode(StartPos, EndPos);
public sealed record SimpleStmtsNode(int StartPos, int EndPos, StatementNode[] Elements, Symbol[] Separators, Symbol Newline) : StatementNode(StartPos, EndPos);
public sealed record BreakStmtNode(int StartPos, int EndPos, Symbol Symbol1) : StatementNode(StartPos, EndPos);
public sealed record ContinueStmtNode(int StartPos, int EndPos, Symbol Symbol1) : StatementNode(StartPos, EndPos);
public sealed record PassStmtNode(int StartPos, int EndPos, Symbol Symbol1) : StatementNode(StartPos, EndPos);


public sealed record TypeParameterTypedNode(int StartPos, int EndPos, Symbol Name, Symbol Colon, ExpressionNode Right) : StatementNode(StartPos, EndPos);
public sealed record TypeParameterNode(int StartPos, int EndPos, Symbol Name) : StatementNode(StartPos, EndPos);
public sealed record TypeStarParameterNode(int StartPos, int EndPos, Symbol Mul, Symbol Name) : StatementNode(StartPos, EndPos);
public sealed record TypePowerParameterNode(int StartPos, int EndPos, Symbol Power, Symbol Name) : StatementNode(StartPos, EndPos);
public sealed record TypeParamSequenceNode(int StartPos, int EndPos, StatementNode[] Elements, Symbol[] Separators) : StatementNode(StartPos, EndPos);
public sealed record TypeParamsNode(int StartPos, int EndPos, Symbol Symbol1, StatementNode Right, Symbol Symbol2) : StatementNode(StartPos, EndPos);
public sealed record TypeAliasNode(int StartPos, int EndPos, Symbol Symbol1, Symbol Name, StatementNode? Parameters, Symbol Symbol2, ExpressionNode Right) : StatementNode(StartPos, EndPos);
public sealed record GlobalNode(int StartPos, int EndPos, Symbol Symbol1, Symbol[] Elements, Symbol[] Separators) : StatementNode(StartPos, EndPos);
public sealed record NonlocalNode(int StartPos, int EndPos, Symbol Symbol1, Symbol[] Elements, Symbol[] Separators) : StatementNode(StartPos, EndPos);

