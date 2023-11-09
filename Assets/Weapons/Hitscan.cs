using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : Bullet
{
    [SerializeField] float distance = 10;
    [SerializeField] LayerMask hitLayer;

    protected override float Damage => damage;

    public override void Fire(Vector3 forward)
    {   
        //Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the Scene.
        Ray hitscan = new Ray(transform.position, forward);

        //Structure used to get information back from a raycast.
        RaycastHit[] hits = Physics.RaycastAll(hitscan, distance, hitLayer);
        

        Debug.Log(hits.Length);
        if (hits.Length > 0)
        {
            var hit = hits[0];
            var damageTaker = hit.collider.GetComponent<DamageTaker>();
            damageTaker.TakeDamage(damage);
        }

        StartCoroutine(DelayedDestroy());
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
