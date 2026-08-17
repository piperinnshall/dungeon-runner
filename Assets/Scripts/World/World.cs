using UnityEngine;

public class World {
  public GameManager Game { get; private set; } = new GameManager();
  public TreasureManager Treasure { get; private set; } = new TreasureManager();

  public void Start() {
    Game.Transition(new GameManager.IState.Loading());
    var treasure = new StubTreasure();
    Treasure.Add(ICoinPacket.CreateCheap(treasure));
    Treasure.Add(ICoinPacket.CreateSacred(treasure));
    Debug.Log($"Total Coins: {Treasure.GetTotalCoins()}");
    Treasure.Clear();
    Debug.Log($"Total Coins: {Treasure.GetTotalCoins()}");
    Debug.Log(treasure.Treasure);
  }
}

/*
 * var treasures = FindObjectsOfType<ITreasure>();
 * foreach (var treasure in treasures) {
 *   treasure.Initialize(world.Treasure)
 * }
 */
