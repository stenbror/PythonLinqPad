
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
public sealed record ReturnNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Right) : StatementNode(StartPos, EndPos);
public sealed record RaiseNode(int StartPos, int EndPos, Symbol Symbol1) : StatementNode(StartPos, EndPos);
public sealed record RaiseElementNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Left) : StatementNode(StartPos, EndPos);
public sealed record RaiseFromNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Left, Symbol Symbol2, ExpressionNode Right) : StatementNode(StartPos, EndPos);
public sealed record YieldStmtNode(int StartPos, int EndPos, ExpressionNode Right) : StatementNode(StartPos, EndPos);
public sealed record AssertSingleNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Left) : StatementNode(StartPos, EndPos);
public sealed record AssertNode(int StartPos, int EndPos, Symbol Symbol1, ExpressionNode Left, Symbol Symbol2, ExpressionNode Right) : StatementNode(StartPos, EndPos);


public sealed record DottedNameNode(int StartPos, int EndPos, Symbol[] Elements, Symbol[] Separators) : StatementNode(StartPos, EndPos);
public sealed record DottedAsNameNode(int StartPos, int EndPos, StatementNode Left, Symbol As, Symbol Name) : StatementNode(StartPos, EndPos);
public sealed record DottedAsNamesNode(int StartPos, int EndPos, StatementNode[] Elements, Symbol[] Separators) : StatementNode(StartPos, EndPos);
public sealed record ImportFromNode(int StartPos, int EndPos, Symbol Name) : StatementNode(StartPos, EndPos);
public sealed record ImportFromAsNode(int StartPos, int EndPos, Symbol Left, Symbol As, Symbol Right) : StatementNode(StartPos, EndPos);
public sealed record ImportFromAsNamesNode(int StartPos, int EndPos, StatementNode[] Elements, Symbol[] Separators) : StatementNode(StartPos, EndPos);
public sealed record ImportNameNode(int StartPos, int EndPos, Symbol Symbol1, StatementNode Right) : StatementNode(StartPos, EndPos);
public sealed record ImportFromStmtNode(int StartPos, int EndPos, Symbol From, Symbol[] Dots, StatementNode? Left, Symbol Import, Symbol? Start, StatementNode? Right, Symbol? End) : StatementNode(StartPos, EndPos);
public sealed record DelStatementNode(int StartPos, int EndPos, Symbol Symbol1, StatementNode Right) : StatementNode(StartPos, EndPos);
public sealed record DelTargetsNode(int StartPos, int EndPos, StatementNode[] Elements, Symbol[] Separators) : StatementNode(StartPos, EndPos);



public sealed record BlockNode(int StartPos, int EndPos, Symbol Indent, StatementNode Right, Symbol Dedent) : StatementNode(StartPos, EndPos);
public sealed record IfStatementNode(int StartPos, int EndPos, Symbol If, ExpressionNode Left, Symbol Colon, StatementNode Right, StatementNode[] Elif, StatementNode? Else) : StatementNode(StartPos, EndPos);
public sealed record ElifStatementNode(int StartPos, int EndPos, Symbol Elif, ExpressionNode Left, Symbol Colon, StatementNode Right) : StatementNode(StartPos, EndPos);
public sealed record ElseStatementNode(int StartPos, int EndPos, Symbol Else, Symbol Colon, StatementNode Right) : StatementNode(StartPos, EndPos);
public sealed record WhileStatementNode(int StartPos, int EndPos, Symbol While, ExpressionNode Left, Symbol Colon, StatementNode Right, StatementNode? Else) : StatementNode(StartPos, EndPos);


