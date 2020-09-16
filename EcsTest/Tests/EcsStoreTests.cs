using System;
using System.Linq;
using EcsTest.TestData;
using NUnit.Framework;
using ObjectEcs.Models;
using ObjectEcs.Services;

namespace EcsTest.Tests
{
    [TestFixture]
    public class EcsStoreTests
    {
        private EcsStore _ecsStore;
        private EcsStoreService _ecsStoreService;
        
        [SetUp]
        public void SetUp()
        {
            _ecsStore = new EcsStore();
            _ecsStoreService = new EcsStoreService(_ecsStore);
        }
        
        [Test]
        public void Add()
        {
            var entity = new EcsEntity().AddComponent(new TestComponent1());
            _ecsStoreService.AddEntity(entity);
            var actualEntity = _ecsStore.Entities.FirstOrDefault();

            Assert.AreEqual(_ecsStore.Entities.Count, 1);
            Assert.AreEqual(entity, actualEntity.Value);
            Assert.AreNotEqual(default(Guid), actualEntity.Value.Id);
            Assert.AreEqual(actualEntity.Value.Id, actualEntity.Key );
        }
        
        [Test]
        public void Add_FewEqualEntities()
        {
            var entity1 = new EcsEntity().AddComponent(new TestComponent1());
            var entity2 = new EcsEntity().AddComponent(new TestComponent1());
            _ecsStoreService.AddEntity(entity1).AddEntity(entity2);
            
            var actualEntities = _ecsStore.Entities.FirstOrDefault();

            Assert.AreEqual(_ecsStore.Entities.Count, 2);
            Assert.AreEqual(entity1, _ecsStore.Entities[entity1.Id]);
            Assert.AreEqual(entity2, _ecsStore.Entities[entity2.Id]);
        }
        
        [Test]
        public void Add_SavingGuid()
        {
            var entity = new EcsEntity().AddComponent(new TestComponent1());
            
            var existingGuid = Guid.NewGuid();
            entity.Id = existingGuid;
            
            _ecsStoreService.AddEntity(entity);
            var actualEntity = _ecsStore.Entities.FirstOrDefault();
            
            Assert.AreEqual(_ecsStore.Entities.Count, 1);
            Assert.AreEqual(entity, actualEntity.Value);
            Assert.AreEqual(existingGuid, actualEntity.Key);
            Assert.AreEqual(existingGuid, actualEntity.Value.Id);
        }
        
        [Test]
        public void Add_EntityWithComponent()
        {
            var entity = new EcsEntity();
            
            var component = new  TestComponent1();
            
            entity.AddComponent(component);
            _ecsStoreService.AddEntity(entity);
            
            var actualEntity = _ecsStore.Entities.FirstOrDefault();

            Assert.AreEqual(_ecsStore.Entities.Count, 1);
            Assert.AreEqual(entity, actualEntity.Value);

            var entityByComponent = _ecsStore.EntitiesByComponents[typeof(TestComponent1)]?.FirstOrDefault();
            Assert.AreEqual(entity, entityByComponent);
            Assert.AreEqual(1, _ecsStore.EntitiesByComponents.Count);
            Assert.AreEqual(1, _ecsStore.EntitiesByComponents[typeof(TestComponent1)].Count);
        }
        
        [Test]
        public void Add_ManyComponents()
        {
            var entity = new EcsEntity();
            
            var component1 = new  TestComponent1();
            var component2 = new  TestComponent2();
            
            entity
                .AddComponent(component1)
                .AddComponent(component2);
            
            _ecsStoreService.AddEntity(entity);
            
            var actualEntity = _ecsStore.Entities.FirstOrDefault();

            Assert.AreEqual(_ecsStore.Entities.Count, 1);
            Assert.AreEqual(entity, actualEntity.Value);

            var entityByComponent1 = _ecsStore.EntitiesByComponents[typeof(TestComponent1)]?.FirstOrDefault();
            var entityByComponent2 = _ecsStore.EntitiesByComponents[typeof(TestComponent2)]?.FirstOrDefault();

            Assert.AreEqual(entity, entityByComponent1);
            Assert.AreEqual(entity, entityByComponent2);
            
            Assert.AreEqual(2, _ecsStore.EntitiesByComponents.Count);
            Assert.AreEqual(1, _ecsStore.EntitiesByComponents[typeof(TestComponent1)].Count);
            Assert.AreEqual(1, _ecsStore.EntitiesByComponents[typeof(TestComponent2)].Count);
        } 
        
