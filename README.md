# DartSharp

Dart-like programming language, implemented as interpreter using C#, with access .NET types and objects.
After completing the interpreter, maybe I could compile to C# code (or IL?).

Work in Progress.

## Examples

The interpreter is under development. You can use if, while, declare functions and variables.
You can start the interactive interpreter with command

```
dartsharp
```

It is the .exe produced by the compilation of DartSharp.Console project.

Hello world

```dart
print("Hello, world");
```

Fibonacci example:

```dart
int fib(int  n) {
  if (n < 2) return n;
  return fib(n - 1) + fib(n - 2);
}

main() {
  print('fib(20) = ${fib(20)}');
}
```

Access to native methods

```dart
var a = "foo";
print(a.Length);
3
print(a.Substring(1));
oo
print(a.ToUpper());
FOO
```

