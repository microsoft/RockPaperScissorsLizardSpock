namespace RPSLS.Game.Multiplayer.Builders
{
    public class BaseRequestBuilder<T> where T : new()
    {
        protected T _product;
        public BaseRequestBuilder()
        {
            _product = new T();
        }

        public T Build() => _product;
    }
}
