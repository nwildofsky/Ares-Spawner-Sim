using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public struct Game
    {
        public static UnityAction OnTouchSpawner;
    }

    public struct UI
    {
        public static UnityAction<AgentType, int> OnAgentCountChanged;
        public static UnityAction OnAgentCountHeard;
    }

}