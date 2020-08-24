using System;
using EcsTest.TestData;
using NUnit.Framework;
using ObjectEcs.Models;
using ObjectEcs.Services;

namespace EcsTest.Tests
{
    [TestFixture]
    public class EcsWorldTests
    {
        private EcsWorld _ecsWorld;
        private EcsStoreService _ecsStoreService;
        private EcsStore _ecsStore;

        [SetUp]
        public void SetUp()
        {
            _ecsStore = new EcsStore();
            _ecsStoreService = new EcsStoreService(_ecsStore);
            _ecsWorld = new EcsWorld(_ecsStoreService);

            StaticStore.CountOfDestroy = 0;
            StaticStore.CountOfInit = 0;
            StaticStore.CountOfUpdate = 0;
        }

        [Test]
        public void Add_OneService()
        {
            _ecsWorld.AddService<TestService1>();
            
            Assert.AreEqual(1, _ecsWorld.Services.Count);
            Assert.NotNull(_ecsWorld.Services[typeof(TestService1)]);
        }
        
        [Test]
        public void Add_DuplicateOfService_Fail()
        {
            _ecsWorld.AddService<TestService1>();

            Assert.AreEqual(1, _ecsWorld.Services.Count);
            Assert.NotNull(_ecsWorld.Services[typeof(TestService1)]);

            Assert.Throws<ArgumentException>(() => _ecsWorld.AddService<TestService1>());
        }

        [Test]
        public void Add_FewServices()
        {
            _ecsWorld.AddService<TestService1>().AddService<TestService2>();
            
            Assert.AreEqual(2, _ecsWorld.Services.Count);
            Assert.NotNull(_ecsWorld.Services[typeof(TestService1)]);
            Assert.NotNull(_ecsWorld.Services[typeof(TestService2)]);
        }

        [Test]
        public void Init()
        {
            _ecsWorld.AddService<TestService3UpdateInitFinish>();
            
            _ecsWorld.Init();
            
            AssetStaticStore(1, 0, 0);
        }
        
        [Test]
        public void Init_fewTimes()
        {
            _ecsWorld.AddService<TestService3UpdateInitFinish>();
            
            _ecsWorld.Init();
            _ecsWorld.Init();
            
            AssetStaticStore(2, 0, 0);
        }
        
        [Test]
        public void Run()
        {
            _ecsWorld.AddService<TestService3UpdateInitFinish>();
            
            _ecsWorld.Update();
            
            AssetStaticStore(0, 1, 0);
        }
        
        [Test]
        public void Run_fewTimes()
        {
            _ecsWorld.AddService<TestService3UpdateInitFinish>();
            
            _ecsWorld.Update();
            _ecsWorld.Update();
            
            AssetStaticStore(0, 2, 0);
        }
        
        [Test]
        public void Destroy()
        {
            _ecsWorld.AddService<TestService3UpdateInitFinish>();
            
            _ecsWorld.Destroy();

            AssetStaticStore(0, 0, 1);
        }
        
        [Test]
        public void Destroy_fewTimes()
        {
            _ecsWorld.AddService<TestService3UpdateInitFinish>();
            
            _ecsWorld.Destroy();
            _ecsWorld.Destroy();

            AssetStaticStore(0, 0, 2);
        }
        
        [Test]
        public void InitUpdateDestroy()
        {
            _ecsWorld.AddService<TestService3UpdateInitFinish>();
            
            _ecsWorld.Init();
            _ecsWorld.Update();
            _ecsWorld.Update();
            _ecsWorld.Destroy();

            AssetStaticStore(1, 2, 1);
        }

        private void AssetStaticStore(int init, int update, int destroy)
        {
            Assert.AreEqual(init, StaticStore.CountOfInit);
            Assert.AreEqual(destroy, StaticStore.CountOfDestroy);
            Assert.AreEqual(update, StaticStore.CountOfUpdate);
        }
    }
}