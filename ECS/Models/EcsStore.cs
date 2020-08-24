using System;
using System.Collections.Generic;

namespace ObjectEcs.Models
{
    public class EcsStore
    {
        public EcsStore()
        {
            Entities = new Dictionary<Guid, EcsEntity>();
            EntitiesByComponents = new Dictionary<Type, HashSet<EcsEntity>>();
        }
        
        public IDictionary<Guid, EcsEntity> Entities { get; set; }
        public IDictionary<Type, HashSet<EcsEntity>>  EntitiesByComponents { get; set; }
    }
}