using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer instance;
    public float _time;
    [SerializeField] private TextMeshProUGUI _text;

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
        }
    }
    private void Update()
    {
        
        if (PlayerHP.instance._HP <= 0) return;

        _time -= Time.deltaTime;
        _time = Mathf.Max(0f, _time);

        _text.text = "Time: " + _time.ToString("F2");

        if (_time <= 0f)
        {
            GameStateManager.instance.SetState(GameState.Victory);
        }
    }
    public void ResetTimer(float startTime)
    {
        _time = startTime;
    }
}
