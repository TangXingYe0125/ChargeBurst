using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBlade : MonoBehaviour
{
    [SerializeField] private int _swordCount;
    [SerializeField] private float _angle;
    [SerializeField] private float _radius;
    [SerializeField] private float _delayTime;
    [SerializeField] private GameObject _swordPrefab;
    [SerializeField] private GameObject _lastBOSS;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            ControlBlades(_lastBOSS.transform.position);
        }
    }
    public void ControlBlades(Vector2 attackPos)
   {
        List<Vector2> spawnPosList = new List<Vector2>();
        for (int i = 0; i < _swordCount; i++)
        {
            var dir = i % 2 == 0 ? -1 : 1;
            var p = Quaternion.Euler(0, 0, _angle * ((i + 1) / 2) * dir) * new Vector2(0, _radius);
            spawnPosList.Add(p);
        }
        var startPos = attackPos + new Vector2(0, _radius);
        for (int i = 0; i < spawnPosList.Count; i++)
        {
            var sword = Instantiate(_swordPrefab, startPos, Quaternion.identity);
            sword.transform.up = Vector2.down;

            var mSequence = DOTween.Sequence();
            var pos = attackPos + spawnPosList[i];
            var dir = (attackPos - pos).normalized;
            mSequence.AppendInterval((i + 1) / 2 * _delayTime);
            mSequence.Append(DOTween.To(() => sword.transform.up, x => sword.transform.up = x,(Vector3)dir,0.3f));
            mSequence.Join(sword.transform.DOMove(pos,0.3f));
            mSequence.AppendInterval(1.0f);
            mSequence.Append(sword.transform.DOMove(-dir + pos, 0.4f));
            mSequence.Append(sword.transform.DOMove(attackPos, 0.5f));
            mSequence.Play();

        }

   }
}
