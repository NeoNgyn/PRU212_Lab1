using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour
{
    public float damagePerSecond = 20f;
    public float pushForce = 5f;
    public LineRenderer lineRenderer;
    public float maxDistance = 20f;

    public AudioClip laserIntroClip;
    public AudioClip laserLoopClip;
    public float fadeOutDuration = 0.1f;
    public float laserVolume = 0.1f;

    // For explosion and item spawning like MissileController
    [Header("Item Spawning")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private float itemSpawnChance = 0.1f;
    [SerializeField] private Vector2 spawnOffsetRange = new Vector2(0.5f, 0.5f);

    private AudioSource introSource;
    private AudioSource loopSource;
    private bool isFiring = false;

    private int asteroidLayerMask;
    private SpriteRenderer laserSpriteRenderer;
    private Coroutine fadeOutCoroutine;

    private void Awake()
    {
        Physics2D.queriesHitTriggers = true;
        asteroidLayerMask = LayerMask.GetMask("Asteroid");

        laserSpriteRenderer = GetComponent<SpriteRenderer>();

        introSource = gameObject.AddComponent<AudioSource>();
        introSource.clip = laserIntroClip;
        introSource.playOnAwake = false;
        introSource.loop = false;
        introSource.volume = laserVolume;

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.clip = laserLoopClip;
        loopSource.playOnAwake = false;
        loopSource.loop = true;
        loopSource.volume = laserVolume;
    }

    private void Update()
    {
        if (isFiring)
        {
            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
                introSource.volume = laserVolume;
                loopSource.volume = laserVolume;
                fadeOutCoroutine = null;
            }

            if (!introSource.isPlaying && !loopSource.isPlaying)
            {
                StartLaserSound();
            }

            FireLaser();
        }
    }

    public void SetFiring(bool firing)
    {
        if (firing == isFiring) return;

        isFiring = firing;

        if (!isFiring)
        {
            if (fadeOutCoroutine == null && gameObject.activeInHierarchy)
                fadeOutCoroutine = StartCoroutine(FadeOutAndStop(fadeOutDuration));
        }
        else
        {
            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
                introSource.volume = laserVolume;
                loopSource.volume = laserVolume;
                fadeOutCoroutine = null;
            }

            if (!introSource.isPlaying && !loopSource.isPlaying && gameObject.activeInHierarchy)
            {
                StartLaserSound();
            }
        }
    }

    private void StartLaserSound()
    {
        if (laserIntroClip == null || laserLoopClip == null)
        {
            Debug.LogWarning("Missing intro or loop clip.");
            return;
        }

        double startTime = AudioSettings.dspTime + 0.05;

        introSource.volume = laserVolume;
        loopSource.volume = laserVolume;

        introSource.PlayScheduled(startTime);
        loopSource.PlayScheduled(startTime + laserIntroClip.length);
    }

    private IEnumerator FadeOutAndStop(float duration)
    {
        float startVolumeIntro = introSource.volume;
        float startVolumeLoop = loopSource.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            introSource.volume = Mathf.Lerp(startVolumeIntro, 0f, t);
            loopSource.volume = Mathf.Lerp(startVolumeLoop, 0f, t);
            yield return null;
        }

        introSource.Stop();
        loopSource.Stop();

        introSource.volume = laserVolume;
        loopSource.volume = laserVolume;
        fadeOutCoroutine = null;
    }

    private void FireLaser()
    {
        Vector3 direction = transform.up;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, asteroidLayerMask);
        float laserLength = maxDistance;

        if (hit.collider != null)
        {
            AsteroidController asteroid = hit.collider.GetComponent<AsteroidController>();
            Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();

            if (asteroid != null)
            {
                bool destroyed = asteroid.TakeDamage(damagePerSecond * Time.deltaTime);
                if (destroyed)
                {
                    // Explosion effect
                    GameObject explosion = Instantiate(GameManager.instance.explosion, hit.point, Quaternion.identity);
                    Destroy(explosion, 0.5f);

                    // Play explosion sound
                    if (GameManager.instance.explosionSound != null)
                    {
                        AudioSource.PlayClipAtPoint(GameManager.instance.explosionSound, hit.point);
                    }

                    string asteroidName = asteroid.gameObject.name;

                    // Spawn random item with chance
                    if (itemPrefabs.Length > 0 && Random.value <= itemSpawnChance)
                    {
                        GameObject itemToSpawn = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
                        Vector2 randomOffset = new Vector2(
                            Random.Range(-spawnOffsetRange.x, spawnOffsetRange.x),
                            Random.Range(-spawnOffsetRange.y, spawnOffsetRange.y)
                        );
                        Vector2 spawnPos2D = hit.point + randomOffset;
                        Instantiate(itemToSpawn, new Vector3(spawnPos2D.x, spawnPos2D.y, 0f), Quaternion.identity);

                    }

                    // Create smaller asteroids and spawn stars like MissileController
                    if (asteroidName.Contains("Asteroid") || asteroidName.Contains("AsterHuge2"))
                    {
                        CreateSmallAsteroids(hit.point);

                        if (Random.Range(0f, 1f) <= 0.4f && GameManager.instance.starItemPrefab != null)
                        {
                            Instantiate(GameManager.instance.starItemPrefab, hit.point, Quaternion.identity);
                        }
                    }
                    else
                    {
                        if (Random.Range(0f, 1f) <= 0.5f && GameManager.instance.starItemPrefab != null)
                        {
                            Instantiate(GameManager.instance.starItemPrefab, hit.point, Quaternion.identity);
                        }
                    }

                    Destroy(asteroid.gameObject);
                }

                if (rb != null)
                {
                    float mass = rb.mass;
                    float adjustedPushForce = (mass > 0f) ? pushForce / mass : pushForce;
                    rb.AddForce(direction.normalized * adjustedPushForce, ForceMode2D.Force);
                }

                laserLength = hit.distance;
            }
        }

        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + direction * laserLength);
        }

        if (laserSpriteRenderer != null)
        {
            float spriteHeight = laserSpriteRenderer.sprite.bounds.size.y;
            Vector3 localScale = transform.localScale;
            localScale.y = laserLength / spriteHeight;
            transform.localScale = localScale;
            transform.localPosition = Vector3.zero;
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
