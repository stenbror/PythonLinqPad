# PythonLinqPad

A LinqPad like tool with Roslyn like Python 3.14 Interpreter, that will visually show parse tree with nodes, symbols and trivia.

## Building and testing

 - dotnet build
 - dotnet test

Produces a single dll that can be used for Roslyn like analyzing of Python Source code with nodes, symbols and trivia. We are starting from
atom rule and moving upwards in expression parsing for now. This is early version and not able to parse much. Check Unittests for progress.
