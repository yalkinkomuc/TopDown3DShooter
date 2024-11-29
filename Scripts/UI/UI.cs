using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection WeaponSelection { get; private set; }
    public GameObject pauseUI;
    public UI_GameOver gameOverUI { get; private set; }
    public UI_Settings settingsUI { get; private set; }

    [SerializeField] private GameObject[] uiElements;

    public GameObject loadingScreenUI;

    public GameObject victoryScreenUI;

    [Header("Fade Image")]
    [SerializeField] private Image fadeImage;


    private void Awake()
    {
        instance = this;
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        WeaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
        gameOverUI = GetComponentInChildren<UI_GameOver>(true);
        settingsUI = GetComponentInChildren<UI_Settings>(true);
    }

    private void Start()
    {
        AssignInputUI();
        StartCoroutine(ChangeImageAlpha(0, 1.5f, null));

        // test için sonra kaldır.
        if (GameManager.instance.quickStart)
        {
            LevelGenerator.instance.InitializieGeneration();
            StartTheGame();
        }


    }

    

    private void ResumeTimeInGameEnabled()
    {
        if (inGameUI.isActiveAndEnabled)
            TimeManager.instance.ResumeTime();

        ControlsManager.instance.SwitchToCharacterControls();
    }
    public void SwitchTo(GameObject uiToSwitchOn)
    {
        foreach (GameObject go in uiElements)
        {
            go.SetActive(false);
        }

        uiToSwitchOn.SetActive(true);
        settingsUI.LoadSettings();

        ResumeTimeInGameEnabled();
    }

    public void ShowGameOverUI(string message = "GAME OVER")
    {
        SwitchTo(gameOverUI.gameObject);
        gameOverUI.ShowGameOverMessage(message);
    }

    public void ShowVictoryScreenUI()
    {
        StartCoroutine(ChangeImageAlpha(1, 1.5f, SwitchToVictoryScreenUI));
    }

    private void SwitchToVictoryScreenUI()
    {
        SwitchTo(victoryScreenUI);

        Color color = fadeImage.color;
        color.a = 0;

        fadeImage.color = color;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Çıkış");
    }
    public void StartTheGame() => StartCoroutine(StartGameSequence());

    public void StartLevelGeneration() => LevelGenerator.instance.InitializieGeneration();

    public void RestartTheGame()
    {
        SwitchTo(loadingScreenUI.gameObject);
        
        StartCoroutine(LoadingScreen(SceneManager.GetActiveScene().name));  
    }

    private IEnumerator LoadingScreen(string sceneName)
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName);

        while (!loadScene.isDone)
        {
            yield return null;
        }

        loadingScreenUI.SetActive(false);

    }

    public void PauseSwitch()
    {
        bool gamePaused = pauseUI.activeSelf;

        if (gamePaused)
        {
            SwitchTo(inGameUI.gameObject);
            ControlsManager.instance.SwitchToCharacterControls();
            TimeManager.instance.ResumeTime();
            Cursor.visible = false;
        }
        else
        {
            SwitchTo(pauseUI);
            ControlsManager.instance.SwitchToUIControls();
            TimeManager.instance.PauseTime();
            Cursor.visible = true;
        }


    }

    private void AssignInputUI()
    {
        PlayerControls controls = GameManager.instance.player.controls;
        controls.UI.UI_Pause.performed += ctx => PauseSwitch();
    }

    private IEnumerator StartGameSequence()
    {
        TimeManager.instance.ResumeTime();

        bool quickStart = GameManager.instance.quickStart;

        if (quickStart == false)
        {
            fadeImage.color = Color.black;
            StartCoroutine(ChangeImageAlpha(1, 1, null));
            yield return new WaitForSeconds(1);

        }

        yield return null;
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();

        if (quickStart)
            StartCoroutine(ChangeImageAlpha(0, .1f, null));
        else
            StartCoroutine(ChangeImageAlpha(0, 1f, null));

        AudioManager.instance.PlayBGM(1);



    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, System.Action onComplete)
    {
        float time = 0;
        Color currentColor = fadeImage.color;
        float startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        onComplete?.Invoke();
    }

    [ContextMenu("AssignAudioToButtons")]
    public void AssignAudioListenersToButtons()
    {
        UI_Button[] buttons = FindObjectsOfType<UI_Button>(true);

        foreach (var button in buttons)
        {
            button.AssignAudioSource();
        }
    }


}
