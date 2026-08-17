using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World {
  private GameManager _game = new GameManager();
  public void Start() { _game.Transition(new GameManager.States.Loading()); }
}

public class GameManager {
  public States State { get; private set; } = new States.Menu();

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

  public void Transition(States to) => State = Transition(State, to);

  private States Transition(States state, States to) => (state, to) switch {
    (States.Menu, States.Loading) => Loading(to),
    (States.Loading, States.Playing) => to,
    (States.Playing, States.Dead) => to,
    (States.Dead, States.Loading) => to,
    _ => throw new InvalidOperationException("Invalid transition")
  };

  private States Loading(States to) {
    SceneManager.sceneLoaded += OnWorldLoaded;
    SceneManager.LoadSceneAsync("SampleScene");
    return to;
  }

  private void OnWorldLoaded(Scene scene, LoadSceneMode mode) {
      SceneManager.sceneLoaded -= OnWorldLoaded;
      Transition(new States.Playing());
  }
}


