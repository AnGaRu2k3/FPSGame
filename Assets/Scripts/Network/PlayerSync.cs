using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSync : MonoBehaviourPun
{
    [SerializeField] private GameObject muzzleEffect;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject enemyOnMapPrefab;
    [SerializeField] private float followDuration = 3f; // enemyOnMapDuration

    public void ApplyToShoot(Vector3 direction, Vector3 spawnPosition, float velocity)
    {
        Debug.Log("ApplyToShoot");
        Debug.Log($"The player shoot is have viewID = {GetComponent<PhotonView>().ViewID}");
        photonView.RPC("ShootBullet", RpcTarget.All, direction, spawnPosition, velocity);
        photonView.RPC("CreateEnemyDotOnMap", RpcTarget.Others, photonView.ViewID);
    }
    public void ApplyToReloadGun()
    {
        photonView.RPC("ReloadGun", RpcTarget.Others);

    }
    public void ReloadComplete()
    {
        Debug.Log("run reload function");
        if (gameObject.GetComponentInChildren<Weapon>() == null)
        {
            Debug.Log("no weapon to reload complete");  
        }
        gameObject.GetComponentInChildren<Weapon>().ReloadComplete();
    }
    [PunRPC]
    public void ReloadGun()
    {
        gameObject.GetComponentInChildren<Weapon>().StartReloadAnimation();
    }
    
    [PunRPC]
    public void ShootBullet(Vector3 direction, Vector3 spawnPosition, float velocity)
    {
        Debug.Log("ShootBullet");
        AudioManager.instance.PlayFire();
        // muzzle effect
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        // Create bullet and add force
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        
        bullet.transform.forward = direction;
        bullet.GetComponent<Rigidbody>().AddForce(direction * velocity, ForceMode.Impulse);
        bullet.GetComponent<Bullet>().SetShootingPlayer(gameObject);
        StartCoroutine(DestroyBulletAfter(bullet, 3f));
        Debug.Log($"The client receive this RPC have ID {GetComponent<PhotonView>().ViewID}");
    }

    [PunRPC]
    public void CreateEnemyDotOnMap(int shooterViewID)
    {
        // Find Shooter
        PhotonView shooterPhotonView = PhotonView.Find(shooterViewID);
        if (shooterPhotonView == null)
        {
            Debug.LogWarning($"Shooter with ViewID {shooterViewID} not found.");
            return;
        }

        GameObject shooter = shooterPhotonView.gameObject;
        GameObject enemyDot = Instantiate(enemyOnMapPrefab, shooter.transform.position, Quaternion.Euler(90, 0, 0));
        // follow shooter and destroy after duration
        StartCoroutine(FollowPlayerAndDestroy(enemyDot, shooter.transform, followDuration));
    }

    private IEnumerator FollowPlayerAndDestroy(GameObject enemyDot, Transform target, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (target != null && enemyDot != null)
            {
                enemyDot.transform.position = target.position;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //Destroy
        if (enemyDot != null)
        {
            Destroy(enemyDot);
        }
    }

    private IEnumerator DestroyBulletAfter(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

 }
