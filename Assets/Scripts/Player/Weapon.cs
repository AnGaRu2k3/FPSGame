using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.Animations.Rigging;
using Cinemachine;

public class Weapon : MonoBehaviour
{
    const float AIMRECOILUP = 0f;
    const float RECOILUP = 0.1f;
    const float AIMRECOILBACK = 0f;
    const float RECOILBACK = 0.2f;
    [Header("Camera")]
    [SerializeField] private CinemachineFreeLook freelookCamera;
    [Header("Network")]
    [SerializeField] private bool localPlayer = false;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerSync playerSync;
    [Header("Camera and Aiming")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform aimSpot;
    [SerializeField] private bool isAiming = false;
    [SerializeField] private float zoomOut = 60f; 
    [SerializeField] private float zoomIn = 40f;     
    [SerializeField] private float zoomSpeed = 10f; 

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
    [Space]
    [Header("Muzzle")]
    [SerializeField] private GameObject muzzleEffect;
    [Space]
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [Space]
    [Header("Recoil")]

    [SerializeField] private float recoilLength = 1f;
    [SerializeField] private float recoverLength = 2f;
    [SerializeField] private float currentRecoilRigValue = 0f;
    [SerializeField] private Rig recoidRigLayer;
    
    [SerializeField] private float cameraRecoilVertical;
    [SerializeField] private float cameraRecoilHorizontal;
    private float recoilVelocity = 0;

    
    [SerializeField] private bool recoiling = false;
    [SerializeField] private bool recovering = false;
    private void Awake()
    {
        ready2Shoot = true;
        currentBulletInBurst = 0;
        playerCamera = Camera.main;
    }
    private void Start()
    {
        playerSync = player.GetComponent<PlayerSync>();
        recoidRigLayer.weight = currentRecoilRigValue;
        animator = player.GetComponent<Animator>();
        //Debug.Log("defaultCamePos" + defaultCameraPosition);
    }
    // Update is called once per frame
    void Update()
    {
        // if not the owner player
        if (!localPlayer) return;
        if (!player || player.GetComponent<PlayerStatus>().IsDeath()) return;

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
            playerSync.ApplyToReloadGun();
            StartReloadAnimation();
        }
        if (ready2Shoot && isShooting && !reloading)
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
        //if (Input.GetKey(KeyCode.Mouse1)) // hold right mouse
        //{
        //    if (isAiming == false)
        //    {
        //        // aim setting
        //        spreadIntensity /= 10;
        //        // set to aim camera
        //        aimFreeLookCamera.Priority = player.GetComponent<PlayerStatus>().MaxPriority();
        //    }
        //    isAiming = true;
        //}
        //if (Input.GetKeyUp(KeyCode.Mouse1) || reloading) // release right mouse
        //{
        //    if (isAiming == true)
        //    {
        //        // no Aim setting
        //        spreadIntensity *= 10;
        //        // set to tps camera
        //        freelookCamera.Priority = player.GetComponent<PlayerStatus>().MaxPriority();
        //    }
        //    isAiming = false;
        //}

    }
    private void FireWeapon()
    {
        if (ammo == 0) return;
        ammo --;
        recoiling = true;
        recovering = false;
        
        
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
            Debug.Log("Wait for reset");
            Invoke("Resetshot", shootingDelay);
            allowReset = false;
        }


    }
    void Resetshot()
    {
        Debug.Log("have reset");
        ready2Shoot = true;
        allowReset = true;
    }
    // move rig value from 0 - 1 for recoilriglayer
    void Recoil()
    {
        freelookCamera.m_YAxis.Value -= cameraRecoilVertical;
        freelookCamera.m_XAxis.Value += UnityEngine.Random.Range(-cameraRecoilHorizontal, cameraRecoilHorizontal);

        float targetRecoilLayer = 1f;
        currentRecoilRigValue = Mathf.SmoothDamp(currentRecoilRigValue, targetRecoilLayer, ref recoilVelocity, recoilLength);
        recoidRigLayer.weight = currentRecoilRigValue;
        if (Mathf.Abs(currentRecoilRigValue - targetRecoilLayer) < 0.01f)
        {
            recoiling = false;
            recovering = true;
        }

    }
    // move rig value from 1 - 0 for recoilriglayer
    void Recovering()
    {
        float targetRecoilLayer = 0f;
        currentRecoilRigValue = Mathf.SmoothDamp(currentRecoilRigValue, targetRecoilLayer, ref recoilVelocity, recoilLength);
        recoidRigLayer.weight = currentRecoilRigValue;
        if (Mathf.Abs(currentRecoilRigValue - targetRecoilLayer) < 0.01f)
        {
            recoiling = false;
            recovering = false;
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

        int layerMask = ~LayerMask.GetMask("Player");
        //shooting from middle of player camera
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            targetPoint = hit.point;
            Debug.Log("Hit object: " + hit.collider.name);
        }
        else
        {
            targetPoint = ray.GetPoint(100); // Point far away if no hit
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
      
        Debug.Log("Reload Complete!");
    }
    public void SetLocalPlayer()
    {
        localPlayer = true;
        freelookCamera = GameObject.Find("FreeLookCamera").GetComponent<CinemachineFreeLook>();
    }
    public void StartReloadAnimation()
    {
        animator.SetTrigger("Reload");
    } 
}
