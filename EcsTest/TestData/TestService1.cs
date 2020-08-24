using ObjectEcs.Interfaces;
using ObjectEcs.Services;

namespace EcsTest.TestData
{
    public class TestService1 : BaseEcsService, IEcsUpdateService, IEcsInitService, IEcsDestroyService
    {
        public TestService1(EcsStoreService storeService) : base(storeService)
        {
        }

        public void Update()
        {
            StaticStore.CountOfUpdate++;
        }

        public void Init()
        {
            StaticStore.CountOfInit++;
        }

        public void Destroy()
        {
            StaticStore.CountOfDestroy++;
        }
    }
}