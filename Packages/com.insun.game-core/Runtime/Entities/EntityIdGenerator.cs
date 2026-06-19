using System.Collections.Generic;

namespace InSun.GameCore.Entities
{
    public sealed class EntityIdGenerator
    {
        private readonly Dictionary<uint, ushort> generations = new();
        private readonly Stack<uint> freeIds = new();

        private uint nextId;

        public EntityId Get()
        {
            if (freeIds.TryPop(out var id) == false)
            {
                id = nextId++;
                generations[id] = 0;
            }

            return new EntityId(
                value: id,
                generation: generations[id]
            );
        }

        public void Release(EntityId id)
        {
            var nextGeneration = generations[id.Value] + 1;
            generations[id.Value] = (ushort)nextGeneration;

            freeIds.Push(id.Value);
        }

        public bool IsValid(EntityId id)
        {
            if (generations.TryGetValue(id.Value, out var generation) == false)
            {
                return false;
            }

            return id.Generation == generation;
        }
    }
}
