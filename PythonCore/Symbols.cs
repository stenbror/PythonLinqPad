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
public sealed record PyBreak(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyClass(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyContinue(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyDef(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyDel(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyElif(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyElse(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyExcept(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyFinally(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyFor(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyFrom(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyGlobal(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyIf(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyImport(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyIn(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyIs(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyLambda(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyNonlocal(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyNot(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyOr(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyPass(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyRaise(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyReturn(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyTry(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyWhile(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyWith(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyYield(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyMatch(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyCase(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyType(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos);
public sealed record PyDefault(uint StartPos, uint EndPos) : Symbol(StartPos, EndPos); // '_'


/*
   +       -       *       **      /       //      %      @
   <<      >>      &       |       ^       ~       :=
   <       >       <=      >=      ==      !=
   
   
   (       )       [       ]       {       }
   ,       :       .       ;       @       =       ->
   +=      -=      *=      /=      //=     %=      @=
   &=      |=      ^=      >>=     <<=     **=
 
 */