public sealed record TryFinallyStatementBlockNode(int StartPos, int EndPos, Symbol Try, Symbol Colon, StatementNode Left, StatementNode Finally) : StatementNode(StartPos, EndPos);
public sealed record TryExceptFinallyStatementBlockNode(int StartPos, int EndPos, Symbol Try, Symbol Colon, StatementNode Left, StatementNode[] Excepts, StatementNode? Else, StatementNode? Finally) : StatementNode(StartPos, EndPos);
public sealed record DefaultExceptStatementNode(int StartPos, int EndPos, Symbol Except, Symbol Colon, StatementNode Right) : StatementNode(StartPos, EndPos);
public sealed record ExceptStatementNode(int StartPos, int EndPos, Symbol Except, ExpressionNode Left, Symbol? As, Symbol? Name, Symbol Colon, StatementNode Right) : StatementNode(StartPos, EndPos);
public sealed record StarExceptStatementNode(int StartPos, int EndPos, Symbol Except, Symbol Star, ExpressionNode Left, Symbol? As, Symbol? Name, Symbol Colon, StatementNode Right) : StatementNode(StartPos, EndPos);
public sealed record FinallyStatementNode(int StartPos, int EndPos, Symbol Finally, Symbol Colon, StatementNode Right) : StatementNode(StartPos, EndPos);

public sealed record MatchStatementNode(int StartPos, int EndPos, Symbol Match, ExpressionNode Left, Symbol Colon, Symbol Newline, Symbol Indent, StatementNode[] Elements, Symbol Dedents) : StatementNode(StartPos, EndPos);
public sealed record GuardStatementNode(int StartPos, int EndPos, Symbol If, ExpressionNode Right) : StatementNode(StartPos, EndPos);
public sealed record MatchCaseStatementNode(int StartPos, int EndPos, Symbol Case, StatementNode Pattern, StatementNode? Guard, Symbol Colon, StatementNode Right) : StatementNode(StartPos, EndPos);

public sealed record MatchAsPatternNode(int StartPos, int EndPos, StatementNode Left, Symbol As, Symbol Name) : StatementNode(StartPos, EndPos);
public sealed record MatchOrPatternsNode(int StartPos, int EndPos, StatementNode[] Elements, Symbol[] Separators) : StatementNode(StartPos, EndPos);



public sealed record MatchDefaultCasePatternNode(int StartPos, int EndPos, Symbol Default) : StatementNode(StartPos, EndPos);
public sealed record MatchStringCasePatternNode(int StartPos, int EndPos, Symbol String) : StatementNode(StartPos, EndPos);
public sealed record MatchNoneCasePatternNode(int StartPos, int EndPos, Symbol None) : StatementNode(StartPos, EndPos);
public sealed record MatchTrueCasePatternNode(int StartPos, int EndPos, Symbol True) : StatementNode(StartPos, EndPos);
public sealed record MatchFalseCasePatternNode(int StartPos, int EndPos, Symbol False) : StatementNode(StartPos, EndPos);
public sealed record MatchNumberCasePatternNode(int StartPos, int EndPos, Symbol Number) : StatementNode(StartPos, EndPos);
public sealed record MatchSignedNumberCasePatternNode(int StartPos, int EndPos, Symbol Signed, Symbol Number) : StatementNode(StartPos, EndPos);
public sealed record MatchImaginaryNumberCasePatternNode(int StartPos, int EndPos, Symbol Real, Symbol Signed, Symbol Imiginary) : StatementNode(StartPos, EndPos);
public sealed record MatchCaptureTargetCasePatternNode(int StartPos, int EndPos, Symbol Nane) : StatementNode(StartPos, EndPos);
public sealed record MatchDottedNameCasePatternNode(int StartPos, int EndPos, Symbol Name, Symbol[] Names, Symbol[] Separators) : StatementNode(StartPos, EndPos);
public sealed record MatchMappingCasePatternNode(int StartPos, int EndPos, Symbol Start, StatementNode[] Elements, Symbol[] Separators, Symbol End) : StatementNode(StartPos, EndPos);
public sealed record DoubleStarPatternNode(int StartPos, int EndPos, Symbol DoubleStar, Symbol Name) : StatementNode(StartPos, EndPos);
public sealed record KeyValuePatternNode(int StartPos, int EndPos, StatementNode Left, Symbol Colon, StatementNode Right) : StatementNode(StartPos, EndPos);


