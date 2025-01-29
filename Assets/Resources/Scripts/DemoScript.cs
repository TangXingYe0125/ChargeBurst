using UnityEngine;
using System.Collections;

public class DemoScript : MonoBehaviour {
    [SerializeField] private string scene;
    [SerializeField] private Color loadToColor;
	
	public void GoFade()
    {
        Initiate.Fade(scene, loadToColor, 1.0f);
    }
}
