
using FluentValidation;

namespace Commander
{
    public interface ICommand
    {
        abstract bool ValidateThis();
    }
}
