namespace DartSharp.Expressions
{
    using System;

    public interface IExpression
    {
        object Evaluate(DartSharp.Context context);
    }
}
