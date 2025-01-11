using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private GameObject shootingPlayer;
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
        if (objectHit.gameObject.CompareTag("Player"))
        {
            Debug.Log("bullet hit to other player");
            GameObject player = objectHit.gameObject;
            CreateBloodSpray(objectHit);
            player.GetComponent<PlayerStatus>().TakeDamage(30, shootingPlayer);
        }
    }
    public void SetShootingPlayer(GameObject _player)
    {
        shootingPlayer = _player;
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
        Destroy(hole, 2.0f);

    }
    void CreateBloodSpray(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];
        GameObject bloodSpray = Instantiate(
            GlobalReferences.Instance.bloodSprayPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );
        bloodSpray.transform.SetParent(objectHit.transform);
        Destroy(bloodSpray, 2.0f);

    }

}
