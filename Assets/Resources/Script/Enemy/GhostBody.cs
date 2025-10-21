using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBody : MonoBehaviour
{
    [SerializeField] private Ghost _ghost;
    public void OnSkillEnd()
    {
        _ghost.OnSkillEnd();
    }
}
