using UnityEngine;

// Separate class for simply handling the particle systems associated with each state of a spawner
public class SpawnerSFX : MonoBehaviour
{
    // Required references
    [SerializeField]
    private Spawner _spawner;
    [SerializeField]
    private ParticleSystem _touchableSFX;
    [SerializeField]
    private ParticleSystem _acceleratedSFX;
    [SerializeField]
    private ParticleSystem _cooldownSFX;

    // Subscribe to Event Manager events on enable
    private void OnEnable()
    {
        EventManager.SpawnerEvent.OnResetState += Default;
        EventManager.SpawnerEvent.OnAccelerate += Accelerate;
        EventManager.SpawnerEvent.OnBeginCooldown += BeginCooldown;
    }

    // Unsubscribe to Event Manager events on disable
    private void OnDisable()
    {
        EventManager.SpawnerEvent.OnResetState -= Default;
        EventManager.SpawnerEvent.OnAccelerate -= Accelerate;
        EventManager.SpawnerEvent.OnBeginCooldown -= BeginCooldown;
    }

    // Begin state or going from cooldown state to normal
    private void Default(Spawner spawner)
    {
        // Only applied if this is attached to the correct spawner
        if (this._spawner != spawner)
            return;

        _touchableSFX.Play();
        _cooldownSFX.Stop();
    }

    // Leaving default state and going into accelerated state
    private void Accelerate(Spawner spawner)
    {
        // Only applied if this is attached to the correct spawner
        if (this._spawner != spawner)
            return;

        _acceleratedSFX.Play();
        _touchableSFX.Stop();
    }

    // Ending accelerated state and going into cooldown state
    private void BeginCooldown(Spawner spawner)
    {
        // Only applied if this is attached to the correct spawner
        if (this._spawner != spawner)
            return;

        _cooldownSFX.Play();
        _acceleratedSFX.Stop();
    }
}
