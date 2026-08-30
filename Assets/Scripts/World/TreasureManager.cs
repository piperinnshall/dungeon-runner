using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public record TreasureManagerState(
  IReadOnlyList<ICoinPacket> Packets,
  IReadOnlyList<TreasureState> Treasures
);

public class TreasureManager {
  private readonly List<ICoinPacket> _packets = new();
  private List<ITreasure> _treasures = new();

  public TreasureManagerState State =>
    new(_packets.ToList(), _treasures.Select(t => t.State).ToList());

  public void Initialize(IReadOnlyList<ITreasure> treasures) => 
    _treasures = treasures.ToList();

  public int GetTotalCoins() => _packets.Sum(p => p.Amount);

  public void Open(List<ICoinPacket> packets) {
    _packets.AddRange(packets);
    Debug.Log($"Opened Treasure");
    Debug.Log($"Player has: {GetTotalCoins()} Coins");
  }

  public void Redistribute() {
    var closed = _treasures.Where(t => !t.State.IsOpened).ToList();
    if (!closed.Any()) return;
    foreach (var (p, i) in _packets.Enumerate()) closed[i % closed.Count()].Add(p);
    _packets.Clear();
  }
}

public interface ICoinPacket {
  int Amount { get; }
  public sealed record Cheap(int Amount) : ICoinPacket;
  public sealed record Moderate(int Amount) : ICoinPacket;
  public sealed record Valuable(int Amount) : ICoinPacket;
  public sealed record Sacred(int Amount) : ICoinPacket;
  public static ICoinPacket Create<T>() where T : ICoinPacket {
    return typeof(T) switch {
      var t when t == typeof(Cheap) => new Cheap(UnityEngine.Random.Range(1, 6)),
      var t when t == typeof(Moderate) => new Moderate(UnityEngine.Random.Range(7, 14)),
      var t when t == typeof(Valuable) => new Valuable(UnityEngine.Random.Range(15, 21)),
      var t when t == typeof(Sacred) => new Sacred(UnityEngine.Random.Range(22, 27)),
      _ => throw new ArgumentException($"Unknown packet type: {typeof(T)}")
    };
  }
}

