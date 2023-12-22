namespace WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.Cache
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        T Set<T>(string key, T value);

        T Set<T>(string key, T value, int minute);
    }
}
