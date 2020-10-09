namespace RayStudio.UtilScripts.Pool
{
    public interface IPoolItem
    {
        void AwakeFromPool();
        void RecycleToPool();
    }
}
