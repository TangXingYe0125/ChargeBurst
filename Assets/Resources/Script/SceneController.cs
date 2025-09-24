using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Transactions;

public class SceneController : MonoBehaviour
{
    private Material _sceneChangeMaterial;
    [SerializeField] private RawImage _targetImage;
    private bool _isLoading;
    public int _currentSceneNum;
    private void Awake()
    {
        _sceneChangeMaterial = _targetImage.material;
        _sceneChangeMaterial.SetFloat("_Slider", 1.0f);
        _ = BeginScene();
        _isLoading = false;
        _currentSceneNum = SceneManager.GetActiveScene().buildIndex;
    }

    public async Task BeginScene()
    {
        Debug.Log("开始初期加载");
        EventTrigger._isEventTriggerReady = true;
        await _sceneChangeMaterial.DOFloat(0, "_Slider", 2.0f).AsyncWaitForCompletion();
        EventTrigger._isEventTriggerReady = false;
        Debug.Log("初期加载完成");
    }

    public async Task LoadScene(int _SceneNum)
    {
        EventTrigger._isEventTriggerReady = true;
        if (_isLoading) return;

        _isLoading = true;
        Debug.Log("开始加载新场景");
        await _sceneChangeMaterial.DOFloat(1, "_Slider", 2.0f).AsyncWaitForCompletion();

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(_SceneNum);
        loadOp.allowSceneActivation = true;
        while (!loadOp.isDone)
        {
            await Task.Yield();
        }
        Debug.Log("新场景加载完成");
        EventTrigger._isEventTriggerReady = false;
    }

    public async void BackToTitle()
    {
        if(EventTrigger._isEventTriggerReady) return;
        await LoadScene(0);
    }
    public async void TryAgain()
    {
        if (EventTrigger._isEventTriggerReady) return;
        await LoadScene(GameProgress.LastLevelIndex);
    }
    public async void NextStage()
    {
        if (EventTrigger._isEventTriggerReady) return;
        await LoadScene(_currentSceneNum + 1);
    }
}
