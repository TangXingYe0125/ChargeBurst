using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    [SerializeField] private Image _frontBar; 
    [SerializeField] private Image _backBar; 
    [SerializeField] private float _delaySpeed = 0.5f; 

    private float _currentHP;
    [SerializeField]private float _maxHP;

    void Start()
    {
        _currentHP = _maxHP;
        UpdateBars();
    }

    public void TakeDamage(float damage)
    {
        _currentHP -= damage;
        _currentHP = Mathf.Clamp(_currentHP, 0, _maxHP);
        _frontBar.fillAmount = _currentHP / _maxHP; 
        StopAllCoroutines();
        StartCoroutine(UpdateBackBar());
    }

    private IEnumerator UpdateBackBar()
    {
        yield return new WaitForSeconds(0.2f); 
        while (_backBar.fillAmount > _frontBar.fillAmount)
        {
            _backBar.fillAmount = Mathf.Lerp(_backBar.fillAmount, _frontBar.fillAmount, Time.deltaTime / _delaySpeed);
            yield return null;
        }
        _backBar.fillAmount = _frontBar.fillAmount;
    }

    private void UpdateBars()
    {
        _frontBar.fillAmount = _currentHP / _maxHP;
        _backBar.fillAmount = _frontBar.fillAmount;
    }
}
