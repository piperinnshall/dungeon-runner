using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World {
  public GameManager Game { get; private set; } = new();
  public TreasureManager Treasure { get; private set; } = new();

  public void Start() {
    Game.Transition(new IState.Loading(() => { Test.TreasureView(Treasure); }));
  }
}

class Test {
  public static void TreasureView(TreasureManager tm) {

    var treasures = new List<ITreasure> {
      new Treasure<ICoinPacket.Cheap>("chest_test"),
    };

    var views = UnityEngine.Object.FindObjectsByType<TreasureView>();
    foreach (var view in views){
      view.Initialize(treasures.Single(t => t.State.Id == view.Id));
    }

    tm.Initialize(treasures);
    treasures.ForEach(t => t.Initialize(tm));

    Debug.Log($"Player has: {tm.GetTotalCoins()} Coins");  
  }
}

 /*
  public static void Treasure(TreasureManager tm) {
    Debug.Log("Create All World Treasures:");
    var treasures = new List<ITreasure> {
      new Treasure<ICoinPacket.Cheap>(""),
      new Treasure<ICoinPacket.Sacred>(""),
      new Treasure<ICoinPacket.Valuable>(""),
    };
    tm.Initialize(treasures);
    treasures.ForEach(t => {
      t.Initialize(tm);
      Debug.Log($"Treasure: {string.Join(", ", t.State.Packets)}");
    });
    Debug.Log($"Player has: {tm.GetTotalCoins()} Coins");  
    treasures[0].Open();
    treasures[1].Open();
    Debug.Log($"Open Treasure0");
    Debug.Log($"Open Treasure1");
    Debug.Log($"Player has: {tm.GetTotalCoins()} Coins");
    tm.Redistribute();
    Debug.Log("Player Died. Redistributing Treasure.");
    Debug.Log($"Player has: {tm.GetTotalCoins()} Coins");
    treasures.ForEach(t => Debug.Log($"Treasure: {string.Join(", ", t.State.Packets)} (IsOpened: {t.State.IsOpened})"));
  }
  */


