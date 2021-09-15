using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public abstract class CommanderValidator<T> : AbstractValidator<T> where T : Command
    {
        private string Message { get; set; }

        public new ICommandResult Validate(T instance)
        {
            if (Equals(instance, default(T)))
            {
                return CommandResult.IsFailed(Message ?? string.Empty);
            }

            var result = base.Validate(instance);

            return result.IsValid ? CommandResult.Success() : CommandResult.IsFailed(Message ?? result.ToString());
        }

        public ValueTask<ICommandResult> ValidateAsync(T instance) => ValueTask.FromResult(Validate(instance));

        public void CustomMessage(string message) => Message = message;
    }
}
