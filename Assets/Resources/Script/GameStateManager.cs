using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System.Transactions;
using UnityEngine.SceneManagement;

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

    [Header("场景管理")]
    [SerializeField] private SceneController _sceneController;

    [Header("玩家相关")]
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

    [SerializeField] private GameObject _pausePanel;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        _dieMaterial.SetFloat("_Strength", 1.0f);
    }

    private void Start()
    {
        _weaponDieMaterial = _weaponSR.material;
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
        _pausePanel.SetActive(true);
        Time.timeScale = 0f;
        CurrentState = GameState.Paused;
    }
    private void ResumeGame()
    {
        _pausePanel.SetActive(false);
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
    }

    private async Task HandleGameOver()
    {
        FreezePlayer();
        _playerSR.GetComponent<Renderer>().material = _dieMaterial;

        var playerTween = _dieMaterial.DOFloat(-1, "_Strength", 3.0f);
        var weaponTween = _weaponDieMaterial.DOFloat(-1, "_Strength", 3.0f);

        _dieParticle.Play();
        _weaponDieParticle.Play();

        await playerTween.AsyncWaitForCompletion();
        await weaponTween.AsyncWaitForCompletion();

        GameProgress.LastLevelIndex = SceneManager.GetActiveScene().buildIndex;
        await _sceneController.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }
    private async Task HandleVictory()
    {
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
}
