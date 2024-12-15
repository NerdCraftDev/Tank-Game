using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float speed = 10.0f;
    public float maxBounces = 3; // Adjust this as needed.
    private int bounceCount = 0;

    [ServerCallback]
    private void Update() {
        transform.forward = transform.GetComponent<Rigidbody>().velocity.normalized;
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        int collisionLayer = collision.gameObject.layer;
        string collisionLayerString = LayerMask.LayerToName(collisionLayer);
        if (collisionLayer != gameObject.layer && collision.gameObject.tag == "Tank") {
            collision.gameObject.transform.position = GameObject.Find(collisionLayerString.Substring(0, collisionLayerString.Length-5)).transform.Find("Spawner").position;
            collision.gameObject.transform.LookAt(Vector3.zero);
            RpcUpdatePos(collision.gameObject.transform, GameObject.Find(collisionLayerString.Substring(0, collisionLayerString.Length-5)).transform.Find("Spawner").position);
            DestroySelf();
        }
        // Check if the bullet should bounce.
        if (bounceCount < maxBounces) {
            if (collisionLayer != gameObject.layer && !collisionLayerString.Contains("Tank")) {
                // Reflect the bullet's velocity upon collision.
                Vector3 reflectedVelocity = Vector3.Reflect(transform.GetComponent<Rigidbody>().velocity, collision.GetContact(0).normal);
                transform.GetComponent<Rigidbody>().velocity = reflectedVelocity.normalized * speed;
                transform.forward = reflectedVelocity.normalized;
                bounceCount++;
            }
        }
        else {
            // If the maximum number of bounces is reached, destroy the bullet.
            DestroySelf();
        }
    }

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    void RpcUpdatePos(Transform obj, Vector3 pos) {
        obj.position = pos;
        obj.LookAt(Vector3.zero);
    }
}
