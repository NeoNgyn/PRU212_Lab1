using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;

    [Header("Shield")]
    public GameObject shieldPrefab;
    public float shieldDuration = 3f;

    [Header("Missile")]
    public GameObject missile;
    public Transform missileSpawnPosition;
    public float destroyTime = 4f;

    public Transform muzzleSpawnPosition;

    [Header("Sound")]
    public AudioClip laserSound;
    private AudioSource audioSource;

    public Transform playerTransform;
    public Transform playerSprite;
    float shipBoundaryRadius = 0.5f;

    [Header("Multi Missile Spawn Points")]
    public Transform missileSpawnPositionLeft;
    public Transform missileSpawnPositionRight;

    private bool isDead = false;

    public GameObject laserBeamPrefab;
    private GameObject laserInstance;
    private LaserBeam laserBeamScript;

    public Transform laserSpawnPosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SpawnShield();
    }

    void Update()
    {

        if (GameManager.instance == null || !GameManager.instance.isGameStarted) return;

        PlayerMovement();
        HandleShooting();
        UpdateLaserPosition();
    }

    void PlayerMovement()
    {
        float xPos = Input.GetAxis("Horizontal");
        float yPos = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(xPos, yPos, 0) * speed * Time.deltaTime;
        transform.Translate(movement);

        // Lấy vị trí hiện tại
        Vector3 pos = transform.position;

        // Giới hạn vùng camera (bottom-left & top-right)
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // Lấy kích thước thực tế của sprite
        float halfWidth = 0.5f;
        float halfHeight = 0.5f;

        SpriteRenderer sr = playerSprite.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            halfWidth = sr.bounds.extents.x;
            halfHeight = sr.bounds.extents.y;
        }

        // Clamp lại vị trí
        pos.x = Mathf.Clamp(pos.x, bottomLeft.x + halfWidth + shipBoundaryRadius, topRight.x - halfWidth - shipBoundaryRadius);
        pos.y = Mathf.Clamp(pos.y, bottomLeft.y + halfHeight + shipBoundaryRadius, topRight.y - halfHeight - shipBoundaryRadius);

        transform.position = pos;
    }

    void HandleShooting()
    {
        int level = GameManager.instance.currentPlayerLevel;

        if (level < 2)
    {
            if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnMissile();
            SpawnMuzzleFlash();
                if (laserSound != null) audioSource.PlayOneShot(laserSound);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (laserInstance == null)
                {
                    // Use laserSpawnPosition here instead of missileSpawnPosition
                    laserInstance = Instantiate(laserBeamPrefab, laserSpawnPosition.position, laserSpawnPosition.rotation, transform);
                    laserBeamScript = laserInstance.GetComponent<LaserBeam>();
                    laserInstance.SetActive(true);
                }
                else if (!laserInstance.activeInHierarchy)
                {
                    laserInstance.SetActive(true);
                }

                laserBeamScript.SetFiring(true);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                if (laserBeamScript != null && laserInstance.activeInHierarchy)
                {
                    laserBeamScript.SetFiring(true);
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (laserBeamScript != null)
                {
                    laserBeamScript.SetFiring(false);
                    laserInstance.SetActive(false);
            }
        }
    }
    }

    void UpdateLaserPosition()
    {
        if (laserInstance != null && GameManager.instance.currentPlayerLevel >= 2)
        {
            // Update position and rotation based on laserSpawnPosition
            laserInstance.transform.position = laserSpawnPosition.position;
            laserInstance.transform.rotation = laserSpawnPosition.rotation;
        }
    }

    void SpawnMissile()
    {
        GameObject gm = Instantiate(missile, missileSpawnPosition);
        gm.transform.SetParent(null);
        Destroy(gm, destroyTime);

        if (GameManager.instance.currentPlayerLevel >= 3)
        {
            if (missileSpawnPositionLeft != null)
            {
                Quaternion leftRotation = missileSpawnPosition.rotation * Quaternion.Euler(0, 0, -15f);
                GameObject gmLeft = Instantiate(missile, missileSpawnPositionLeft.position, leftRotation);
                gmLeft.transform.SetParent(null);
                Destroy(gmLeft, destroyTime);
            }

            if (missileSpawnPositionRight != null)
            {
                Quaternion rightRotation = missileSpawnPosition.rotation * Quaternion.Euler(0, 0, 15f);
                GameObject gmRight = Instantiate(missile, missileSpawnPositionRight.position, rightRotation);
                gmRight.transform.SetParent(null);
                Destroy(gmRight, destroyTime);
            }
        }
    }

    void SpawnMuzzleFlash()
    {
        GameObject muzzle = Instantiate(GameManager.instance.muzzleFlash, muzzleSpawnPosition);
        muzzle.transform.SetParent(null);
        Destroy(muzzle, 0.2f);
    }

    private bool isDead = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Asteroid"))
        {
            isDead = true;

            // Disable laser before disabling player to avoid coroutine on inactive object
            if (laserBeamScript != null)
            {
                laserBeamScript.SetFiring(false);
                if (laserInstance != null)
                    laserInstance.SetActive(false);
            }

            Vector3 centerPos = transform.position;
            GameObject explosion = Instantiate(GameManager.instance.explosion, centerPos, Quaternion.identity);
            Destroy(explosion, 0.1f);

            GameManager.instance.LoseHeart();
            gameObject.SetActive(false);
        }
        
        if (collision.gameObject.name.Contains("Heart"))
        {
            GameManager.instance.GainHeart();
        }
    }
   
    public void SpawnShield()
    {
        if (shieldPrefab != null)
        {
            Debug.Log("Spawning shield...");
            GameObject shield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            shield.transform.SetParent(transform);
            shield.transform.localPosition = Vector3.zero;
            Destroy(shield, shieldDuration);
        }
        else
        {
            Debug.LogWarning("Shield Prefab is not assigned!");
        }
    }

    private void OnDisable()
    {
        if (laserBeamScript != null)
        {
            laserBeamScript.SetFiring(false);
        }
    }

}
