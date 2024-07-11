
namespace PythonCore;

public record Trivia(int StartPos, int EndPos);

public sealed record WhiteSpaceTrivia(int StartPos, int EndPos) : Trivia(StartPos, EndPos);
public sealed record TabulatorTrivia(int StartPos, int EndPos) : Trivia(StartPos, EndPos);
public sealed record NewlineTrivia(int StartPos, int EndPos, char Ch1, char Ch2) : Trivia(StartPos, EndPos);
public sealed record LineContinuationTrivia(int StartPos, int EndPos) : Trivia(StartPos, EndPos);
public sealed record CommentTrivia(int StartPos, int EndPos, string Comment) : Trivia(StartPos, EndPos);
