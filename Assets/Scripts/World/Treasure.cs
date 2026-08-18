using System.Collections.Generic;
using UnityEngine;

public interface ITreasure {
  IReadOnlyList<ICoinPacket> Packets { get; }
  bool IsOpened { get; }
  void Open();
  void Add(ICoinPacket packet);
  void Initialize(TreasureManager manager);
}

public class StubTreasure<T> : ITreasure where T : ICoinPacket {
  private TreasureManager _manager;
  private readonly List<ICoinPacket> _packets = new();
  public IReadOnlyList<ICoinPacket> Packets => _packets;
  public bool IsOpened { get; private set; }

  public StubTreasure() => _packets.Add(ICoinPacket.Create<T>());

  public void Open() {
    if (IsOpened || _manager == null) return;
    IsOpened = true;
    _packets.ForEach(_manager.Add);
    _packets.Clear();
  }

  public void Add(ICoinPacket packet) => _packets.Add(packet);
  public void Initialize(TreasureManager manager) => _manager = manager;
}

