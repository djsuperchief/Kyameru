namespace Kyameru.Core.Contracts
{
    public interface IChain<T> where T : class
    {
        IChain<T> SetNext(IChain<T> next);

        void Handle(T item);
    }
}