using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private float speed = 2f; 
    [SerializeField] private float rotationSpeed = 180f; 
    [SerializeField] private AudioClip collectSound; 

    private AudioSource audioSource; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {

        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        { 
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
                //Detach AudioSource to let the sound finish playing after destruction
                //Keep the items visible and the sound on
                audioSource.transform.SetParent(null);
                DontDestroyOnLoad(audioSource.gameObject);
                Destroy(audioSource.gameObject, collectSound.length);
            }
            //Destroy(gameObject);
        }
    }
}