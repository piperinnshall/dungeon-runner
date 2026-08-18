using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreasureManager {
  private List<ICoinPacket> _packets = new();
  public int GetTotalCoins() => _packets.Sum(p => p.Amount);
  public void Add(ICoinPacket packet) => _packets.Add(packet);
  public void Remove(ICoinPacket packet) => _packets.Remove(packet);

  public void OnPlayerDeath(List<ITreasure> allTreasures) {
    var closed = allTreasures.Where(t => !t.IsOpened).ToList();
    if (closed.Count == 0) return;
    foreach (var (p, i) in _packets.Enumerate()) closed[i % closed.Count].Add(p);
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
    var amount = typeof(T).Name switch {
      nameof(Cheap) => UnityEngine.Random.Range(1, 6),
      nameof(Moderate) => UnityEngine.Random.Range(7, 14),
      nameof(Valuable) => UnityEngine.Random.Range(15, 21),
      nameof(Sacred) => UnityEngine.Random.Range(22, 27),
      _ => throw new System.ArgumentException("Unknown packet type")
    };
    return (ICoinPacket)Activator.CreateInstance(typeof(T), amount);
  }
}

