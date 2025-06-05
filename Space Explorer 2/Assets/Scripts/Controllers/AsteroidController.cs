using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float speed;
    public float rotationSpeed = 60f;
    private Vector3 moveDirection;

    [Header("Health")]
    public int health = 3;

    [Header("Physics")]
    public float mass = 1f; // You can tweak this in Inspector

    private Rigidbody2D rb;

    private float accumulatedDamage = 0f;

    void Start()
    {
        rotationSpeed = Random.Range(-150f, 150f);

        float angleOffset = Random.Range(-30f, 30f);
        moveDirection = Quaternion.Euler(0, 0, angleOffset) * Vector3.down;

        if (!gameObject.name.Contains("Asteroid") && !gameObject.name.Contains("AsterHuge2"))
        {
            health = 1;
        }
        else
        {
            health = 2;
        }

        int playerLevel = GameManager.instance.currentPlayerLevel;
        if (playerLevel == 2)
        {
            health += 1;
        }
        else if (playerLevel == 3)
        {
            health += 2;
        }

        // Setup Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.mass = mass;
        rb.gravityScale = 0;          // no gravity for asteroids in space
        rb.linearDamping = 0.5f;      // replaces rb.drag
        rb.angularDamping = 0.5f;     // replaces rb.angularDrag
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // or allow rotation if you want
    }

    void Update()
    {
        // Use Rigidbody2D to move for smooth physics
        if (rb != null)
        {
            rb.MovePosition(rb.position + (Vector2)(moveDirection * speed * Time.deltaTime));
            rb.MoveRotation(rb.rotation + rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += moveDirection * speed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    public bool TakeDamage(float damage)
    {
        accumulatedDamage += damage;

        while (accumulatedDamage >= 1f)
        {
            health -= 1;
            accumulatedDamage -= 1f;

            if (health <= 0)
            {
                return true;
            }
        }

        return false;
    }
}
