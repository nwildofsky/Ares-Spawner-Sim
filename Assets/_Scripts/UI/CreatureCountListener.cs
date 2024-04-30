using UnityEngine;

// Intermediate class which listens to all UI events regarding the creature counts, adjusts
// the actual counts and percentages, then sends another event to a display class which updates the UI
public class CreatureCountListener : MonoBehaviour
{
    // Individual Counts
    public int BatCount {  get; private set; }
    public int CrabCount { get; private set; }
    public int SalamanderCount { get; private set; }
    public int SpiderCount { get; private set; }

    // Total
    public int Total {  get; private set; }

    // Calculated Percentages
    public float BatCountAsPercentage { get => BatCount / (float)Total; }
    public float CrabCountAsPercentage { get => CrabCount / (float)Total; }
    public float SalamanderCountAsPercentage { get => SalamanderCount / (float)Total; }
    public float SpiderCountAsPercentage { get => SpiderCount / (float)Total; }

    // Subscribe to Event Manager events on enable
    private void OnEnable()
    {
        EventManager.UI.OnAgentCountChanged += AdjustCount;
    }

    // Unsubscribe to Event Manager events on disable
    private void OnDisable()
    {
        EventManager.UI.OnAgentCountChanged -= AdjustCount;
    }

    // Accepts the Type that changed and the new count and recalculates fields
    private void AdjustCount(AgentType type, int count)
    {
        switch (type)
        {
            case AgentType.Bat:
                Total += count - BatCount;
                BatCount = count;
                break;
            case AgentType.Crab:
                Total += count - CrabCount;
                CrabCount = count;
                break;
            case AgentType.Salamander:
                Total += count - SalamanderCount;
                SalamanderCount = count;
                break;
            case AgentType.Spider:
                Total += count - SpiderCount;
                SpiderCount = count;
                break;
            default: break;
        }

        // Sends the event on to display scripts
        EventManager.UI.OnAgentCountHeard?.Invoke();
    }
}
