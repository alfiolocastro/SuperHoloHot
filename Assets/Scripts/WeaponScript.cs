using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

using Microsoft.MixedReality.Toolkit.UI;

[RequireComponent(typeof(Rigidbody))]
[SelectionBase]
public class WeaponScript : MonoBehaviour
{
    [Header("Bools")]
    public bool active = true;
    public bool reloading;

    private Rigidbody rb;
    private Collider collider;
    private Renderer renderer;

    [Space]
    [Header("Weapon Settings")]
    public float reloadTime = .3f;
    public int bulletAmount = 6;

    AudioSource clip;

    bool isPlayerWeapon;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();

        clip = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (bulletAmount == -1)
            bulletAmount = 256;
        else
            if (bulletAmount >1)
            bulletAmount = Random.Range(1, bulletAmount);


       

        //ChangeSettings();
    }

    public void AssingToPlayer()
    {
        if (SuperHotScript.instance != null)
        {
            SuperHotScript.instance.GetWeapon(this.gameObject);
            //Equip(true);
            isPlayerWeapon = true;
        }
    }

    public void Equip(bool inUse)
    {

        if (rb != null)
        {
            rb.isKinematic = inUse;
            rb.useGravity = !inUse;
        }
            //rb.interpolation = RigidbodyInterpolation.Interpolate;
            if (collider != null)
                collider.isTrigger = inUse;

        
        if (inUse)
        if (SuperHotScript.weapon == this)
        {
            collider.isTrigger = false;
        }

        if (!inUse)
            isPlayerWeapon = false;

    }

    public void Grab()
    {
        collider.isTrigger = false;
        //rb.isKinematic = true;
    }


    void ChangeSettings()
    {
        if (transform.parent != null)
            return;

        rb.isKinematic = (SuperHotScript.instance.weaponscript == this) ? true : false;
       // rb.interpolation = (SuperHotScript.instance.weaponscript == this) ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
        collider.isTrigger = (SuperHotScript.instance.weaponscript == this);
    }

    private void Update()
    {
        if (clip != null)
        clip.pitch = Time.timeScale;
    }

    public bool Shoot(Vector3 pos,Quaternion rot, bool isEnemy)
    {

        if (!active)
            return true;

        if (clip != null)
        clip.pitch = Time.timeScale;

        if (reloading)
            return true;

        if (bulletAmount <= 0)
            return false;

        if (SuperHotScript.instance == null)
            return true;

        if (!SuperHotScript.instance.gameObject.activeInHierarchy)
            return true;

        if (!SuperHotScript.instance.weaponscript == this)
            bulletAmount--;

        GameObject bullet = Instantiate(SuperHotScript.instance.bulletPrefab, pos, rot);

        clip.Play();

        if (GetComponentInChildren<ParticleSystem>() != null)
            GetComponentInChildren<ParticleSystem>().Play();

        if(SuperHotScript.instance.weaponscript == this)
            StartCoroutine(Reload());

        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .01f, 10, 90, false, true).SetUpdate(true);

        //if(SuperHotScript.instance.weaponscript == this)
        //    transform.DOLocalMoveZ(-.1f, .05f).OnComplete(()=>transform.DOLocalMoveZ(0,.2f));

        return true;

    }

    public void Throw()
    {
        if (!active)
            return;

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
        s.AppendCallback(() => transform.parent = null);
        s.AppendCallback(() => transform.position = Camera.main.transform.position + (Camera.main.transform.right * .1f));
        s.AppendCallback(() => ChangeSettings());
        s.AppendCallback(() => rb.AddForce(Camera.main.transform.forward * 10, ForceMode.Impulse));
        s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * 20, ForceMode.Impulse));
    }

    public void Pickup()
    {
        if (!active)
            return;

        SuperHotScript.instance.weaponscript = this;
        isPlayerWeapon = true;
        //ChangeSettings();

        transform.parent = SuperHotScript.weapon;

        transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
        transform.DOLocalRotate(Vector3.zero, .25f).SetUpdate(true);
    }

    public void Release()
    {
        active = true;
        transform.parent = null;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        collider.isTrigger = false;

        rb.AddForce((Camera.main.transform.position - transform.position) * 2, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 2, ForceMode.Impulse);

        isPlayerWeapon = false;

    } 

    IEnumerator Reload()
    {
        if (SuperHotScript.instance.weaponscript != this)
            yield break;
        SuperHotScript.instance.ReloadUI(reloadTime);
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Enemy") && collision.relativeVelocity.magnitude < 15)
        {
            BodyPartScript bp = collision.gameObject.GetComponent<BodyPartScript>();

            if (!bp.enemy.dead)
                Instantiate(SuperHotScript.instance.hitParticlePrefab, transform.position, transform.rotation);

            bp.HidePartAndReplace();
            bp.enemy.Ragdoll();
        }

    }

    public Vector3 movementVelocity;

    public void PollWeaponPhysics(ManipulationEventData evt)
    {
        if (SuperHotScript.instance != null && SuperHotScript.instance.weaponscript == this)
        {
            movementVelocity = evt.PointerVelocity;
        }
    }


}
