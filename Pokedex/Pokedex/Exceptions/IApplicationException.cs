namespace Pokedex.Exceptions
{
    public interface IApplicationException
    {
        int GetCode();
        string GetMessage();
    }
}
