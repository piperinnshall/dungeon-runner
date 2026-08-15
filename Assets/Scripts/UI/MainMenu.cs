using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
  public void Play() {
    new World().Start();
  }

  public void Quit() {
    Application.Quit();
  }
}
