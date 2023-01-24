using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    public static bool OutOfBoundsCheck(Transform objectTransform) {
        Vector2 oPos = new Vector2(Mathf.Abs(objectTransform.position.x), Mathf.Abs(objectTransform.position.y));
        Vector2 oScale = objectTransform.lossyScale;
        oScale += oScale * 0.1f; // 10% object scale compensation just in case it seems jarring

        Vector2 bounds = ScreenBounds;
        if ((oPos - oScale).x > bounds.x || (oPos - oScale).y > bounds.y)
            return true;
        else return false;
    }
}

public struct DamageReport {
    private int amountInflicted;
    private Vector2? pointOfCollision; // this is the position of what sent the damage, for example, the projectile's position during the collision
    
    public DamageReport(int damage) {
        this.amountInflicted = damage;
        this.pointOfCollision = null;
    }

    public DamageReport(int damage, Vector2? inflictedAt) {
        this.amountInflicted = damage;
        this.pointOfCollision = inflictedAt;
    }

    public int AmountInflicted { get { return amountInflicted; } }
    public Vector2? PointOfCollision { get { return pointOfCollision; } }
}