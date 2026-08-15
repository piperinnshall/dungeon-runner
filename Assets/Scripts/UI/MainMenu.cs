using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
  private World _world;
  public void Awake() { _world = new World(); }
  public void Play() { _world.Start(); }
  public void Quit() { Application.Quit(); }
}
