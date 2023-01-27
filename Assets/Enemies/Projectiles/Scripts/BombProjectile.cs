using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BombProjectile : Projectile {
    [SerializeField] private SpriteRenderer innerBody;
    
    [SerializeField] private Projectile projectileType;
    [SerializeField] private int projectileCount = 0;
    [SerializeField] private float projectileScaleProportion = 0.3f;
    [SerializeField] private float projectileSpeed = 2.0f;
    [SerializeField] private ProjectileHandlers projectileHandler = ProjectileHandlers.Standard;

    private Transform innerBodyTrans;
    private float innerBodyScale = 0.8f;
    private float innerFluctuation = 1.0f;
    private Sprite projectileSprite;
    private Shapes projectileShape;

    private Vector2? bSpawnPos, bDestination, distanceGoal;
    private Vector2 distanceTravelled = Vector2.zero;

    private Projectile FireProjectile(Vector3? localPos = null, Vector3? localRot = null) { // copied behaviour from shooterbehaviour
        localPos ??= new Vector3(0.0f, 0.0f, 0.0f);
        localRot ??= new Vector3(0.0f, 0.0f, 0.0f);

        Projectile proj;
        // get/make new projectile
        switch (projectileHandler) {
            case ProjectileHandlers.Pooling:
                proj = GlobalVariables.ProjectilePool.GetUnusedProjectile();
                proj.transform.SetParent(this.transform);
                break;
            case ProjectileHandlers.Entities:
                Vector3 eRot = localRot.Value + this.transform.localEulerAngles;
                Vector3 inheritPos = this.transform.position;
                Vector3 ePos = inheritPos + localPos.Value;

                float theta = Mathf.Deg2Rad * this.transform.localEulerAngles.z;
                ePos.x = (Mathf.Cos(theta) * localPos.Value.x) - (Mathf.Sin(theta) * localPos.Value.y) + inheritPos.x;
                ePos.y = (Mathf.Sin(theta) * localPos.Value.x) + (Mathf.Cos(theta) * localPos.Value.y) + inheritPos.y;

                Vector2 pScale = this.transform.lossyScale;
                pScale *= projectileScaleProportion;
                
                GlobalVariables.EntityHandler.Spawn(ePos, Quaternion.Euler(eRot), pScale.x, projectileSpeed,
                    projectileShape, bodyColors[1]);
                
                return null;
            default:
                proj = Instantiate(projectileType, this.transform);
                break;
        }
        
        Transform pTrans = proj.transform;
        Vector3 bScale = transform.localScale;
        // apply transforms
        pTrans.localEulerAngles = localRot.Value;
        pTrans.localScale = new Vector3(bScale.x * projectileScaleProportion, 
            bScale.y * projectileScaleProportion, 1.0f);
        pTrans.localPosition = localPos.Value;
        proj.SetSprite(projectileSprite);
        
        // return object to the pool
        if (projectileHandler == ProjectileHandlers.Pooling)
            pTrans.SetParent(GlobalVariables.ProjectilePool.transform);
        else 
            pTrans.SetParent(null);

        // initialise projectile
        proj.Init(projectileSpeed);
        proj.SetColors(bodyColors);
        
        // if pooling, set object to being used by making it active
        if (projectileHandler == ProjectileHandlers.Pooling)
            proj.gameObject.SetActive(true);
        
        return proj;
    }

    protected override void GetSpriteRenderer() { spriteRenderer = gameObject.GetComponent<SpriteRenderer>(); }

    public override void SetColors(Color[] newColors) {
        base.SetColors(newColors);

        innerBody.color = newColors[0];
    }

    public override void SetSprite(Sprite newSprite) {
        projectileSprite = newSprite;
    }

    public void SetShape(Shapes shape) {
        this.projectileShape = shape;
    }
    
    public BombProjectile InitBomb(ProjectileHandlers handler = ProjectileHandlers.Standard, int shrapnelCount = 10, 
        float shrapnelProportion = 0.3f, float shrapnelSpeed = 2.0f) {
        this.projectileHandler = handler;
        this.projectileCount = shrapnelCount;
        this.projectileScaleProportion = shrapnelProportion;
        this.projectileSpeed = shrapnelSpeed;

        return this;
    }
    
    private void Explode() {
        if (projectileType == null) {
            Destroy(this.gameObject); // no behaviour just delete object
            return;
        }

        float projAngle = 360.0f / projectileCount;

        for (int i = 1; i <= projectileCount; i++) {
            FireProjectile(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, (projAngle * i)));
        }
        
        Destroy(this.gameObject); // all projectiles deployed, no need for this object
        return;
    }

    private void FluctuateInnerBody() {
        if (innerBodyTrans == null) innerBodyTrans = innerBody.transform;
        
        innerBodyScale += Time.deltaTime * innerFluctuation;
        innerBodyScale = Mathf.Clamp(innerBodyScale, 0.6f, 0.8f);
        
        if (innerBodyScale >= 0.8f || innerBodyScale <= 0.6f)
            innerFluctuation *= -1;

        innerBodyTrans.localScale = new Vector3(innerBodyScale, innerBodyScale, innerBodyTrans.localScale.z);
    }

    private void Start() {
        Vector2 spawnPos = this.transform.position;
        bSpawnPos = spawnPos;
        
        bDestination = bSpawnPos.Value * -1;
        // if the destination isn't on the boundary of the world, this is pointless, it needs to hit the world boundary
        if (!(Mathf.Abs(bDestination.Value.x) >= GlobalVariables.ScreenBounds.x) &&
            !(Mathf.Abs(bDestination.Value.y) >= GlobalVariables.ScreenBounds.y)) bDestination = null;
        else {
            bDestination = new Vector2(
                Mathf.Clamp(bDestination.Value.x, -GlobalVariables.ScreenBounds.x, GlobalVariables.ScreenBounds.x),
                Mathf.Clamp(bDestination.Value.y, -GlobalVariables.ScreenBounds.y, GlobalVariables.ScreenBounds.y));

            distanceGoal = bDestination - spawnPos;
        }
    }

    protected override void Update() {
        Move(Speed * Time.deltaTime);
        
        if (bDestination != null) distanceTravelled = (Vector2)transform.position - bSpawnPos.Value;

        FluctuateInnerBody();
    }
    
    private void LateUpdate() {
        if (bDestination == null) {
            if (Mathf.Abs(transform.position.x) >= Mathf.Abs(GlobalVariables.ScreenBounds.x) ||
                Mathf.Abs(transform.position.y) >= Mathf.Abs(GlobalVariables.ScreenBounds.y))
                Explode();
        }
        else {
            if (distanceTravelled.sqrMagnitude >= distanceGoal.Value.sqrMagnitude) 
                Explode();
        }
    }
}
