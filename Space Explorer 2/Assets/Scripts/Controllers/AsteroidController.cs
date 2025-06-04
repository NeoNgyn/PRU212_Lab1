using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float speed;
    public float rotationSpeed = 60f;
    private Vector3 moveDirection;

    [Header("Health")]
    public int health = 3;
    void Start()
    {
        rotationSpeed = Random.Range(-150f, 150f);

        // Góc lệch random từ -30° đến +30° so với hướng "down"
        float angleOffset = Random.Range(-30f, 30f);

        // Tạo hướng "down" ban đầu (Vector2.down) rồi xoay góc lệch
        moveDirection = Quaternion.Euler(0, 0, angleOffset) * Vector3.down;

        // Mặc định health
        if (!gameObject.name.Contains("Asteroid") && !gameObject.name.Contains("AsterHuge2"))
        {
            health = 1;
        }
        else
        {
            health = 2;
        }

        // Tăng máu theo Level hiện tại
        int playerLevel = GameManager.instance.currentPlayerLevel;
        if (playerLevel == 2)
        {
            health += 1;
        }
        else if (playerLevel == 3)
        {
            health += 2;
        }
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        // Xoay sprite quanh trục Z (quay hình ảnh)
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
    }

    // Hàm nhận sát thương
    public bool TakeDamage(int damage)
    {
        health -= damage;
        return health <= 0; // Trả về true nếu hết máu
    }
}
