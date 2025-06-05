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
    private bool upgradedToLv4 = false;
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
    public TMPro.TextMeshProUGUI timeText;
    //public TMPro.TextMeshProUGUI highScoreText;

    [Header("Sound Effects")]
    public AudioClip explosionSound;
    private AudioSource audioSource;
    public AudioClip upgradeSound;

    public bool isGameStarted = false;

    [Header("Settings")]
    public int maxHearts = 5;
    public int currentHearts = 5;
    public HeartUI heartUI;

    [Header("Items")]
    public GameObject starItemPrefab;

    public GameObject asterMedium3Prefab;

    public GameObject upgradeEffectPrefab;

    private float currentTime = 0f;
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
        UpdateTime();
        //UpdateHighScoreText();

        // Start the game immediately when GameScene loads
        StartGameButton();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(true);
        }

        // Đảm bảo game không bị tạm dừng và GameStateManager tồn tại
        if (!GameStateManager.instance.IsPaused()) // Hoặc !isPaused nếu GameManager cũng có isPaused riêng
        {
            currentTime += Time.deltaTime;
            GameStateManager.instance.AddTime(Time.deltaTime); // Cập nhật thời gian đã trôi qua

            if (currentTime >= 1f)
            {
                UpdateTime();
                currentTime -= 1f; // Subtract 1 second to keep the remainder for the next update
            }
        }
        // Luôn cập nhật điểm từ GameManager vào GameStateManager (hoặc chỉ cập nhật khi điểm thay đổi)
        // Đây là cách đơn giản nhất để đảm bảo GameStateManager luôn có điểm số mới nhất.
        GameStateManager.instance.AddScore(this.score - GameStateManager.instance.GetScore()); // Đồng bộ điểm, chỉ thêm phần chênh lệch
                                                                                               // Hoặc đơn giản hơn:
                                                                                               // GameStateManager.instance.SetScore(this.score); // Cần tạo hàm SetScore trong GameStateManager nếu chưa có
                                                                                               // ... nhưng cách này không phù hợp với cấu trúc GameStateManager hiện tại lắm.
                                                                                               // Tốt nhất là GameStateManager tự quản lý điểm và thời gian, GameManager chỉ gọi AddScore/AddTime của GameStateManager.
                                                                                               // HÃY SỬA ĐỔI CÁCH GỌI AddScore ở Dưới đây.

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
        else if (!upgradedToLv4 && score >= 800)
        {
            upgradedToLv4 = true;
            ChangeBackground(backgroundPrefabLv4);
            UpdateAsteroidSpawn(4);
        }

    }

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
        // GameStateManager.instance.ResetGameState(); // XÓA DÒNG NÀY
        Time.timeScale = 1f;
    }

    public void PauseGame(bool isPause)
    {
        if (isPause == true)
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

        // --- XÓA TOÀN BỘ PHẦN NÀY, LeaderboardManager sẽ lo việc lưu điểm ---
        // int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        // if (score > savedHighScore)
        // {
        //     PlayerPrefs.SetInt("HighScore", score);
        //     PlayerPrefs.Save();
        // }
        // UpdateHighScoreText();
        // --- KẾT THÚC PHẦN XÓA ---
    }

    //void UpdateHighScoreText()
    //{
    //    if (highScoreText != null)
    //    {
    //        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
    //        highScoreText.text = "High Score: " + savedHighScore.ToString();
    //    }
    //}

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
        currentTime = 0f; 
        if (timeText != null) timeText.text = "Time: 00:00"; 
        ChangeBackground(backgroundPrefabLv1);
        UpdateAsteroidSpawn(1);
        upgradedToLv2 = false;
        upgradedToLv3 = false;
        upgradedToLv4 = false;
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
        upgradedToLv4 = false;

        // Reset mạng (vẫn giữ nếu heartUI là riêng của GameManager)
        currentHearts = maxHearts;
        if (heartUI != null) heartUI.UpdateHearts(currentHearts, maxHearts); // Kiểm tra null trước khi gọi

        // Respawn ship
        SpawnPlayerByLevel();

        // score = 0; // XÓA DÒNG NÀY (GameStateManager sẽ reset)
        UpdateScoreText(); // Cập nhật hiển thị score của GameManager
        currentTime = 0f;
        if (timeText != null) timeText.text = "Time: 00:00";
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
        // 1. Đảm bảo game không bị tạm dừng khi chuyển Scene
        Time.timeScale = 1f;

        // 2. Lưu điểm của người chơi vào Leaderboard
        SaveCurrentGameResultToLeaderboard();

        // 3. Reset GameStateManager để chuẩn bị cho lượt chơi mới
        if (GameStateManager.instance != null)
        {
            GameStateManager.instance.ResetGameState();
        }

        // 4. Tải lại StartMenuScene (Main Menu)
        SceneManager.LoadScene("StartMenuScene");

        // --- XÓA HOẶC BÌNH LUẬN DÒNG THOÁT ỨNG DỤNG NẾU BẠN KHÔNG MUỐN THOÁT HẲN ---
        // Application.Quit();
        // #if UNITY_EDITOR
        // UnityEditor.EditorApplication.isPlaying = false;
        // #endif

    }

    // Score
    public void AddScore(int amount)
    {
        // Đảm bảo GameStateManager.instance tồn tại trước khi gọi.
        // Điều này quan trọng vì GameStateManager là nơi lưu điểm cuối cùng.
        if (GameStateManager.instance != null)
        {
            GameStateManager.instance.AddScore(amount);
        }
        else
        {
            Debug.LogWarning("GameManager: GameStateManager.instance is null! Cannot add score to global state.");
        }

        // Cập nhật biến score cục bộ của GameManager để logic nâng cấp level vẫn đúng
        score += amount;

        UpdateScoreText(); // Vẫn cập nhật UI của GameManager
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    void UpdateTime()
    {
        if (timeText != null && scoreText != null && SceneManager.GetActiveScene().name == "GameScene")
        {
            string minutes = Mathf.Floor(GameStateManager.instance.GetTimeElapsed() / 60).ToString("00");
            string seconds = (GameStateManager.instance.GetTimeElapsed() % 60).ToString("00");
            timeText.text = "Time: " + minutes + ":" + seconds;
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
        if (upgradeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(upgradeSound, 1f);
        }
        
        GameObject newPlayer = Instantiate(newPlayerRootPrefab, spawnPosition, Quaternion.identity);
        SpawnUpgradeEffect(newPlayer);
        //if (upgradeSound != null && audioSource != null)
        //{
        //    audioSource.PlayOneShot(upgradeSound, 1f);
        //}

        if (newPlayerRootPrefab == playerLevel2Prefab)
            currentPlayerLevel = 2;
        else if (newPlayerRootPrefab == playerLevel3Prefab)
            currentPlayerLevel = 3;
    }

    void SpawnUpgradeEffect(GameObject playerObject)
    {
        if (upgradeEffectPrefab != null && playerObject != null)
        {
            GameObject effect = Instantiate(upgradeEffectPrefab, playerObject.transform.position, Quaternion.identity);

            // Gắn effect vào ship như là child object
            effect.transform.SetParent(playerObject.transform);

            // Đặt vị trí local relative to ship (có thể điều chỉnh offset nếu cần)
            effect.transform.localPosition = Vector3.zero;

            // Tự động destroy effect sau 0.5s
            Destroy(effect, 0.5f);
        }
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
        else if (level == 4)
        {
            asteroidPerSpawn = 4;
            InvokeRepeating("SpawnAsteroid", 1f, 0.8f); // nhiều hơn nữa, nhanh hơn nữa
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

    // --- THÊM HÀM NÀY VÀO GameManager.cs ---
    private void SaveCurrentGameResultToLeaderboard()
    {
        // Debug.Log này sẽ giúp bạn xác nhận hàm có được gọi không

        if (GameStateManager.instance == null)
        {
            return;
        }
        if (LeaderboardManager.instance == null)
        {
            return;
        }

        // Lấy dữ liệu tên, điểm, thời gian từ GameStateManager
        string pName = GameStateManager.instance.CurrentPlayerName;
        int pScore = GameStateManager.instance.GetScore();
        float pTime = GameStateManager.instance.GetTimeElapsed();

        // Thêm debug log để xem dữ liệu thực tế trước khi lưu
        // Tạo một đối tượng PlayerData mới
        PlayerData newEntry = new PlayerData
        {
            playerName = pName,
            score = pScore,
            timePlayed = pTime
        };

        // Thêm entry mới này vào LeaderboardManager để lưu
        LeaderboardManager.instance.AddEntry(newEntry);
    }

}