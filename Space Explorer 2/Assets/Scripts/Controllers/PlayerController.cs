//using UnityEditor.Rendering.LookDev;
using UnityEngine;
//using UnityEditor.Rendering;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;

    [Header("Missile")]
    public GameObject missile;
    public Transform missileSpawnPosition;
    public float destroyTime = 4f;

    public Transform muzzleSpawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Sound")]
    public AudioClip laserSound;
    private AudioSource audioSource;

    public Transform playerTransform;

    public Transform playerSprite; // Gán thủ công đối tượng "Player" trong Inspector
    float shipBoundaryRadius = 0.5f;

    [Header("Multi Missile Spawn Points")]
    public Transform missileSpawnPositionLeft;
    public Transform missileSpawnPositionRight;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {

        if (GameManager.instance == null || !GameManager.instance.isGameStarted) return;
        PlayerMovement();
        PlayShoot();
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

    void PlayShoot()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnMissile();
            SpawnMuzzleFlash();

            if (laserSound != null)
            {
                audioSource.PlayOneShot(laserSound);
            }
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

        if (collision.gameObject.tag == "Asteroid")
        {
            isDead = true;
            GameObject gm = Instantiate(GameManager.instance.explosion, playerTransform.position, playerTransform.rotation);
            Destroy(gm, 0.1f);
            GameManager.instance.LoseHeart(); // Dùng GameManager quản lý mạng

            // Để cho GameManager lo việc spawn player mới
            gameObject.SetActive(false);
        }
        
        if(collision.gameObject.name.Contains("Heart"))
        {
            GameManager.instance.GainHeart();
        }
    }
   
}
