using UnityEngine;

public class StarItem : MonoBehaviour
{
    public float fallSpeed = 1f;
    public float lifetime = 3f; 

    private void Start()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        Destroy(gameObject, lifetime);
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
