using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager {
  public IState State { get; private set; } = new IState.Menu();

  public interface IState {
    public sealed record Menu : IState;
    public sealed record Loading : IState;
    public sealed record Playing : IState;
    public sealed record Dead : IState;
  }

  public void Transition(IState to) => State = Transition(State, to);

  private IState Transition(IState state, IState to) => (state, to) switch {
    (IState.Menu, IState.Loading) => Loading(to),
    (IState.Loading, IState.Playing) => to,
    (IState.Playing, IState.Dead) => to,
    (IState.Dead, IState.Loading) => to,
    _ => throw new InvalidOperationException("Invalid transition")
  };

  private IState Loading(IState to) {
    SceneManager.sceneLoaded += OnWorldLoaded;
    SceneManager.LoadSceneAsync("SampleScene");
    return to;
  }

  private void OnWorldLoaded(Scene scene, LoadSceneMode mode) {
    SceneManager.sceneLoaded -= OnWorldLoaded;
    Transition(new IState.Playing());
  }
}
