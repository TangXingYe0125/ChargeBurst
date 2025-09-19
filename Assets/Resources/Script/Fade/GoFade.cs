using UnityEngine;
using System.Collections;

public class GoFade : MonoBehaviour {
    [SerializeField] private string scene;
    [SerializeField] private Color loadToColor;
	
	public void StartFade()
    {
        Initiate.Fade(scene, loadToColor, 1.0f);
    }
}
