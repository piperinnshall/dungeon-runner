using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World {
  public GameManager Game { get; private set; } = new GameManager();
  public TreasureManager Treasure { get; private set; } = new TreasureManager();

  public void Start() {
    Game.Transition(new IState.Loading());
    Test.Treasure(Treasure);
  }
}

class Test {
  public static void Treasure(TreasureManager Treasure) {
    Debug.Log("Create All World Treasures:");
    var treasures = new List<ITreasure> {
      new StubTreasure<ICoinPacket.Cheap>(),
      new StubTreasure<ICoinPacket.Sacred>(),
      new StubTreasure<ICoinPacket.Valuable>(),
    };
    treasures.ForEach(t => {
      t.Initialize(Treasure);
      Debug.Log($"Treasure: {string.Join(", ", t.Packets)}");
    });
    Debug.Log($"Player has: {Treasure.GetTotalCoins()} Coins");
    Debug.Log("");

    treasures[0].Open();
    treasures[1].Open();
    Debug.Log($"Open Treasure0");
    Debug.Log($"Open Treasure1");
    Debug.Log($"Player has: {Treasure.GetTotalCoins()} Coins");
    Debug.Log("");

    Treasure.OnPlayerDeath(treasures);
    Debug.Log("Player Died. Redistributing Treasure.");
    Debug.Log($"Player has: {Treasure.GetTotalCoins()} Coins");
    Debug.Log("");

    treasures.ForEach(t => Debug.Log($"Treasure: {string.Join(", ", t.Packets)} (IsOpened: {t.IsOpened})"));
  }
}

/*
 * var treasures = FindObjectsOfType<ITreasure>();
 * foreach (var treasure in treasures) {
 *   treasure.Initialize(world.Treasure)
 * }
 */
