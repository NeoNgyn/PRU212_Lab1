using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    [Header("Sound Effects")]
    private AudioSource[] audioSources;
    private int currentAudioSourceIndex = 0;
    private int audioSourcePoolSize = 5;

    private float timeElapsed = 0f;
    private int score = 0;
    private bool isPaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        audioSources = new AudioSource[audioSourcePoolSize];
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].playOnAwake = false;
            audioSources[i].spatialBlend = 0f;
        }
    }

    public void ResetGameState()
    {
        timeElapsed = 0f;
        score = 0;
        isPaused = false;
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }

    public bool IsPaused() => isPaused;

    public float GetTimeElapsed() => timeElapsed;
    public void SetTimeElapsed(float time) => timeElapsed = time;
    public void AddTime(float deltaTime) => timeElapsed += deltaTime;

    public int GetScore() => score;
    public void AddScore(int points) => score += points;

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSources[currentAudioSourceIndex] != null)
        {
            audioSources[currentAudioSourceIndex].PlayOneShot(clip, 1f);
            audioSources[currentAudioSourceIndex].pitch = 0.9f;
            currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSourcePoolSize;
        }
    }
}