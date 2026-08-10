using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
  public void Play()
  {
    SceneManager.LoadSceneAsync("SampleScene");
  }

  public void Quit() 
  {
    Application.Quit();
  }
}