        [Test]
        public void Add_AddComponentToExistingEntity()
        {
            var entity = new EcsEntity();
            
            var component1 = new  TestComponent1();
            var component2 = new  TestComponent2();
            
            entity.AddComponent(component1);
            
            _ecsStoreService.AddEntity(entity);

            Assert.AreEqual(1, _ecsStore.EntitiesByComponents.Count);

            entity.AddComponent(component2);
            
            Assert.AreEqual(2, _ecsStore.EntitiesByComponents.Count);
            Assert.AreEqual(1, _ecsStore.EntitiesByComponents[typeof(TestComponent1)].Count);
            Assert.AreEqual(1, _ecsStore.EntitiesByComponents[typeof(TestComponent2)].Count);
            
            var entityByComponent1 = _ecsStore.EntitiesByComponents[typeof(TestComponent1)]?.FirstOrDefault();
            var entityByComponent2 = _ecsStore.EntitiesByComponents[typeof(TestComponent2)]?.FirstOrDefault();

            Assert.AreEqual(entity, entityByComponent1);
            Assert.AreEqual(entity, entityByComponent2);
        }

        [Test]
        public void Get_ByOneInterface()
        {
            var entity = new EcsEntity().AddComponent(new  TestComponent1());

            _ecsStoreService.AddEntity(entity);

            var actualEntity = _ecsStoreService.GetEntities(typeof(TestComponent1));
            Assert.AreEqual(1, actualEntity.Count);
            Assert.AreEqual(entity, actualEntity.FirstOrDefault());
        }
        
        [Test]
        public void Get_ComponentWithManyComponents_ByOne()
        {
            var entity = new EcsEntity()
                .AddComponent(new  TestComponent1())
                .AddComponent(new TestComponent2());

            _ecsStoreService.AddEntity(entity);

            var actualEntity = _ecsStoreService.GetEntities(typeof(TestComponent1));
            Assert.AreEqual(1, actualEntity.Count);
            Assert.AreEqual(entity, actualEntity.FirstOrDefault());
        }
        
        [Test]
        public void Get_ComponentWithManyComponentsByOne_WithFewEntities()
        {
            var entity1 = new EcsEntity()
                .AddComponent(new  TestComponent1())
                .AddComponent(new TestComponent2());
            
            var entity2 = new EcsEntity()
                .AddComponent(new TestComponent2());

            _ecsStoreService.AddEntity(entity1).AddEntity(entity2);

            var actualEntity = _ecsStoreService.GetEntities(typeof(TestComponent1));
            Assert.AreEqual(1, actualEntity.Count);
            Assert.AreEqual(entity1, actualEntity.FirstOrDefault());
        }
        
        [Test]
        public void Get_FewComponents_ByOneInterface()
        {
            var entity1 = new EcsEntity()
                .AddComponent(new  TestComponent1())
                .AddComponent(new TestComponent2());
            
            var entity2 = new EcsEntity()
                .AddComponent(new TestComponent2());

            _ecsStoreService.AddEntity(entity1).AddEntity(entity2);

            var actualEntity = _ecsStoreService.GetEntities(typeof(TestComponent2));
            Assert.AreEqual(2, actualEntity.Count);
            Assert.AreEqual(entity1, actualEntity[0]);
            Assert.AreEqual(entity2, actualEntity[1]);
        }
        
        [Test]
        public void Get_FewComponents_ByOneInterface_WithDifferentTimeCreatingOfComponent()
        {
            var entity1 = new EcsEntity()
                .AddComponent(new TestComponent2());
            
            var entity2 = new EcsEntity()
                .AddComponent(new TestComponent1());

            _ecsStoreService.AddEntity(entity1).AddEntity(entity2);

            entity2.AddComponent(new TestComponent2());

            var actualEntities = _ecsStoreService.GetEntities(typeof(TestComponent2));
            Assert.AreEqual(2, actualEntities.Count);
            Assert.AreEqual(entity1, actualEntities[0]);
            Assert.AreEqual(entity2, actualEntities[1]);
        }

