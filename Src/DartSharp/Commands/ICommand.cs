
namespace DartSharp.Commands
{
    public interface ICommand
    {
        // TODO maybe void
        object Execute(Context context);
    }
}
