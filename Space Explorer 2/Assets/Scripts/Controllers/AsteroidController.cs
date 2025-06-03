using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float speed;
    public float rotationSpeed = 60f;
    private Vector3 moveDirection;

    
    void Start()
    {
        rotationSpeed = Random.Range(-150f, 150f);

        // Góc lệch random từ -30° đến +30° so với hướng "down"
        float angleOffset = Random.Range(-30f, 30f);

        // Tạo hướng "down" ban đầu (Vector2.down) rồi xoay góc lệch
        moveDirection = Quaternion.Euler(0, 0, angleOffset) * Vector3.down;
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        // Xoay sprite quanh trục Z (quay hình ảnh)
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
