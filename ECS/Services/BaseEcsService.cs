namespace ObjectEcs.Services
{
    public abstract class BaseEcsService
    {
        protected EcsStoreService StoreService;

        protected BaseEcsService(EcsStoreService storeService)
        {
            StoreService = storeService;
        }
    }
}