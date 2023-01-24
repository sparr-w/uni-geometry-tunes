using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {
    [SerializeField] private int poolCapacity;
    [SerializeField] private Projectile projectile;
    private Projectile[] pool;
    private LinkedList<Projectile> freeList;

    public ProjectilePool Init(int capacity) {
        if (pool == null) {
            freeList = new LinkedList<Projectile>();
            
            pool = new Projectile[capacity];
            for (int i = 0; i < capacity; i++) {
                pool[i] = Instantiate(projectile, this.transform);
                pool[i].SetAssociatedPool(this);
                pool[i].gameObject.SetActive(false);
                freeList.AddFirst(pool[i]);
            }

            poolCapacity = capacity;
        } else ResizePool(capacity);

        return this;
    }

    private void Start() {
        if (pool == null)
            Init(poolCapacity);
    }

    public void ResizePool(int newCapacity) {
        Projectile[] newPool = new Projectile[newCapacity];

        int l = newCapacity > poolCapacity ? newCapacity : poolCapacity;
        
        for (int i = 0; i < l; i++) {
            if (i < poolCapacity) { 
                if (i >= newCapacity) // no longer needed
                    Destroy(pool[i]);
                else // reuse, reduce, recycle
                    newPool[i] = pool[i];
            }
            else {
                newPool[i] = Instantiate(projectile, this.transform);
                newPool[i].SetAssociatedPool(this);
                newPool[i].gameObject.SetActive(false);
            }
        }

        poolCapacity = newCapacity;
        pool = newPool;
    }

    public Projectile GetUnusedProjectile() {
        // look for an unused object and return the first one found
//        foreach (Projectile proj in pool) {
//            if (proj.gameObject.activeInHierarchy == false)
//                return proj;
//        }

        if (freeList.First != null) {
            Projectile proj = freeList.First.Value;
            freeList.RemoveFirst();
            
            return proj;
        }

        return null;
    }

    public void ReturnUsedProjectile(Projectile proj) {
        // set object passed through to being unused
        proj.gameObject.SetActive(false);
        freeList.AddFirst(proj);
    }
}
