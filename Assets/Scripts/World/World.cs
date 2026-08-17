using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World {
  private GameManager _game = new GameManager();
  public void Start() { _game.Transition(new GameManager.States.Loading()); }
}

public class GameManager {
  private States _state = new States.Menu();
  public States State => _state;

  public abstract record States {
    public sealed record Menu() : States;
    public sealed record Loading() : States;
    public sealed record Playing() : States;
    public sealed record Dead() : States;
    public override string ToString() => this switch {
      Menu => "Menu",
      Loading => "Loading",
      Playing => "Playing",
      Dead => "Dead",
      _ => throw new InvalidOperationException()
    };
  }

  public void Transition(States to) => _state = Transition(_state, to);

  private States Transition(States state, States to) => (state, to) switch {
    (States.Menu, States.Loading) => LoadWorld(to),
    (States.Loading, States.Playing) => to,
    (States.Playing, States.Dead) => to,
    (States.Dead, States.Loading) => to,
    _ => throw new InvalidOperationException("Invalid transition")
  };

  private States LoadWorld(States state) {
    SceneManager.sceneLoaded += (scene, mode) => Transition(new States.Playing());
    SceneManager.LoadSceneAsync("SampleScene");
    return state;
  }
}

