using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public float duration = 3f;

    [Header("Item Spawning")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private float itemSpawnChance = 0.1f;
    [SerializeField] private Vector2 spawnOffsetRange = new Vector2(0.5f, 0.5f);

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid"))
        {
            AsteroidController asteroid = other.GetComponent<AsteroidController>();
            if (asteroid != null)
            {
                // Deal max damage: instantly kill the asteroid
                bool isDead = asteroid.TakeDamage(asteroid.health);

                if (isDead)
                {
                    GameObject explosion = Instantiate(GameManager.instance.explosion, other.transform.position, Quaternion.identity);
                    Destroy(explosion, 0.5f);

                    if (GameManager.instance.explosionSound != null)
                    {
                        AudioSource.PlayClipAtPoint(GameManager.instance.explosionSound, other.transform.position);
                    }

                    // Spawn items if any
                    if (itemPrefabs.Length > 0 && Random.value <= itemSpawnChance)
                    {
                        GameObject itemToSpawn = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
                        Vector2 randomOffset = new Vector2(
                            Random.Range(-spawnOffsetRange.x, spawnOffsetRange.x),
                            Random.Range(-spawnOffsetRange.y, spawnOffsetRange.y)
                        );
                        Instantiate(itemToSpawn, other.transform.position + (Vector3)randomOffset, Quaternion.identity);
                    }

                    // Spawn star item chance like missile does
                    if (Random.Range(0f, 1f) <= 0.5f && GameManager.instance.starItemPrefab != null)
                    {
                        Instantiate(GameManager.instance.starItemPrefab, other.transform.position, Quaternion.identity);
                    }

                    Destroy(other.gameObject);
                }
            }
        }
    }
}
