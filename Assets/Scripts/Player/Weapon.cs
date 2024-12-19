using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
public class Weapon : MonoBehaviour
{
    const float AIMRECOILUP = 0f;
    const float RECOILUP = 0.1f;
    const float AIMRECOILBACK = 0f;
    const float RECOILBACK = 0.2f;

    [Header("Network")]
    [SerializeField] private bool localPlayer = false;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerSync playerSync;
    [Header("Camera and Aiming")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform aimSpot;
    [SerializeField] private bool isAiming = false;
    [SerializeField] private float aimSpeed = 10f;

    [SerializeField] private float zoomOut = 60f; 
    [SerializeField] private float zoomIn = 40f;     
    [SerializeField] private float zoomSpeed = 10f; 

    private Vector3 defaultCameraPosition;
    private Vector3 currentCameraVelocity = Vector3.zero;
    [Header("Reload")]
    [SerializeField] private bool reloading = false;
    [Space]
    [Header("Weapon")]
    [SerializeField] private int ammo = 60;
    [SerializeField] private int magAmmo = 60;
    [SerializeField] private enum Shootmode
    {
        Single,
        Burst,
        Auto
    };
    
    [SerializeField] private Shootmode currentShootingMode;
    [SerializeField] private float spreadIntensity = 3;
    
    [Space]
    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletVelocity = 30;
    [Space]
    [Header("Shooting")]
    [SerializeField] private float shootingDelay = 1f;
    [SerializeField] private bool isShooting, ready2Shoot;
    [SerializeField] private bool allowReset = true;
    
    [Space]
    [Header("Burst")]
    [SerializeField] private int bulletPerBurst = 3;
    [SerializeField] private int currentBulletInBurst = 0 ;
    [SerializeField] private float burstDelay = 0.2f;
    //[Space]
    //[Header("Muzzle")]
    //[SerializeField] private GameObject muzzleEffect;
    [Space]
    [Header("Animator")]
    [SerializeField] private Animator weaponAnimator;
    [Space]
    [Header("Recoil")]

    [SerializeField] private float recoilLength = 1f;
    [SerializeField] private float recoverLength = 2f;
    [SerializeField] private float recoilUp = 0.1f, recoilBack = 0.2f;

    
    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;

    
    [SerializeField] private bool recoiling = false;
    [SerializeField] private bool recovering = false;
    private void Awake()
    {
        ready2Shoot = true;
        currentBulletInBurst = 0;
        playerCamera = transform.parent.GetComponentInChildren<Camera>();
        weaponAnimator = GetComponent<Animator>();
        originalPosition = transform.localPosition;
    }
    private void Start()
    {
        playerSync = player.GetComponent<PlayerSync>();
        defaultCameraPosition = playerCamera.transform.localPosition;
        //Debug.Log("defaultCamePos" + defaultCameraPosition);
    }
    // Update is called once per frame
    void Update()
    {
        // if not the owner player
        if (!localPlayer) return;

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
        // condition reload 
        if ((reloading == false && ammo == 0)|| (Input.GetKeyDown(KeyCode.R) && ammo < magAmmo))
        {
            reloading = true;
            weaponAnimator.SetTrigger("Reload");
        }
        if (ready2Shoot && isShooting)
        {
            currentBulletInBurst = 0; 
            FireWeapon();
        }
        
        if (recoiling)
        {
            Recoil();
        }
        if (recovering)
        {
            Recovering();
        }
        if (Input.GetKey(KeyCode.Mouse1)) // hold right mouse
        {
            if (isAiming == false)
            {
                // Aim setting
                playerCamera.nearClipPlane = 0.03f;
                spreadIntensity /= 10;
                recoilUp = AIMRECOILUP;
                recoilBack = AIMRECOILBACK;
            }
            isAiming = true;
        }   
        if (Input.GetKeyUp(KeyCode.Mouse1) || reloading) // release right mouse
        {
            if (isAiming == true)
            {
                // no Aim setting
                playerCamera.nearClipPlane = 0.3f;
                spreadIntensity *= 10;
                recoilUp = RECOILUP;
                recoilBack = RECOILBACK;
            }
            isAiming = false;
        }
        
        HandleAiming();
        
    }
    private void FireWeapon()
    {
        if (ammo == 0) return;
        // nếu đang nạp đ
        if (reloading == true)
        {
            weaponAnimator.SetTrigger("CancelReload");
            reloading = false;
        }
        ammo --;
        recoiling = true;
        recovering = false;
        
        weaponAnimator.SetTrigger("Shoot");
        ready2Shoot = false;
        Vector3 shootingDirection = CalcShootingDirectionAndSpread().normalized;
        Debug.Log("the direction is" + shootingDirection);
        // sync throuth all client the bullet


        playerSync.ApplyToShoot(shootingDirection, bulletSpawn.position, bulletVelocity);
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
    void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);
        transform.localPosition =
            Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);
        if (Vector3.Distance(transform.localPosition, finalPosition) < 0.01f)
        {
            recoiling = false;
            recovering = true; 
        }

    }
    void Recovering()
    {
        Vector3 finalPosition = originalPosition;
        transform.localPosition =
            Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);
        if (Vector3.Distance(transform.localPosition, finalPosition) < 0.01f)
        {
            recoiling = false;
            recovering = false;
        }

    }
    void HandleAiming()
    {
        if (isAiming)
        {
            
            // move camera to aim spot
            playerCamera.transform.position = Vector3.SmoothDamp(
                playerCamera.transform.position,
                aimSpot.position,
                ref currentCameraVelocity,
                1f / aimSpeed
            );

            // zoom in
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                zoomIn,
                Time.deltaTime * zoomSpeed
            );
        }
        else
        {
            
            // Move camera to the local position
            playerCamera.transform.localPosition = Vector3.SmoothDamp(
                playerCamera.transform.localPosition,
                defaultCameraPosition,
                ref currentCameraVelocity,
                1f / aimSpeed
            );

            // zoom out
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                zoomOut,
                Time.deltaTime * zoomSpeed
            ) ;
        }
    }
    public int GetAmmo()
    {
        return ammo;
    }
    public int GetMagAmmo()
    {
        return magAmmo;
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
    public void ReloadComplete()
    {
        ammo = magAmmo;
        reloading = false;
        weaponAnimator.SetTrigger("CancelReload");
        Debug.Log("Reload Complete!");
    }
    public void SetLocalPlayer()
    {
        localPlayer = true;
    }
}
