using UnityEngine;

public class HeartItemController : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GainHeart();
                Debug.Log("Player gained 1 health from heart item!");
            }
        }
    }
}
