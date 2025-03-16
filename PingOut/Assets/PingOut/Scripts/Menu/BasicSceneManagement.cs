using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSceneManagement : MonoBehaviour
{
	[SerializeField, Scene] private int scene;

	[Button]
	public void LoadScene()
	{
		SceneManager.LoadScene(scene, LoadSceneMode.Single);
	}
    [Button]
    public void ReLoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
    public void LoadSceneIn(float sec)
    {
		Invoke("LoadScene", sec);
    }
    public void LoadScene(int index)
	{
		scene = index;
		LoadScene();
	}

	[Button]
	public void Quit()
	{
		if (Application.isEditor)
		{
			Debug.LogError("Application Quit");
		}
		else
		{
			Application.Quit();
		}
	}
}
