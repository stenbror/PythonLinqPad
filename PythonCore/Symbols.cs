namespace PythonCore;

public record Symbol(uint StartPos, uint EndPos);

public sealed record PyFalse(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyNone(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyTrue(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyAnd(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyAs(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyAssert(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyAsync(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyAwait(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);