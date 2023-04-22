namespace Scellecs.Morpeh {
    using System.Runtime.CompilerServices;
    using Collections;
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal static class ArchetypeExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Dispose(this Archetype archetype) {
            archetype.usedInNative = false;
            
            archetype.id      = -1;
            archetype.length  = -1;
            archetype.world   = null;
            archetype.entities?.Clear();
            archetype.entities = null;
            
            archetype.entitiesNative?.Clear();
            archetype.entitiesNative = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(this Archetype archetype, Entity entity) {
            archetype.length++;
            
            if (archetype.usedInNative) {
                entity.indexInCurrentArchetype = archetype.entitiesNative.Add(entity.entityId.id);
            }
            else {
                archetype.entities.Set(entity.entityId.id);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove(this Archetype archetype, Entity entity) {
            archetype.length--;

            if (archetype.usedInNative) {
                var index = entity.indexInCurrentArchetype;
                if (archetype.entitiesNative.RemoveAtSwap(index, out int newValue)) {
                    archetype.world.entities[newValue].indexInCurrentArchetype = index;
                }
            }
            else {
                archetype.entities.Unset(entity.entityId.id);
            }
            if (archetype.length == 0) {
                Pool(archetype);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Pool(Archetype archetype) {
            archetype.world.archetypes.Remove(archetype.id, out _);
            archetype.world.removedArchetypes.Add(archetype);
            archetype.world.archetypesCount--;
            archetype.usedInNative = false;
            archetype.entities.Clear();
            archetype.entitiesNative = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFilter(this Archetype archetype, Filter filter) {
            archetype.filters.Add(filter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFilter(this Archetype archetype, Filter filter) {
            archetype.filters.Remove(filter);
        }
    }
}
