using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreasureManager {
  private List<ICoinPacket> Packets { get; } = new();

  public int GetTotalCoins() => Packets.Sum(p => p.Amount);
  public void Add(ICoinPacket packet) => Packets.Add(packet);
  public void Remove(ICoinPacket packet) => Packets.Remove(packet);
  public void Clear() => Packets.Clear();
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

