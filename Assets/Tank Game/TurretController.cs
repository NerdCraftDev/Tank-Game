using System.Runtime.CompilerServices;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class TurretController : NetworkBehaviour
{
    public Transform turretTransform; // Assign the transform of the turret part here.
    public GameObject projectilePrefab; // Assign the projectile prefab in the Unity editor.
    public float shotCooldown = 0.25f;
    public float reloadTime = 1.0f;
    public float ammoYOffset = 1.5f;
    [SyncVar]
    public int clipAmmo;
    public int maxClipAmmo = 4;
    public Sprite ammoSprite;
    private SpriteRenderer[] renderers;
    private bool reloading = false;
    private bool isShooting = false;
    private float reloadCooldown;
    private void Start() {
        if (!isLocalPlayer) return;
        renderers = new SpriteRenderer[maxClipAmmo];
        clipAmmo = maxClipAmmo;
        reloadCooldown = reloadTime;
        for (int i = 0; i < maxClipAmmo; i++) {
            GameObject obj = new GameObject("Ammo Sprite");
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = ammoSprite;
            Vector3 pos = transform.position;
            renderer.transform.position = new Vector3(pos.x-.3f+(.2f*i), pos.y+ammoYOffset, pos.z);
            Vector3 rot = renderer.transform.rotation.eulerAngles;
            rot.x=60;
            renderer.transform.rotation = Quaternion.Euler(rot);
            renderers[i] = renderer;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        reloadCooldown -= Time.deltaTime;
        if (reloadCooldown - (clipAmmo * .2f) <= 0) {
            if (clipAmmo < maxClipAmmo) {
                CmdAddAmmo();
            }
            else {
                reloading = false;
            }
            reloadCooldown = reloadTime;
        }
        // Cast a ray from the camera to the mouse cursor position.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray))
        {
            Vector3 pointAtY = ray.GetPoint(FindDistanceToY(ray, turretTransform.position.y));
            turretTransform.LookAt(pointAtY);
            Vector3 rot = turretTransform.rotation.eulerAngles;
            turretTransform.rotation = Quaternion.Euler(new Vector3(25, rot.y, rot.z));
            // Check for left mouse button click.
            if (Input.GetMouseButton(0) && !isShooting && clipAmmo > 0)
            {
                isShooting = true;
                CmdShootProjectile(turretTransform.Find("Bullet Spawn").position, pointAtY);
                reloadCooldown += .6f*shotCooldown;
            }
        }

        // Adjust the opacity of the sprites based on the variable value
        for (int i = 0; i < renderers.Length; i++)
        {
            if (i < clipAmmo)
            {
                renderers[i].color = new Color(1f, 1f, 1f, 1f); // 100% opacity
            }
            else
            {
                renderers[i].color = new Color(1f, 1f, 1f, 0.5f); // 50% opacity
            }
            Vector3 pos = transform.position;
            renderers[i].transform.position = new Vector3(pos.x-.3f+(.2f*i), pos.y+ammoYOffset, pos.z);
        }
    }


    [Command]
    void CmdShootProjectile(Vector3 spawnPosition, Vector3 pointAt)
    {
        // Instantiate a projectile at the specified starting point.
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        Material projectileMaterial = new Material(Shader.Find("Standard"));
        projectileMaterial.color = GetComponentInChildren<MeshRenderer>().material.color;
        
        // Assign the material to the projectile's renderer.
        MeshRenderer[] projectileRenderers = projectile.GetComponentsInChildren<MeshRenderer>();
        if (projectileRenderers != null)
        {
            foreach (MeshRenderer renderer in projectileRenderers) {
                renderer.material = projectileMaterial;
            }
        }
        projectile.transform.forward = turretTransform.forward;
        projectile.layer = gameObject.layer;
        NetworkServer.Spawn(projectile);
        Vector3 dir = (pointAt - spawnPosition).normalized;

        // Set the velocity for the projectile's rigidbody (if it has one).
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = dir * projectile.GetComponent<Bullet>().speed;
        }

        clipAmmo--;
        if (!reloading) {
            reloading = true;
        }
        RpcResetShooting();
    }

    [ClientRpc]
    void RpcResetShooting() {
        Invoke("ResetShooting", shotCooldown);
    }

    [Command]
    void CmdAddAmmo() {
        RpcAddAmmo();
    }
    [ClientRpc]
    void RpcAddAmmo()
    {
        clipAmmo++;
    }
    void ResetShooting()
    {
        isShooting = false;
    }

    float FindDistanceToY(Ray ray, float targetY)
    {
        if (ray.direction.y == 0)
        {
            return -1; // Direction is parallel to the Y-axis
        }

        // Calculate the distance along the ray where the Y-coordinate matches the target Y
        float distance = (targetY - ray.origin.y) / ray.direction.y;

        if (distance < 0)
        {
            return -1; // The target Y is behind the ray's origin
        }

        return distance;
    }
}
