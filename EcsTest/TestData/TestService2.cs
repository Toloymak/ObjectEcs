using ObjectEcs.Services;

namespace EcsTest.TestData
{
    public class TestService2 : BaseEcsService
    {
        public TestService2(EcsStoreService storeService) : base(storeService)
        {
        }
    }
}