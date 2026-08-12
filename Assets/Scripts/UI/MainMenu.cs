using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
  public void Play()
  {
    SceneManager.LoadSceneAsync("GreyboxLevel"); //Make sure to change if there is a tutorial or thing you want before gameplay
  }

  public void Quit() 
  {
    Application.Quit();
  }
}
