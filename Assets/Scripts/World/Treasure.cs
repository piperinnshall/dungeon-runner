using UnityEngine;
using System.Collections.Generic;

// Unity supports C# init-only properties used by records, but its framework
// does not provide IsExternalInit. This type is required by the compiler when
// compiling records with positional properties.
namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }

// TODO: Implement with actual Treasure class
public interface ITreasure {
  ICoinPacket Treasure { get; }
  void ReceivePacket(ICoinPacket packet); 
  void Initialize(TreasureManager manager);
}


public class TreasureManager {
  private List<ICoinPacket> _packets = new();
  public int GetTotalCoins() => _packets.Sum(p => p.Amount);
  public void Add(ICoinPacket packet) => _packets.Add(packet);

}

public interface ICoinPacket {
    int Amount { get; }

    public sealed record Cheap(int Amount) : ICoinPacket;
    public sealed record Moderate(int Amount) : ICoinPacket;
    public sealed record Valuable(int Amount) : ICoinPacket;
    public sealed record Sacred(int Amount) : ICoinPacket;

    public static Cheap CreateCheap() => new(UnityEngine.Random.Range(1, 6));
    public static Moderate CreateModerate() => new(UnityEngine.Random.Range(7, 14));
    public static Valuable CreateValuable() => new(UnityEngine.Random.Range(15, 21));
    public static Sacred CreateSacred() => new(UnityEngine.Random.Range(22, 27));
}
