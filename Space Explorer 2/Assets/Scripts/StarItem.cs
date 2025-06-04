using UnityEngine;

public class StarItem : MonoBehaviour
{
    private float fallSpeed = 1f;
    [SerializeField] private float rotationSpeed = 180f;

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.AddScore(50);
            Destroy(this.gameObject);
        }
    }
}
