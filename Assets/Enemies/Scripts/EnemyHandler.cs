using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour {
    public PlayerController[] Players;

    private void Start() {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        
        if (p.Length != 0) {
            Players = new PlayerController[p.Length];

            for (int i = 0; i < p.Length; i++)
                Players[i] = p[i].GetComponent(typeof(PlayerController)) as PlayerController;
        }
    }
}
