using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System.Transactions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Playing,
    Paused,
    GameOver,
    Victory
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    [SerializeField] private SceneController _sceneController;

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private DamageDetecter _damageDetecter;
    [SerializeField] private Attack _attack;

    [SerializeField] private SpriteRenderer _playerSR;
    [SerializeField] private SpriteRenderer _weaponSR;
    [SerializeField] private Material _dieMaterial;
    [SerializeField] private ParticleSystem _dieParticle;
    private Material _weaponDieMaterial;
    [SerializeField] private ParticleSystem _weaponDieParticle;
    [SerializeField] private Animator _playerAnimator;

    public GameState CurrentState { get; private set; } = GameState.Playing;

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _volumeControl;
    [SerializeField] private GameObject _menu;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        _dieMaterial.SetFloat("_Dissolve", 0.0f);
    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void Start()
    {
        _weaponDieMaterial = _weaponSR.material;
        _pauseMenu.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (CurrentState == GameState.Paused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    public async void SetState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Playing:
                ResumeGame();
                break;

            case GameState.Paused:
                PauseGame();
                break;

            case GameState.GameOver:
                await HandleGameOver();
                break;

            case GameState.Victory:
                await HandleVictory();
                break;
        }
    }

    private void PauseGame()
    {
        _pauseMenu.SetActive(true);
        _menu.SetActive(true);
        _volumeControl.SetActive(false);

        foreach (Button btn in _pauseMenu.GetComponentsInChildren<Button>(true))
        {
            btn.transform.DOKill();
            btn.transform.localScale = Vector3.one;
        }
        Time.timeScale = 0f;
        CurrentState = GameState.Paused;
    }
    private void ResumeGame()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
    }

    private async Task HandleGameOver()
    {
        Kills.instance.GetAmount();
        FreezePlayer();
        _playerSR.GetComponent<Renderer>().material = _dieMaterial;

        var playerTween = _dieMaterial.DOFloat(1, "_Dissolve", 3.0f);
        var weaponTween = _weaponDieMaterial.DOFloat(1, "_Dissolve", 3.0f);

        _dieParticle.Play();
        _weaponDieParticle.Play();

        await playerTween.AsyncWaitForCompletion();
        await weaponTween.AsyncWaitForCompletion();

        GameProgress.LastLevelIndex = SceneManager.GetActiveScene().buildIndex;
        await _sceneController.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }
    private async Task HandleVictory()
    {
        Kills.instance.GetAmount();
        FreezePlayer();
        _playerAnimator.SetTrigger("isStageClear");

        await Task.Delay(1000);// wait for 1.0f

        await _sceneController.LoadScene(_sceneController._currentSceneNum + 1);
    }
    private void FreezePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject playerBody = GameObject.FindGameObjectWithTag("PlayerBody");

        var rb = player.GetComponent<Rigidbody2D>();
        var rbBody = playerBody.GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;
        if (rbBody) rbBody.simulated = false;

        var col = player.GetComponent<Collider2D>();
        var colBody = playerBody.GetComponent<Collider2D>();
        if (col) col.enabled = false;
        if (colBody) colBody.enabled = false;

        _playerMovement.enabled = false;
        _damageDetecter.enabled = false;
        _attack.enabled = false;
    }
    public async void PauseMenuBackToTitle()
    {
        if (EventTrigger._isEventTriggerReady) return;
        await _sceneController.LoadScene(0);
    }
}
