using UnityEngine;

public class DeathEffectManager : MonoBehaviour
{
    public static DeathEffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void PlayDeathEffect(GameObject deathEffectPrefab, Vector3 position)
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, position, Quaternion.identity);
        }
    }
}