        [Test]
        public void Get_Few_ByFewComponents()
        {
            var entity1 = new EcsEntity().AddComponent(new TestComponent1()).AddComponent(new TestComponent2());
            var entity2 = new EcsEntity().AddComponent(new TestComponent1());
            
            _ecsStoreService.AddEntity(entity1).AddEntity(entity2);

            var actualEntities = _ecsStoreService
                .GetEntities(typeof(TestComponent1), typeof(TestComponent2));
            
            Assert.AreEqual(1, actualEntities.Count);
            Assert.AreEqual(entity1, actualEntities[0]);
        }

        [Test]
        public void RemoveEntity()
        {
            var entity1 = new EcsEntity().AddComponent(new TestComponent1());
            _ecsStoreService.AddEntity(entity1);
            
            Assert.AreEqual(1, _ecsStore.Entities.Count);
            
            _ecsStoreService.RemoveEntity(entity1);
            Assert.AreEqual(0, _ecsStore.Entities.Count);
        }
        
        [Test]
        public void Remove_FewComponents()
        {
            var entity1 = new EcsEntity().AddComponent(new TestComponent1()).AddComponent(new TestComponent2());
            
            _ecsStoreService.AddEntity(entity1);
            Assert.AreEqual(1, _ecsStore.Entities.Count);
            
            _ecsStoreService.RemoveEntity(entity1);
            Assert.AreEqual(0, _ecsStore.Entities.Count);
            Assert.AreEqual(2, _ecsStore.EntitiesByComponents.Count);

            foreach (var componentKeyValue in _ecsStore.EntitiesByComponents)
            {
                Assert.AreEqual(0, componentKeyValue.Value.Count);
            }
        }
        
        [Test]
        public void Remove_CheckAnother()
        {
            var entity1 = new EcsEntity().AddComponent(new TestComponent1()).AddComponent(new TestComponent2());
            var entity2 = new EcsEntity().AddComponent(new TestComponent1());            
            
            _ecsStoreService.AddEntity(entity1).AddEntity(entity2);
            
            Assert.AreEqual(2, _ecsStore.Entities.Count);
            
            _ecsStoreService.RemoveEntity(entity1);
            Assert.AreEqual(1, _ecsStore.Entities.Count);
            Assert.AreEqual(entity2, _ecsStore.Entities.FirstOrDefault().Value);

            var existingEntityForComponent1 = _ecsStore.EntitiesByComponents[typeof(TestComponent1)];
            var existingEntityForComponent2 = _ecsStore.EntitiesByComponents[typeof(TestComponent2)];
            
            Assert.AreEqual(1, existingEntityForComponent1.Count);
            Assert.AreEqual(entity2, existingEntityForComponent1.FirstOrDefault());
            Assert.AreEqual(0, existingEntityForComponent2.Count);
        }

        [Test]
        public void Get_ById_ExistEntity()
        {
            var entity1 = new EcsEntity()
                .AddComponent(new TestComponent1());

            _ecsStoreService.AddEntity(entity1);

            var actualEntity = _ecsStoreService.GetEntity(entity1.Id);
            
            Assert.AreEqual(entity1, actualEntity);
        }
        
        [Test]
        public void Get_ById_NoEntity()
        {
            var actualEntity = _ecsStoreService.GetEntity(Guid.NewGuid());
            
            Assert.AreEqual(null, actualEntity);
        }
        
        [Test]
        public void Get_ById_AnotherEntity()
        {
            var entity1 = new EcsEntity()
                .AddComponent(new TestComponent1());

            _ecsStoreService.AddEntity(entity1);
            
            var actualEntity = _ecsStoreService.GetEntity(Guid.NewGuid());
            
            Assert.AreEqual(null, actualEntity);
        }
    }
}