using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    //weapon
    [SerializeField] private int currentAmmo = 15;
    [SerializeField] private enum Shootmode
    {
        Single,
        Burst,
        Auto
    };
    [SerializeField] private Shootmode currentShootingMode;
    [SerializeField] private float spreadIntensity = 3;
    //bullet
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletVelocity = 30;
    [SerializeField] private float bulletPrefabLifeTime = 3f;
    // shotting
    [SerializeField] private bool isShooting, ready2Shoot;
    [SerializeField] private bool allowReset = true;
    [SerializeField] private float shootingDelay = 1f;
    // burst
    [SerializeField] private int bulletPerBurst = 3;
    [SerializeField] private int currentBulletInBurst = 0 ;
    [SerializeField] private float burstDelay = 0.2f;


    private void Awake()
    {
        ready2Shoot = true;
        currentBulletInBurst = 0;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentShootingMode == Shootmode.Auto) 
        {
            // hold left mouse  
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        if (currentShootingMode == Shootmode.Single || currentShootingMode == Shootmode.Burst) 
        {
            // click left mouse 
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
        if (ready2Shoot && isShooting)
        {
            currentBulletInBurst = 0; 
            FireWeapon();
        }

    }
    private void FireWeapon()
    {
        ready2Shoot = false;
        Vector3 shootingDirection = CalcShootingDirectionAndSpread().normalized;
        Debug.Log("the direction is" + shootingDirection);

        // init and shoot bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        // make the bullet go on the shooting direction
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfter(bullet, bulletPrefabLifeTime));

        // rerun this func until the bulletPerBurst == currentBulletInBurst + 1
        if (currentShootingMode == Shootmode.Burst && currentBulletInBurst + 1 < bulletPerBurst)
        {
            currentBulletInBurst++;
            Invoke("FireWeapon", burstDelay);
        }
        if (allowReset)
        {
            Invoke("Resetshot", shootingDelay);
            allowReset = false;
        }

    }
    void Resetshot()
    {
        ready2Shoot = true;
        allowReset = true;
    }
    private Vector3 CalcShootingDirectionAndSpread()
    {
        //shooting from middle of player camera
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }
        Vector3 direction = (targetPoint - bulletSpawn.position).normalized;

        // Thêm spread sau khi chuẩn hóa
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        Vector3 spread = new Vector3(x, y, 0);

        
        return direction + spread;
    }
    private IEnumerator DestroyBulletAfter(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);

    }
}
