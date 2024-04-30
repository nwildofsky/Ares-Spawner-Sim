using UnityEngine.AI;
using UnityEngine.Events;

// Custom event broadcasting and listening system set up entirely in code
// Class and all events are global static, compartmentalized into structs for organization
public static class EventManager
{
    public struct Game
    {
        public static UnityAction OnGameEnd;
    }

    public struct SpawnerEvent
    {
        public static UnityAction<Spawner> OnResetState;
        public static UnityAction<Spawner> OnAccelerate;
        public static UnityAction<Spawner> OnBeginCooldown;
    }

    public struct AgentEvent
    {
        public static UnityAction<NavMeshAgent, int> OnAgentAnimatedCollision;
    }

    public struct UI
    {
        public static UnityAction<AgentType, int> OnAgentCountChanged;
        public static UnityAction OnAgentCountHeard;
    }

}
