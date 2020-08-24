using System;
using System.Collections.Generic;
using ObjectEcs.Interfaces;
using ObjectEcs.Services;

namespace ObjectEcs.Models
{
    public class EcsWorld
    {
        private readonly EcsStoreService _ecsStoreService;

        public readonly Dictionary<Type, BaseEcsService> Services;
        
        private readonly HashSet<IEcsInitService> _initServices;
        private readonly HashSet<IEcsUpdateService> _updateServices;
        private readonly HashSet<IEcsDestroyService> _finishServices;
        
        public EcsWorld(EcsStoreService ecsStoreService)
        {
            _ecsStoreService = ecsStoreService;
            
            Services = new Dictionary<Type, BaseEcsService>();
            _initServices = new HashSet<IEcsInitService>();
            _updateServices = new HashSet<IEcsUpdateService>();
            _finishServices = new HashSet<IEcsDestroyService>();
        }

        public EcsWorld AddService<T>() where T: BaseEcsService
        {
            var service = (T)Activator.CreateInstance(typeof(T), _ecsStoreService);;
            
            Services.Add(typeof(T), service);
            
            if (service is IEcsInitService initService)
                _initServices.Add(initService);
            
            if (service is IEcsUpdateService updateService)
                _updateServices.Add(updateService);
            
            if (service is IEcsDestroyService finishService)
                _finishServices.Add(finishService);

            return this;
        }

        public void Update()
        {
            foreach (var updateService in _updateServices)
            {
                updateService.Update();
            }
        }

        public void Init()
        {
            foreach (var initService in _initServices)
            {
                initService.Init();
            }
        }

        public void Destroy()
        {
            foreach (var finishService in _finishServices)
            {
                finishService.Destroy();
            }
        }
        
    }
}