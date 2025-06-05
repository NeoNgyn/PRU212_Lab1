using UnityEngine;

public class MissileController : MonoBehaviour
{
    public float missileSpeed = 22f;

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
        //if (collision.gameObject.tag == "Asteroid")
        //{
        //    GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
        //    Destroy(gm, 0.5f);

        //    if (GameManager.instance.explosionSound != null)
        //    {
        //        AudioSource.PlayClipAtPoint(GameManager.instance.explosionSound, transform.position);
        //    }
        //    string asteroidName = collision.gameObject.name;

        //    // Spawn item based on chance
        //    if (itemPrefabs.Length > 0 && Random.value <= itemSpawnChance)
        //    {
        //        // Select random item from prefabs
        //        GameObject itemToSpawn = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        //        // Calculate random offset for spawn position
        //        Vector2 randomOffset = new Vector2(
        //            Random.Range(-spawnOffsetRange.x, spawnOffsetRange.x),
        //            Random.Range(-spawnOffsetRange.y, spawnOffsetRange.y)
        //        );

        //        // Spawn the item
        //        Instantiate(itemToSpawn, transform.position + (Vector3)randomOffset, Quaternion.identity);
        //    }

        //    if (asteroidName.Contains("Asteroid") || asteroidName.Contains("AsterHuge2"))
        //    {
        //        // Tạo thiên thạch nhỏ
        //        CreateSmallAsteroids(collision.transform.position);
        //        // Thiên thạch lớn cũng có 40% cơ hội drop star
        //        if (Random.Range(0f, 1f) <= 0.4f && GameManager.instance.starItemPrefab != null)
        //        {
        //            Instantiate(GameManager.instance.starItemPrefab, collision.transform.position, Quaternion.identity);
        //        }
        //    }
        //    else
        //    {
        //        // Thiên thạch nhỏ có 60% cơ hội drop star
        //        if (Random.Range(0f, 1f) <= 0.5f && GameManager.instance.starItemPrefab != null)
        //        {
        //            Instantiate(GameManager.instance.starItemPrefab, collision.transform.position, Quaternion.identity);
        //        }
        //    }

        //    Destroy(this.gameObject);
        //    Destroy(collision.gameObject);

        //}
        if (collision.gameObject.tag == "Asteroid")
        {
            AsteroidController asteroid = collision.gameObject.GetComponent<AsteroidController>();

            if (asteroid != null)
            {
                // Gây sát thương
                bool isDead = asteroid.TakeDamage(1);

                if (isDead)
                {
                    GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
                    Destroy(gm, 0.5f);

                    if (GameManager.instance.explosionSound != null)
                    {
                        AudioSource.PlayClipAtPoint(GameManager.instance.explosionSound, transform.position);
                    }

                    string asteroidName = collision.gameObject.name;

                    // Spawn item ngẫu nhiên
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

            // Dù asteroid chưa chết, tên lửa vẫn bị phá huỷ
            Destroy(this.gameObject);
        }
    }
    private void CreateSmallAsteroids(Vector3 explosionPosition)
    {
        // Kiểm tra prefab có tồn tại không
        if (GameManager.instance.asterMedium3Prefab == null)
        {
            return;
        }

        // Tạo 3-5 thiên thạch nhỏ ngẫu nhiên
        int smallAsteroidCount = Random.Range(2, 3);

        for (int i = 0; i < smallAsteroidCount; i++)
        {
            // Tạo vị trí ngẫu nhiên xung quanh điểm nổ
            Vector2 randomOffset = Random.insideUnitCircle * 0.8f;
            Vector3 spawnPosition = explosionPosition + new Vector3(randomOffset.x, randomOffset.y, 0);

            // Tạo thiên thạch nhỏ
            GameObject smallAsteroid = Instantiate(
                GameManager.instance.asterMedium3Prefab,
                spawnPosition,
                Quaternion.identity
            );

            // Thiết lập tốc độ ngẫu nhiên cho thiên thạch nhỏ
            AsteroidController asteroidController = smallAsteroid.GetComponent<AsteroidController>();
            if (asteroidController != null)
            {
                asteroidController.speed = Random.Range(2f, 3.5f);
            }
        }
    }
}
