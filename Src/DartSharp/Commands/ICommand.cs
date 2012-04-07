
namespace DartSharp.Commands
{
    public interface ICommand
    {
        object Execute(Context context);
    }
}
