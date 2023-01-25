using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public struct MoveObject : IComponentData {
    public float3 velocity;
}

public class MoveSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.WithAll<MoveObject>().ForEach((ref Translation trans, ref MoveObject move) => {
            trans.Value += move.velocity * Time.DeltaTime;
        });
    }
}

public class ECSBulletSpawn : MonoBehaviour {
    private EntityManager entityManager;
    public int Count { get; set; } = 0;
    
    [SerializeField] private Mesh entityMesh;
    [SerializeField] private Material entityMaterial;
    
    private Entity bulletPrototype;

    private void Start() {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var desc = new RenderMeshDescription(entityMesh, entityMaterial);

        Entity entity = entityManager.CreateEntity(typeof(Prefab));
        RenderMeshUtility.AddComponents(entity, entityManager, desc);

        entityManager.AddComponentData(entity, new Translation { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity, new Rotation { Value = quaternion.identity });
        entityManager.AddComponentData(entity, new Scale { Value = 1.0f });
        entityManager.AddComponentData(entity, new MoveObject());
        bulletPrototype = entity;
    }

    public void Spawn(Vector3 position, Vector3 velocity) {
        Entity newEntity = entityManager.Instantiate(bulletPrototype);
        entityManager.SetComponentData(newEntity, new Translation {
            Value = new float3(position.x, position.y, position.z) });
        entityManager.SetComponentData(newEntity, new MoveObject {
            velocity = new float3(velocity.x, velocity.y, velocity.z) });

        Count = Count + 1;
    }
}
