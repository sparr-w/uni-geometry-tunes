using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Material = UnityEngine.Material;
using SphereCollider = Unity.Physics.SphereCollider;

public struct MoveObject : IComponentData {
    public float3 direction;
    public float speed;
}

public class MoveSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.WithAll<MoveObject>().ForEach((Entity entity, ref Translation trans, ref Scale scale, ref MoveObject move) => {
            trans.Value += move.direction * move.speed * Time.DeltaTime;

            float xPos = Mathf.Abs(trans.Value.x);
            xPos -= scale.Value + (scale.Value * 0.1f); // 10% object scale compensation

            float yPos = Mathf.Abs(trans.Value.y);
            yPos -= scale.Value + (scale.Value * 0.1f);

            Vector2 bounds = GlobalVariables.ScreenBounds;
            if (xPos > bounds.x || yPos > bounds.y)
                PostUpdateCommands.DestroyEntity(entity);
        });
    }
}

[System.Serializable]
public struct ProjectileMesh {
    public Shapes Shape;
    public Mesh Mesh;
}

public class ECSProjectileManager : MonoBehaviour {
    private EntityManager entityManager;
    public int Count { get; set; } = 0;
    
    [SerializeField] private Material entityMaterial;
    [SerializeField] private ProjectileMesh[] projectileMeshes;
    
    private Entity bulletPrototype;

    private void Start() {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var desc = new RenderMeshDescription(projectileMeshes[0].Mesh, entityMaterial);

        Entity entity = entityManager.CreateEntity(typeof(Prefab));
        RenderMeshUtility.AddComponents(entity, entityManager, desc);

        entityManager.AddComponentData(entity, new Translation { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity, new Rotation { Value = quaternion.identity });
        entityManager.AddComponentData(entity, new Scale { Value = 1.0f });
        entityManager.AddComponentData(entity, new MoveObject());
        
        // bullet collider
        var colliderMaterial = Unity.Physics.Material.Default;
        colliderMaterial.CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents;
        
        BlobAssetReference<Unity.Physics.Collider> eCollider = SphereCollider.Create(new SphereGeometry {
            Radius = 0.5f,
            Center = float3.zero
        }, filter: CollisionFilter.Default, colliderMaterial );

        var colliderComponent = new PhysicsCollider { Value = eCollider };
        entityManager.AddComponentData(entity, colliderComponent);
        
        bulletPrototype = entity;
    }

    private Mesh GetAssociatedMesh(Shapes shape) {
        foreach (ProjectileMesh pM in projectileMeshes) {
            if (pM.Shape == shape) return pM.Mesh;
        }

        return null;
    }

    public void Spawn(Vector3 position, Quaternion rotation, float scale, float speed, Shapes shape, Color color) {
        Entity newEntity = entityManager.Instantiate(bulletPrototype);

        Material newMaterial = new Material(entityMaterial);
        newMaterial.color = color;

        var newDesc = new RenderMeshDescription(GetAssociatedMesh(shape), newMaterial);
        RenderMeshUtility.AddComponents(newEntity, entityManager, newDesc);
        
        entityManager.SetComponentData(newEntity, new Translation {
            Value = new float3(position.x, position.y, position.z) });
        
        Vector2 direction;
        float angle = Mathf.Deg2Rad * (rotation.eulerAngles.z + 90.0f);

        direction.x = Mathf.Cos(angle);
        direction.y = Mathf.Sin(angle);

        direction = direction.normalized;

        entityManager.AddComponentData(newEntity, new Scale { Value = scale });
        
        entityManager.SetComponentData(newEntity, new MoveObject {
            direction = new float3(direction.x, direction.y, 0.0f),
            speed = speed
        });
        
        entityManager.SetComponentData(newEntity, new Rotation {
            Value = new quaternion(rotation.x, rotation.y, rotation.z, rotation.w) });
        
        Count = Count + 1;
    }
}

[Unity.Burst.BurstCompile]
struct ProjectileTriggerJob : ITriggerEventsJob {
    public void Execute(TriggerEvent triggerEvent) {
        Entity A = triggerEvent.EntityA;
        Entity B = triggerEvent.EntityB;
        

    }
}