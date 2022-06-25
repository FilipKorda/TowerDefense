using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public string levelToLoad = "MainLevel";
	public string levelToLoad1 = "Store";

	public SceneFader sceneFader;

	public void Play ()
	{
		sceneFader.FadeTo(levelToLoad);
	}

	public void Store()
	{
		sceneFader.FadeTo(levelToLoad1);
	}

	public void Quit ()
	{
		Debug.Log("Exciting...");
		Application.Quit();
	}

}