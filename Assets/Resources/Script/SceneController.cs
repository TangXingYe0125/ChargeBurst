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
        _sceneChangeMaterial.SetFloat("_Slider", 0.0f);
        //_ = BeginScene();
        _isLoading = false;
        _currentSceneNum = SceneManager.GetActiveScene().buildIndex;
        Time.timeScale = 1.0f;
    }
    public async Task BeginScene()
    {
        EventTrigger._isEventTriggerReady = true;
        Physics2D.IgnoreLayerCollision(9, 10, false);
        await _sceneChangeMaterial.DOFloat(0, "_Slider", 2.0f).SetUpdate(true).AsyncWaitForCompletion();
        EventTrigger._isEventTriggerReady = false;
    }
    public async Task LoadScene(int _SceneNum)
    {
        EventTrigger._isEventTriggerReady = true;
        if (_isLoading) return;

        _isLoading = true;

        await _sceneChangeMaterial.DOFloat(1, "_Slider", 2f).SetUpdate(true).AsyncWaitForCompletion();

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(_SceneNum);
        loadOp.allowSceneActivation = true;
        while (!loadOp.isDone)
            await Task.Yield();

        SceneController newSceneCtrl = FindObjectOfType<SceneController>();
        if (newSceneCtrl != null)
        {
            newSceneCtrl._sceneChangeMaterial.SetFloat("_Slider", 1.0f);
            await newSceneCtrl.BeginScene();
        }

        EventTrigger._isEventTriggerReady = false;
        _isLoading = false;
    }

    public async void BackToTitle()
    {
        if(EventTrigger._isEventTriggerReady) return;
        Kills.instance.ResetKills();
        await LoadScene(0);
    }
    public async void TryAgain()
    {
        if (EventTrigger._isEventTriggerReady) return;
        Kills.instance.ResetKills();
        await LoadScene(GameProgress.LastLevelIndex);
    }
    public async void NextStage()
    {
        if (EventTrigger._isEventTriggerReady) return;
        Kills.instance.ResetKills();
        await LoadScene(_currentSceneNum + 1);
    }
}
