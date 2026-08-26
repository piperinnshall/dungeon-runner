using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public record TreasureState(
  string Id,
  bool IsOpened,
  IReadOnlyList<ICoinPacket> Packets
);

public interface ITreasure {
  TreasureState State { get; }
  void Initialize(TreasureManager manager);
  void Add(ICoinPacket packet);
  void Open();
}

public class Treasure<T> : ITreasure where T : ICoinPacket {
  private string _id;
  private bool _isOpened;
  private readonly List<ICoinPacket> _packets = new();
  private TreasureManager _manager;

  public TreasureState State => new(_id, _isOpened, _packets.AsReadOnly());

  public Treasure(string id) {
    _id = id;
    _packets.Add(ICoinPacket.Create<T>());
  }

  public void Initialize(TreasureManager manager) => _manager = manager;
  public void Add(ICoinPacket packet) => _packets.Add(packet);

  public void Open() {
    if (_isOpened) return;
    _isOpened = true;
    _manager.Open(TakePackets());
  }

  private List<ICoinPacket> TakePackets() {
    var packets = _packets.ToList();
    _packets.Clear();
    return packets;
  }
}

/*
 * var views = FindObjectsOfType<TreasureView>();
 * foreach (var view in treasureViews) {
 *   view.Initialize(manager.Treasures.Single(t => t.Id == view.Id));
 * }
 *
 * public class TreasureView : MonoBehaviour {
 *   private ITreasure _treasure;
 *   public void Initialize(ITreasure treasure) => _treasure = treasure;
 *   public void Open() {
 *     _treasure.Open();
 *     PlayOpeningAnimation();
 *   }
 *   private void PlayOpeningAnimation() {
 *     // Animator.Play(...)
 *   }
 * }
 */
