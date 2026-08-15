using UnityEngine;

public class World 
{
  public void Start()
  {
    Debug.Log("Starting Game");
    // SceneManager.LoadSceneAsync("SampleScene");
  }
}

public abstract record GameState {
    public sealed record Loading : GameState;
    public sealed record Playing : GameState;
    public sealed record Dead : GameState;
    public override string ToString() => this switch {
          Loading => "Loading",
          Playing => "Playing",
          Dead    => "Dead",
          _ => throw new InvalidOperationException()
    };
}
