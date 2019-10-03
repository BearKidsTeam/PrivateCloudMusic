namespace Pcm.Proxy.Middlewares
{
    public abstract class BaseMiddleware
    {
        public abstract Endpoint Create(Endpoint func);
    }
}