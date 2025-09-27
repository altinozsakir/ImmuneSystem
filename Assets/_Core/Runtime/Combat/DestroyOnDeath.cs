using UnityEngine;
using Core.Combat; // Health

/// Destroys this GameObject when its Health hits 0.
/// Plays optional VFX/SFX and disables physics/colliders first.
public class DestroyOnDeath : MonoBehaviour
{
    public Health health;
    [Min(0f)] public float delay = 0f;

    [Header("Optional FX")]
    public GameObject deathVFX;
    public AudioClip deathSFX;

    [Header("Clean-up")]
    public bool disableColliders = true;
    public bool disableRigidbody = true;
    public bool hideRenderers = false; // set true if you want it to vanish immediately

    void Awake()
    {
        if (!health) health = GetComponent<Health>();
    }

    void OnEnable()
    {
        if (health) health.onDeath.AddListener(HandleDeath);
    }
    void OnDisable()
    {
        if (health) health.onDeath.RemoveListener(HandleDeath);
    }

    void HandleDeath()
    {
        if (disableColliders)
            foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;

        if (disableRigidbody)
        {
            var rb = GetComponent<Rigidbody>();
            if (rb) { rb.isKinematic = true; rb.detectCollisions = false; }
        }

        if (hideRenderers)
            foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;

        if (deathVFX) Instantiate(deathVFX, transform.position, Quaternion.identity);
        if (deathSFX) AudioSource.PlayClipAtPoint(deathSFX, transform.position);

        Destroy(gameObject, delay);
    }
}
