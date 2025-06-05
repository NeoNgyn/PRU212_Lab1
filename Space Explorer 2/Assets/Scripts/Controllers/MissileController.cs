using UnityEngine;

public class MissileController : MonoBehaviour
{
    public float missileSpeed = 22f;
    public float damage = 5f; // Damage dealt by this missile

    [Header("Item Spawning")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private float itemSpawnChance = 0.1f;
    [SerializeField] private Vector2 spawnOffsetRange = new Vector2(0.5f, 0.5f);

    void Update()
    {
        transform.Translate(Vector3.down * missileSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Asteroid")
        {
            AsteroidController asteroid = collision.gameObject.GetComponent<AsteroidController>();

            if (asteroid != null)
            {
                // Apply missile's damage to asteroid
                bool isDead = asteroid.TakeDamage(damage);

                if (isDead)
                {
                    GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
                    Destroy(gm, 0.5f);

                    if (GameManager.instance.explosionSound != null)
                    {
                        AudioSource.PlayClipAtPoint(GameManager.instance.explosionSound, transform.position);
                    }

                    string asteroidName = collision.gameObject.name;

                    // Spawn item randomly
                    if (itemPrefabs.Length > 0 && Random.value <= itemSpawnChance)
                    {
                        GameObject itemToSpawn = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
                        Vector2 randomOffset = new Vector2(
                            Random.Range(-spawnOffsetRange.x, spawnOffsetRange.x),
                            Random.Range(-spawnOffsetRange.y, spawnOffsetRange.y)
                        );
                        Instantiate(itemToSpawn, transform.position + (Vector3)randomOffset, Quaternion.identity);
                    }

                    if (asteroidName.Contains("Asteroid") || asteroidName.Contains("AsterHuge2"))
                    {
                        CreateSmallAsteroids(collision.transform.position);

                        if (Random.Range(0f, 1f) <= 0.4f && GameManager.instance.starItemPrefab != null)
                        {
                            Instantiate(GameManager.instance.starItemPrefab, collision.transform.position, Quaternion.identity);
                        }
                    }
                    else
                    {
                        if (Random.Range(0f, 1f) <= 0.5f && GameManager.instance.starItemPrefab != null)
                        {
                            Instantiate(GameManager.instance.starItemPrefab, collision.transform.position, Quaternion.identity);
                        }
                    }

                    Destroy(collision.gameObject);
                }
            }

            // Destroy missile regardless of asteroid's death
            Destroy(this.gameObject);
        }
    }

    private void CreateSmallAsteroids(Vector3 explosionPosition)
    {
        if (GameManager.instance.asterMedium3Prefab == null)
        {
            return;
        }

        int smallAsteroidCount = Random.Range(2, 3);

        for (int i = 0; i < smallAsteroidCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 0.8f;
            Vector3 spawnPosition = explosionPosition + new Vector3(randomOffset.x, randomOffset.y, 0);

            GameObject smallAsteroid = Instantiate(
                GameManager.instance.asterMedium3Prefab,
                spawnPosition,
                Quaternion.identity
            );

            AsteroidController asteroidController = smallAsteroid.GetComponent<AsteroidController>();
            if (asteroidController != null)
            {
                asteroidController.speed = Random.Range(2f, 3.5f);
            }
        }
    }
}
