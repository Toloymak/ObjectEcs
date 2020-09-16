using System;
using System.Collections.Generic;
using System.Linq;
using ObjectEcs.Models;

namespace ObjectEcs.Services
{
    public class EcsStoreService
    {
        private readonly EcsStore _store;

        public EcsStoreService(EcsStore store)
        {
            _store = store;
        }

        public EcsStoreService AddEntity(EcsEntity entity)
        {
            if (!entity.Components.Any())
                throw new ArgumentException("Entity have to contain any component");

            if (entity.Id == default)
                entity.Id = Guid.NewGuid();
            
            entity.UpdateFilterCollections = (ecsEntity, component) =>
                AddToComponentCollection(entity, component.GetType());
            
            _store.Entities.Add(entity.Id, entity);

            foreach (var component in entity.Components)
            {
                AddToComponentCollection(entity, component.Key);
            }
            
            return this;
        }

        private void AddToComponentCollection(EcsEntity entity, Type type)
        {
            if (_store.EntitiesByComponents.TryGetValue(type, out var value))
            {
                value.Add(entity);
            }
            else
            {
                _store.EntitiesByComponents.Add(type, new HashSet<EcsEntity>() {entity});
            }
        }

        public IList<EcsEntity> GetEntities(params Type[] types)
        {
            HashSet<EcsEntity> shortestCollection = null;

            // Looking for smaller collection
            foreach (var type in types)
            {
                if (!_store.EntitiesByComponents.TryGetValue(type, out var entities))
                    continue;
                
                if (shortestCollection == null || shortestCollection.Count > entities.Count)
                    shortestCollection = entities;
            }

            if (shortestCollection == null || !shortestCollection.Any())
                return new List<EcsEntity>();

            var result = shortestCollection
                .Where(e => types.All(t => e.Components.ContainsKey(t)))
                .ToList();

            return result;
        }

        public EcsEntity GetEntity(Guid id)
        {
            _store.Entities.TryGetValue(id, out var entity);
            return entity;
        }

        public void RemoveEntity(EcsEntity ecsEntity)
        {
            foreach (var component in ecsEntity.Components)
            {
                _store.EntitiesByComponents[component.Key].Remove(ecsEntity);
            }

            _store.Entities.Remove(ecsEntity.Id);
            ecsEntity.Dispose();
        }
    }
}