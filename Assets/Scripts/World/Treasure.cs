using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Unity supports C# init-only properties used by records, but its framework
// does not provide IsExternalInit. This type is required by the compiler when
// compiling records with positional properties.
namespace System.Runtime.CompilerServices { 
  internal static class IsExternalInit { } 
}

// TODO: Implement with actual Treasure class
public interface ITreasure {
  ICoinPacket Treasure { get; }
  void ReceivePacket(ICoinPacket packet); 
  void Initialize(TreasureManager manager);
}

public class StubTreasure : ITreasure {
  public ICoinPacket Treasure { get; private set; }
  public void ReceivePacket(ICoinPacket packet) => Treasure = packet;
  public void Initialize(TreasureManager manager) { }
}

public class TreasureManager {
  private List<ICoinPacket> _packets = new();
  public int GetTotalCoins() => _packets.Sum(p => p.Amount);
  public void Add(ICoinPacket packet) => _packets.Add(packet);
  public void Remove(ICoinPacket packet) => _packets.Remove(packet);
  public void Clear() {
    _packets.ForEach(packet => packet.ReturnHome());
    _packets.Clear();
  }
}

public interface ICoinPacket {
  int Amount { get; }
  ITreasure Source { get; }
  void ReturnHome() => Source.ReceivePacket(this);

  public sealed record Cheap(int Amount, ITreasure Source) : ICoinPacket;
  public sealed record Moderate(int Amount, ITreasure Source) : ICoinPacket;
  public sealed record Valuable(int Amount, ITreasure Source) : ICoinPacket;
  public sealed record Sacred(int Amount, ITreasure Source) : ICoinPacket;

  public static Cheap CreateCheap(ITreasure source) => new(UnityEngine.Random.Range(1, 6), source);
  public static Moderate CreateModerate(ITreasure source) => new(UnityEngine.Random.Range(7, 14), source);
  public static Valuable CreateValuable(ITreasure source) => new(UnityEngine.Random.Range(15, 21), source);
  public static Sacred CreateSacred(ITreasure source) => new(UnityEngine.Random.Range(22, 27), source);
}
