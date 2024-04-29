using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public static class EventManager
{
    public struct Game
    {
        public static UnityAction OnTouchSpawner;
        public static UnityAction<NavMeshAgent, int> OnAgentCollision;
        public static UnityAction OnGameEnd;
    }

    public struct UI
    {
        public static UnityAction<AgentType, int> OnAgentCountChanged;
        public static UnityAction OnAgentCountHeard;
    }

}
