using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World {
  private GameManager _game = new GameManager();
  public void Start() { _game.Transition(new GameManager.State.Menu()); }
}

public class GameManager {
  private State _state = new State.Menu();
  public State State => _state;

  public abstract record State {
    public sealed record Menu() : State;
    public sealed record Loading() : State;
    public sealed record Playing() : State;
    public sealed record Dead() : State;
    public override string ToString() => this switch {
      Menu => "Menu",
      Loading => "Loading",
      Playing => "Playing",
      Dead => "Dead",
      _ => throw new InvalidOperationException()
    };
  }

  public void Transition(State to) { _state = Transition(_state, to); }
  private State Transition(State state, State to) => (state, to) switch {
    (State.Menu, State.Loading) => LoadWorld(to),
    (State.Loading, State.Playing) => to,
    (State.Playing, State.Dead) => to,
    (State.Dead, State.Loading) => to,
    _ => throw new InvalidOperationException("Invalid transition")
  };

  private State LoadWorld(State state) {
    Debug.Log("Loading World");
    SceneManager.LoadSceneAsync("SampleScene");
    return state;
  }
}

