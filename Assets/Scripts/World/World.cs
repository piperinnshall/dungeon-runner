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
    treasures.ForEach(treasure => treasure.Initialize(Treasure));
    Debug.Log($"First Treasure: {string.Join(", ", treasures.First().Packets)}");
    
    treasures.First().Open();
  }

}

/*
 * var treasures = FindObjectsOfType<ITreasure>();
 * foreach (var treasure in treasures) {
 *   treasure.Initialize(world.Treasure)
 * }
 */
