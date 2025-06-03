using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject[] asteroidPrefabs;
    public float minInstantiateValue;
    public float maxInstantiateValue;
    public float asteroidDestroyTime = 7f;

    [Header("Particle Effects")]
    public GameObject explosion;
    public GameObject muzzleFlash;

    [Header("Panels")]
    public GameObject pauseMenu;
    public GameObject instructionPanel;

    //Level ship
    public GameObject playerPrefab;
    public GameObject playerLevel2Prefab;
    public GameObject playerLevel3Prefab;

    private bool upgradedToLv2 = false;
    private bool upgradedToLv3 = false;
    public int currentPlayerLevel = 1;

    public int asteroidPerSpawn = 1;

    // Change background
    public GameObject backgroundPrefabLv1;
    public GameObject backgroundPrefabLv2;
    public GameObject backgroundPrefabLv3;
    public GameObject backgroundPrefabLv4;
    private GameObject currentBackground;

    public int score = 0;
    public bool isResetting = false;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI highScoreText;

    [Header("Sound Effects")]
    public AudioClip explosionSound;
    private AudioSource audioSource;

    public bool isGameStarted = false;

    [Header("Settings")]
    public int maxHearts = 5;
    public int currentHearts = 5;
    public HeartUI heartUI;

    [Header("Items")]
    public GameObject starItemPrefab;

    public GameObject asterMedium3Prefab;

    public void Awake()
    {
       instance = this;
       audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (heartUI == null)
            heartUI = FindFirstObjectByType<HeartUI>();

        // Hiển thị trái tim ban đầu
        heartUI.UpdateHearts(currentHearts, maxHearts);

        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            SpawnPlayerByLevel();
        }

        pauseMenu.SetActive(false);
        InvokeRepeating("SpawnAsteroid", 1f, 2f);

        if (backgroundPrefabLv1 != null)
        {
            currentBackground = Instantiate(backgroundPrefabLv1, Vector3.zero, Quaternion.identity);
        }

        UpdateHighScoreText();

        // Start the game immediately when GameScene loads
        StartGameButton();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(true);
        }
        if (!upgradedToLv2 && score >= 200)
        {
            upgradedToLv2 = true;
            UpgradePlayer(playerLevel2Prefab);
            ChangeBackground(backgroundPrefabLv2);
            UpdateAsteroidSpawn(2);
        }
        else if (!upgradedToLv3 && score >= 500)
        {
            upgradedToLv3 = true;
            UpgradePlayer(playerLevel3Prefab);
            ChangeBackground(backgroundPrefabLv3);
            UpdateAsteroidSpawn(3);
        }
        else if (score >= 800)
        {
            ChangeBackground(backgroundPrefabLv4);
            UpdateAsteroidSpawn(4);
        }

    }

    //void InstantiateEnemy()
    //{
    //    Vector3 asteroidPos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6.6f);
    //    int randomIndex = Random.Range(0, asteroidPrefabs.Length);
    //    GameObject selectedAsteroid = asteroidPrefabs[randomIndex];
    //    GameObject asteroid = Instantiate(selectedAsteroid, asteroidPos, Quaternion.Euler(0f,0f,180f));
    //    Destroy(asteroid, asteroidDestroyTime);
    //}

    void SpawnPlayerByLevel()
    {
        GameObject playerToSpawn = playerPrefab;

        if (currentPlayerLevel == 2)
            playerToSpawn = playerLevel2Prefab;
        else if (currentPlayerLevel == 3)
            playerToSpawn = playerLevel3Prefab;

        Instantiate(playerToSpawn, new Vector3(0f, -4.2f, 0f), Quaternion.identity);
    }

    public void StartGameButton()
    {
        isGameStarted = true;
        //currentPlayerLevel = 1;
        //upgradedToLv2 = false;
        //upgradedToLv3 = false;
        ResetGameState();
        Time.timeScale = 1f;
    }

    public void PauseGame(bool isPause) 
    {
        if(isPause == true)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {  
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");

        // Check and update high score
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > savedHighScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }

        UpdateHighScoreText();
    }

    void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "High Score: " + savedHighScore.ToString();
        }
    }

    public void ReturnMainMenuGame()
    {
        // Xoá asteroid
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject asteroid in asteroids)
        {
            Destroy(asteroid);
        }

        // Respawn ship nếu nó bị destroy
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null && playerPrefab != null)
        {
            Instantiate(playerPrefab, new Vector3(0f, -4.2f, 0f), Quaternion.identity);
        }
        // Reset mạng
        currentHearts = maxHearts;
        heartUI.UpdateHearts(currentHearts, maxHearts);
        pauseMenu.SetActive(false);
        score = 0;
        UpdateScoreText();
        ChangeBackground(backgroundPrefabLv1);
        UpdateAsteroidSpawn(1);
        Invoke("FinishReset", 0.5f);
        Time.timeScale = 0f;
    }

    public void ResetGameState()
    {
        isResetting = true;
        // Xoá asteroid
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject asteroid in asteroids)
        {
            Destroy(asteroid);
        }
        DestroyAllPlayers();


        currentPlayerLevel = 1;
        upgradedToLv2 = false;
        upgradedToLv3 = false;

        // Reset mạng
        currentHearts = maxHearts;
        heartUI.UpdateHearts(currentHearts, maxHearts);

        // Respawn ship nếu nó bị destroy
        
        SpawnPlayerByLevel();
        

        score = 0;
        UpdateScoreText();
        ChangeBackground(backgroundPrefabLv1);
        UpdateAsteroidSpawn(1);
        Invoke("FinishReset", 0.5f);
        Time.timeScale = 1f;

    }

    private void DestroyAllPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
    }

    void FinishReset()
    {
        isResetting = false;
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // Score
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    //Upgrade
    void UpgradePlayer(GameObject newPlayerRootPrefab)
    {
        GameObject currentPlayerRoot = GameObject.FindGameObjectWithTag("Player");
        Vector3 spawnPosition = new Vector3(0f, -4.2f, 0f);
        if (currentPlayerRoot != null)
        {
            spawnPosition = currentPlayerRoot.transform.position; // lấy vị trí hiện tại của ship
            Destroy(currentPlayerRoot);
        }

        Instantiate(newPlayerRootPrefab, spawnPosition, Quaternion.identity);

        if (newPlayerRootPrefab == playerLevel2Prefab)
            currentPlayerLevel = 2;
        else if (newPlayerRootPrefab == playerLevel3Prefab)
            currentPlayerLevel = 3;
    }

    void ChangeBackground(GameObject newBackgroundPrefab)
    {
        if (newBackgroundPrefab != null)
        {
            if (currentBackground != null)
            {
                Destroy(currentBackground);
            }
            currentBackground = Instantiate(newBackgroundPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    void UpdateAsteroidSpawn(int level)
    {
        CancelInvoke("SpawnAsteroid");
        if (level == 1)
        {
            asteroidPerSpawn = 1;
            InvokeRepeating("SpawnAsteroid", 1f, 2f);  // giống như lúc bắt đầu
        }
        else if (level == 2)
        {
            asteroidPerSpawn = 2;
            InvokeRepeating("SpawnAsteroid", 1f, 1.2f); // nhiều hơn, nhanh hơn
        }
        else if (level == 3)
        {
            asteroidPerSpawn = 3;
            InvokeRepeating("SpawnAsteroid", 1f, 1f); // nhiều hơn nữa, nhanh hơn nữa
        }
    }

    void SpawnAsteroid()
    {
        for (int i = 0; i < asteroidPerSpawn; i++)
        {
            float randomX = Random.Range(minInstantiateValue, maxInstantiateValue);
            Vector3 spawnPos = new Vector3(randomX, 6f, 0f);

            int randomIndex = Random.Range(0, asteroidPrefabs.Length);
            GameObject selectedAsteroid = asteroidPrefabs[randomIndex];
            GameObject asteroid = Instantiate(selectedAsteroid, spawnPos, Quaternion.Euler(0f, 0f, 180f));
            Destroy(asteroid, asteroidDestroyTime);
        }
    }

    public void ShowInstructions()
    {
        instructionPanel.SetActive(true);
    }

    public void HideInstructions()
    {
        instructionPanel.SetActive(false);
    }

    public void LoseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts--;
            heartUI.UpdateHearts(currentHearts, maxHearts);

            if (currentHearts <= 0)
            {
                GameOver();
            }
            else
            {
                // Respawn player mới
                Invoke(nameof(SpawnPlayerByLevel), 0.5f);
            }
        }
    }

    public void GainHeart()
    {
        if (currentHearts < maxHearts)
        {
            currentHearts++;
            heartUI.UpdateHearts(currentHearts, maxHearts);
        }
    }
    public GameObject GetCurrentPlayerPrefab()
    {
        if (currentPlayerLevel == 1) return playerPrefab;
        if (currentPlayerLevel == 2) return playerLevel2Prefab;
        return playerLevel3Prefab;
    }
}
