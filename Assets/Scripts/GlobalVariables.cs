using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour {
    private static PlayerController[] players;

    private static void GatherPlayers() {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        
        if (p.Length != 0) {
            players = new PlayerController[p.Length];

            for (int i = 0; i < p.Length; i++)
                players[i] = p[i].GetComponent(typeof(PlayerController)) as PlayerController;
        }
    }

    public static PlayerController[] Players {
        get {
            if (players != null) return players;
            else {
                GatherPlayers();
                return players;
            }
        }
    }

    public static Vector2 ScreenBounds {
        get {
            Vector2 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,
                Screen.height, Camera.main.transform.position.z));

            return bounds;
        }
    }
}
