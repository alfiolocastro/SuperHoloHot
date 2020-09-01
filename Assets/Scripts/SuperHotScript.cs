using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Input;

using DG.Tweening;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

public class SuperHotScript : MonoBehaviour
{

    public static SuperHotScript instance;

    public float timeLerpSpeed = .3f;

    public float movementDeltaTreShold = .12f; //x second

    public float minTimeScale = .03f;

    public Transform player;

    public float charge;
    public bool canShoot = true;
    public bool action;
    public GameObject bullet;

    [Header("Weapon")]
    public WeaponScript weaponscript;
    public static Transform weapon;
    public LayerMask weaponLayer;


    [Space]
    [Header("UI")]
    public Image indicator;

    [Space]
    [Header("Prefabs")]
    public GameObject hitParticlePrefab;
    public GameObject bulletPrefab;

    Vector3 prevPLayerPos;

    float timeScale;


    public TMP_Text uidelta;
    public TMP_Text uits;

    public AudioSource mainTheme;

    public bool defaultTime;

    private void Awake()
    {
        instance = this;
        if (weapon != null && weapon.GetComponent<WeaponScript>() != null)
            weaponscript = weapon.GetComponent<WeaponScript>();

        prevPLayerPos = player.position;
    }

    public void GetWeapon( ManipulationEventData manipulationEventData)
    {
        GetWeapon(manipulationEventData.ManipulationSource);
    }

    public void GetWeapon(GameObject weap)
    {

        StopCoroutine(ActionE(.1f));
        StartCoroutine(ActionE(.1f));

        weapon = weap.transform;
        weaponscript = weapon.GetComponent<WeaponScript>();
        weaponscript.Equip(true);
    }


    public void ReleaseWeapon(ManipulationEventData manipulationEventData)
    {
        ReleaseWeapon();
    }

    public void ReleaseWeapon()
    {
        StopCoroutine(ActionE(.1f));
        StartCoroutine(ActionE(.1f));

        weaponscript.Equip(false);
        weapon = null;
        weaponscript = null;
    }


    float onTargetTime=0;

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        */
        bool targeted = false;
        if (weapon != null && weaponscript != null)
        {
            ParticleSystem muzzle = weapon.GetComponentInChildren<ParticleSystem>();
            Vector3 origin = muzzle.transform.position;
            Vector3 dir = muzzle.transform.forward;

            //Ray ray = new Ray(origin, dir);
            //Debug.DrawRay(origin, dir*20, Color.blue);

             RaycastHit _hit;
            if (Physics.Raycast(origin, dir, out _hit, 15))
            {
                if (_hit.transform.tag == "Enemy")
                {
                    //Debug.DrawRay(origin, dir*20, Color.black);
                    targeted = true;
                    //onTargetTime += Time.deltaTime;
                }
            }

        }

        if (targeted)
            onTargetTime += Time.fixedUnscaledDeltaTime;
        else
            onTargetTime = 0;


        if (onTargetTime >= 1f)
        {
            
            Shoot();
            onTargetTime = 0;
        }    


        if (Input.GetKeyDown(KeyCode.Space))
        {
            defaultTime = !defaultTime;
        }

        float weaponVelocity = 0;
        if (weaponscript != null)
            weaponVelocity = weaponscript.movementVelocity.magnitude  / Time.unscaledDeltaTime;

        float playerVelocity = Vector3.Distance(player.position, prevPLayerPos) / Time.deltaTime;
        playerVelocity = (playerVelocity >= movementDeltaTreShold) ? playerVelocity : 0;



        float movementSpeed = Mathf.Max(playerVelocity,weaponVelocity);
        prevPLayerPos = player.position;

        /*
        if (Input.GetMouseButtonDown(1))
        {
            StopCoroutine(ActionE(.4f));
            StartCoroutine(ActionE(.4f));

            if(weaponscript != null)
            {
                weaponscript.Throw();
                weaponscript = null;
            }
        }
        */
        
        /*
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit,10, weaponLayer))
        {
            if (Input.GetMouseButtonDown(0) && weaponscript == null)
            {
                hit.transform.GetComponent<WeaponScript>().Pickup();
            }
        }
        */

        float time = Mathf.Clamp(movementSpeed, minTimeScale, 1f);

        time = defaultTime ? 1f : time;
        

        Time.timeScale = Mathf.Lerp(Time.timeScale, time, timeLerpSpeed);

        if (action)
            Time.timeScale = 1;

        timeScale = Time.timeScale;

        mainTheme.pitch = timeScale;

        //uits.text = timeScale.ToString("N3");
        uidelta.text = movementSpeed.ToString("N3"); ;
    }

    IEnumerator ActionE(float time)
    {
        action = true;
        yield return new WaitForSecondsRealtime(time);
        action = false;
    }

    public void ReloadUI(float time)
    {
       // indicator.transform.DORotate(new Vector3(0, 0, 90), time, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).OnComplete(() => indicator.transform.DOPunchScale(Vector3.one / 3, .2f, 10, 1).SetUpdate(true));
    }

    public void Shoot()
    {
        Debug.Log("shooting");
        if (canShoot)
        {
            //if (Input.GetMouseButtonDown(0))
            {
                StopCoroutine(ActionE(.1f));
                StartCoroutine(ActionE(.1f));
                if (weaponscript != null)
                    weaponscript.Shoot(weaponscript.transform.GetComponentInChildren<ParticleSystem>().transform.position, weaponscript.transform.rotation, false);
            }
        }
    }


}
