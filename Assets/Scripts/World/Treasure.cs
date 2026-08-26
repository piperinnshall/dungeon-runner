using System;
using System.Collections.Generic;
using UnityEngine;

public record TreasureState(
  string Id,
  bool IsOpened,
  IReadOnlyList<ICoinPacket> Packets,
);

public interface ITreasure {
  string Id { get; }
  bool IsOpened { get; }
  TreasureState State { get; }
  void Add(ICoinPacket packet);
  void Open();
}

public class Treasure<T> : MonoBehaviour, ITreasure where T : ICoinPacket {
  [SerializeField] private string _id;
  private readonly List<ICoinPacket> _packets = new();

  public string Id => _id;
  public bool IsOpened { get; private set; }
  public TreasureState State => new(Id, IsOpened, _packets.ToList());

  private void Awake() => _packets.Add(ICoinPacket.Create<T>());
  public void Add(ICoinPacket packet) => _packets.Add(packet);
  public void Open() => IsOpened = true;
}

// Unity needs concrete MonoBehaviour types for GameObject's in the Inspector.
public class CheapTreasure : Treasure<ICoinPacket.Cheap> {}
public class ModerateTreasure : Treasure<ICoinPacket.Moderate> {}
public class ValuableTreasure : Treasure<ICoinPacket.Valuable> {}
public class SacredTreasure : Treasure<ICoinPacket.Sacred> {}
