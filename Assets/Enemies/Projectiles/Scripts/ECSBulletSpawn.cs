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
            trans.Value += move.velocity * Time.DeltaTime * 0.2f;
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
        entityManager.AddComponentData(entity, new Scale { Value = 0.2f });
        entityManager.AddComponentData(entity, new MoveObject());
        bulletPrototype = entity;
    }

    public void Spawn(Vector3 position, Quaternion rotation, Color color) {
        Entity newEntity = entityManager.Instantiate(bulletPrototype);

        Material newMaterial = new Material(entityMaterial);
        newMaterial.color = color;

        var newDesc = new RenderMeshDescription(entityMesh, newMaterial);
        RenderMeshUtility.AddComponents(newEntity, entityManager, newDesc);
        
        entityManager.SetComponentData(newEntity, new Translation {
            Value = new float3(position.x, position.y, position.z) });

        Vector2 direction;
        float angle = Mathf.Deg2Rad * (rotation.eulerAngles.z + 90.0f);

        direction.x = Mathf.Cos(angle);
        direction.y = Mathf.Sin(angle);
        
        entityManager.SetComponentData(newEntity, new MoveObject {
            velocity = new float3(direction.x, direction.y, 0.0f) });
        
        entityManager.SetComponentData(newEntity, new Rotation {
            Value = new quaternion(rotation.x, rotation.y, rotation.z, rotation.w) });
        
        Count = Count + 1;
    }
}
