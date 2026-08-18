using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World {
  public GameManager Game { get; private set; } = new GameManager();
  public TreasureManager Treasure { get; private set; } = new TreasureManager();

  public void Start() {
    Game.Transition(new IState.Loading());
    TestTreasures();
  }

  private void TestTreasures() {
    var treasures = new List<ITreasure> {
      new StubTreasure<ICoinPacket.Cheap>(),
      new StubTreasure<ICoinPacket.Sacred>(),
      new StubTreasure<ICoinPacket.Valuable>(),
    };
    treasures.ForEach(t => {
        t.Initialize(Treasure);
        Debug.Log($"Treasure: {string.Join(", ", t.Packets)}");
    });
    treasures[0].Open();
    treasures[1].Open();
    Debug.Log($"Opened Treasures. Manager Coins: {Treasure.GetTotalCoins()}");
    Debug.Log("Player Died");
    Treasure.OnPlayerDeath(treasures);
    treasures.ForEach(t => Debug.Log($"Treasure: {string.Join(", ", t.Packets)} (IsOpened: {t.IsOpened})"));
    Debug.Log($"Manager Coins: {Treasure.GetTotalCoins()}");
  }
}

/*
 * var treasures = FindObjectsOfType<ITreasure>();
 * foreach (var treasure in treasures) {
 *   treasure.Initialize(world.Treasure)
 * }
 */
