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
  void Initialize(TreasureManager manager);
  void Open();
}

public class Treasure : MonoBehaviour, ITreasure {
  [SerializeField] private string _id;

  private TreasureManager _manager;
  private readonly List<ICoinPacket> _packets = new();

  public string Id => _id;
  public bool IsOpened { get; private set; }
  public TreasureState State => new(Id, IsOpened, _packets.ToList());
  public void Add(ICoinPacket packet) => _packets.Add(packet);
  public void Initialize(TreasureManager manager) => _manager = manager;

  public void Open() {
    // ...
  }
}

/*
public class StubTreasure<T> : ITreasure where T : ICoinPacket {
  private TreasureManager _manager;
  private readonly List<ICoinPacket> _packets = new();
  public IReadOnlyList<ICoinPacket> Packets => _packets;
  public bool IsOpened { get; private set; }

  public StubTreasure() => _packets.Add(ICoinPacket.Create<T>());

  public void Open() {
    if (IsOpened) return;
    if (_manager == null) throw new InvalidOperationException("TreasureManager not initialized");
    IsOpened = true;
    _packets.ForEach(_manager.Add);
    _packets.Clear();
  }

  public void Add(ICoinPacket packet) => _packets.Add(packet);
  public void Initialize(TreasureManager manager) => _manager = manager;
}
*/
