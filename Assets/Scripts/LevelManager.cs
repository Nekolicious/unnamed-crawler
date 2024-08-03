using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField]
    private VoidEventChannelSO startTrigger;

    [SerializeField]
    private VoidEventChannelSO dungeonGeneratingFinish;

    [SerializeField]
    private VoidEventChannelSO playerDead;

    [SerializeField]
    private GameObject loadingScreen;

    [SerializeField]
    private GameOver gameOverScreen;

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private InputActionReference pause;

    [SerializeField]
    private ConfirmationWindow confirmationScreen;

    public static bool isPaused = false;
    private static bool isLoading = false;

    public List<AudioClip> menuBGM;
    public List<AudioClip> gameBGM;

    [SerializeField]
    private AudioSource bgmPlayer;

    private VisualElement root;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            PlayBGM();
            DontDestroyOnLoad(gameObject);
        }
    }

    private Scene GetCurrentScene()
    {
        var currentScene = SceneManager.GetActiveScene();
        return currentScene;
    }

    private void OnEnable()
    {
        dungeonGeneratingFinish.onRaiseEvent += SetLoadingScreenOff;
        playerDead.onRaiseEvent += GameOver;
        pause.action.performed += PauseControl;
    }
    private void OnDisable()
    {
        dungeonGeneratingFinish.onRaiseEvent -= SetLoadingScreenOff;
        playerDead.onRaiseEvent -= GameOver;
        pause.action.performed -= PauseControl;
    }

    public void StartGame()
    {
        ResumeGame();
        var dgParam = GlobalStats.instance;
        dgParam.Reset();
        StartCoroutine(LoadSceneAsync());   
    }

    public void NextStage()
    {
        var dgParam = GlobalStats.instance;
        dgParam.dungeonHeight += (int)(dgParam.dungeonHeight * 0.1);
        dgParam.dungeonWidth += (int)(dgParam.dungeonWidth * 0.1);
        dgParam.level++;
        dgParam.keyDropChance = 0;
        dgParam.keyIsDropped = false;
        dgParam.dungeonChestInMap = 0;
        StartNextStage();
    }

    public void StartNextStage()
    {
        ResumeGame();
        StartCoroutine(LoadSceneAsync());
    }

    public void BackToMenu()
    {
        confirmationScreen.gameObject.SetActive(true);
        confirmationScreen.Verify("Menu");
    }

    public void CloseConfirmWindow()
    {
        confirmationScreen.gameObject.SetActive(false);
    }

    public void Menu()
    {
        StartCoroutine(BackToMenuAsync());
    }
    IEnumerator BackToMenuAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainMenu");

        ResumeGame();
        StopBGM();
        loadingScreen.SetActive(true);
        isLoading = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        isLoading = false;
        SetLoadingScreenOff();
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("DungeonScene");
        NowLoading();

        while (!operation.isDone)
        {
            yield return null;
        }

        isLoading = false;
        startTrigger?.RaiseEvent();
    }

    private void NowLoading()
    {
        StopBGM();
        loadingScreen.SetActive(true);
        isLoading = true;
    }

    private void SetLoadingScreenOff()
    {
        loadingScreen.SetActive(false);
        PlayBGM();
    }

    private void PauseControl(InputAction.CallbackContext obj)
    {
        if (GetCurrentScene().name.Equals("DungeonScene") && !isLoading)
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        AudioListener.pause = false;
        isPaused = false;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        Pause.ShowPause();
        AudioListener.pause = true;
        isPaused = true;
    }

    public void UnstuckPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = GlobalStats.instance.playerSpawnPoint;
        ResumeGame();
    }

    private void GameOver()
    {
        GlobalStats.instance.CheckHighScore();
        Time.timeScale = 0f;
        AudioListener.pause = true;
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.GameOverWindow();
    }

    public void CloseGameOverWindow()
    {
        gameOverScreen.gameObject.SetActive(false);
    }

    private void PlayBGM()
    {
        List<AudioClip> bgm;

        switch (GetCurrentScene().name)
        {
            case "DungeonScene":
                bgm = gameBGM;
                break;
            case "MainMenu":
                bgm = menuBGM;
                break;
            default:
                bgm = menuBGM;
                break;
        }
        bgmPlayer.clip = bgm[Random.Range(0, bgm.Count)];
        bgmPlayer.Play();
    }

    private void StopBGM()
    {
        bgmPlayer.Stop();
    }
}
