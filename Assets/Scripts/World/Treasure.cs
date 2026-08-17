using UnityEngine;

public interface ITreasureSource {
  // TOD: Implement with actual Treasure class
  void ReceivePacket(CoinPacket packet);
}

public class TreasureManager {
  private List<CoinPacket> _packets = new();

}

public interface CoinPacket {
    int Amount { get; }

    public sealed record Cheap(int Amount) : CoinPacket;
    public sealed record Moderate(int Amount) : CoinPacket;
    public sealed record Valuable(int Amount) : CoinPacket;
    public sealed record Sacred(int Amount) : CoinPacket;

    public static Cheap Cheap() => new(Random.Shared.Next(1, 6));
    public static Moderate Moderate() => new(Random.Shared.Next(7, 14));
    public static Valuable Valuable() => new(Random.Shared.Next(15, 21));
    public static Sacred Sacred() => new(Random.Shared.Next(22, 27));
}
