using System;
using System.Collections.Generic;
using ObjectEcs.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectEcs.Models
{
    public class EcsEntity : IDisposable
    {
        public EcsEntity()
        {
            Components = new Dictionary<Type, IEcsComponent>();
        }
        
        public Guid Id { get; set; }
        public GameObject Obj { get; private set; }
        public IDictionary<Type, IEcsComponent> Components { get; private set; }

        public EcsEntity AddComponent<T>(T component) where T : IEcsComponent
        {
            if (Components.ContainsKey(typeof(T)))
                throw new ArgumentException($"Component with type {typeof(T)} already exist");
            
            Components.Add(typeof(T), component);
            UpdateFilterCollections(this, component);
            return this;
        }

        public EcsEntity AddGameObject(GameObject gameObject)
        {
            gameObject.tag = Id.ToString();
            Obj = gameObject;
            return this;
        }
        
        public Action<EcsEntity, IEcsComponent> UpdateFilterCollections = (entity, component) => { };

        public void Dispose()
        {
            UpdateFilterCollections = null;
            Components = null;

            if (Obj != null)
            {
                Object.Destroy(Obj);
                Obj = null;
            }
        }
    }
}