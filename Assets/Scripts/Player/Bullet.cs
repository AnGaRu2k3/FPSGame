using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour
{
    public void OnCollisionEnter(Collision objectHit)
    {
        if (objectHit.gameObject.CompareTag("Target"))
        {
            Debug.Log("bullet hit to target named" + objectHit.gameObject.name);
            CreateBulletImpact(objectHit);
            Destroy(gameObject);
        }
        if (objectHit.gameObject.CompareTag("Wall"))
        {
            Debug.Log("bullet hit to wall named" + objectHit.gameObject.name);
            CreateBulletImpact(objectHit);
            Destroy(gameObject);
        }
    }
    void CreateBulletImpact(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];
        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );
        hole.transform.SetParent(objectHit.transform);

    }

}
