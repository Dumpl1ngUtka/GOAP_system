# Unity AI Navigation

> **Scope**: Navigation and pathfinding using the com.unity.ai.navigation package (AI Navigation 1.x/2.x). Covers NavMeshSurface, NavMeshAgent, NavMeshLink, and runtime baking patterns.
> **Load when**: implementing character movement, building NavMeshes, configuring agents, or optimizing pathfinding for Unity 2022.2+ or Unity 6 projects.

---

## Component-Based Workflow

The modern AI Navigation package replaces the legacy Navigation Window with a flexible, component-based system.

### NavMeshSurface
*   **Decouple from Window**: Always use `NavMeshSurface` components attached to environment root objects instead of the legacy global bake.
*   **Per-Agent Meshes**: Create separate `NavMeshSurface` components for different agent types (e.g., "Humanoid", "Vehicle", "LargeMonster").
*   **Collect Geometry**: Use the "Collect Objects" setting to define what geometry is baked:
    *   **All**: Everything in the scene.
    *   **Volume**: Only within a `NavMeshModifierVolume`.
    *   **Children**: Only the children of the `NavMeshSurface` object (preferred for prefabs).
*   **Prefab Support**: Bake NavMeshes directly into Prefabs. This is essential for modular environment pieces and procedural level generation.

### NavMeshModifier & ModifierVolume
*   **Inclusion/Exclusion**: Use `NavMeshModifier` to include or exclude specific objects from baking without relying on "Navigation Static" flags.
*   **Area Assignment**: Use modifiers to assign different Area Types (e.g., "Water", "Lava", "Road") to specific meshes.
*   **Volume Control**: Use `NavMeshModifierVolume` for area assignment within a defined 3D space, regardless of mesh boundaries.

## Navigation Logic

### NavMeshAgent Configuration
*   **Avoidance Priority**: In crowd scenarios, randomize `avoidancePriority` (0-99) to prevent agents from getting stuck when moving to the same target.
*   **Quality Levels**: Use "High Quality" avoidance for hero characters and "None" or "Low Quality" for background NPCs on mobile/Android.
*   **Stopping Distance**: Set an appropriate `stoppingDistance` to prevent "jittering" when the agent reaches its destination.

### Dynamic Navigation
*   **NavMeshObstacle**: 
    *   Use **Carving** for obstacles that move occasionally and then stay still (e.g., a pushable crate).
    *   **Disable Carving** for fast-moving objects to rely on velocity-based avoidance.
*   **NavMeshLink**: Use links for gaps, ladders, jumps, or teleporters. They are more flexible than legacy Off-Mesh Links and can be placed anywhere.

## Runtime Baking (Mobile/Android)

Performance is critical for Android.

*   **Asynchronous Updates**: Use `NavMeshSurface.UpdateNavMesh(navMeshData)` for runtime updates to avoid frame drops.
*   **Voxel Size Optimization**: Use the largest possible "Voxel Size" that still allows agents to pass through narrow points. Avoid extremely small voxels.
*   **Simplify Geometry**: Use simplified proxy colliders (e.g., Boxes, Capsules) instead of complex Mesh Colliders for NavMesh generation.
*   **Limit Update Frequency**: Do not update the NavMesh every frame. Use a throttled update or trigger updates only when environment changes significantly.

## Code Examples

### Simple Movement
```csharp
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    
    public void MoveTo(Vector3 destination)
    {
        if (_agent.isOnNavMesh)
        {
            _agent.SetDestination(destination);
        }
    }
}
```

### Runtime Baking
```csharp
using Unity.AI.Navigation;
using UnityEngine;

public class RuntimeBaker : MonoBehaviour
{
    [SerializeField] private NavMeshSurface _surface;

    public void RefreshNavMesh()
    {
        // Asynchronous update to prevent stuttering
        _surface.UpdateNavMesh(_surface.navMeshData);
    }
}
```

## Anti-patterns

*   **Legacy Dependency**: Mixing `NavMeshSurface` with the legacy Navigation Window "Bake" button. This leads to inconsistent NavMesh data.
*   **Navigation Static Abuse**: Relying on the "Navigation Static" flag. Use `NavMeshModifier` or "Collect Objects: Children" for better control.
*   **Blocking Main Thread**: Using `NavMesh.BuildNavMesh()` at runtime instead of the asynchronous `NavMeshSurface.UpdateNavMesh()`.
*   **Ignoring Agent Types**: Using a single NavMesh for both small and large agents, leading to clipping or pathfinding failures.
