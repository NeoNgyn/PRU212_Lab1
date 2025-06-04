using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound; 

    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true; 
        audioSource.clip = collectSound;
        audioSource.volume = 0.3f;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
    }
}
