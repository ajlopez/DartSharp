namespace DartSharp.Commands
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Expressions;
    using DartSharp.Language;
    using DartSharp.Methods;

    public class IfCommand : ICommand
    {
        private IExpression condition;
        private ICommand thencommand;
        private ICommand elsecommand;

        public IfCommand(IExpression condition, ICommand thencommand)
            : this(condition, thencommand, null)
        {
        }

        public IfCommand(IExpression condition, ICommand thencommand, ICommand elsecommand)
        {
            this.condition = condition;
            this.thencommand = thencommand;
            this.elsecommand = elsecommand;
        }

        public IExpression Condition { get { return this.condition; } }

        public ICommand ThenCommand { get { return this.thencommand; } }

        public ICommand ElseCommand { get { return this.elsecommand; } }

        public object Execute(Context context)
        {
            if (Predicates.IsTrue(this.condition.Evaluate(context)))
                this.thencommand.Execute(context);
            else if (this.elsecommand != null)
                this.elsecommand.Execute(context);

            return null;
        }
    }
}

