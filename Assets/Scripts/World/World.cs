using UnityEngine;

public class World {
  private GameState _state = new GameState.Menu();
  public GameState State => _state;
  public void Transition(GameState desired) { _state = TransitionState(_state, desired); }

  public void Start() {
    Debug.Log("Starting Game");
    SceneManager.LoadSceneAsync("SampleScene");
  }
}

public abstract record GameState {
  public sealed record Menu : GameState;
  public sealed record Loading : GameState;
  public sealed record Playing : GameState;
  public sealed record Dead : GameState;
  public override string ToString() => this switch {
    Menu => "Menu",
    Loading => "Loading",
    Playing => "Playing",
    Dead    => "Dead",
    _ => throw new InvalidOperationException()
  };
}

public GameState TransitionState(GameState fromState, GameState toState) {
  return (fromState, toState) switch {
    (GameState.Loading, GameState.Playing) => toState,
    (GameState.Playing, GameState.Dead) => toState,
    (GameState.Dead, GameState.Loading) => toState,
    _ => throw new InvalidOperationException("Invalid transition")
  };
}
