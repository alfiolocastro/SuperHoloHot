using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class EnemyScript : MonoBehaviour
{
    Animator anim;
    public bool dead;
    public Transform weaponHolder;

    Transform player;
    public float activationRange = 6;

    public bool isActivated;


    public bool isWalking;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (weaponHolder.GetComponentInChildren<WeaponScript>() != null)
            weaponHolder.GetComponentInChildren<WeaponScript>().Equip(true);

        player = FindObjectOfType<Camera>().transform;

        targetPos = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
        previousPos = targetPos;

        transform.LookAt(targetPos);

        StartCoroutine(RandomAnimation());

        isActivated = false;

    }

    public void EquipWeapon(bool val)
    {
        if (weaponHolder.GetComponentInChildren<WeaponScript>() != null)
            weaponHolder.GetComponentInChildren<WeaponScript>().Equip(val);
    }

    Vector3 targetPos;
    Vector3 previousPos;

    private void FixedUpdate()
    {

        if (SuperHotScript.instance == null || !SuperHotScript.instance.gameObject.activeInHierarchy)
            return;

        if (!dead)
        {
            Quaternion rot = transform.rotation;

            targetPos = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);

            transform.LookAt(Vector3.Lerp(previousPos,targetPos,.2f));

            previousPos = targetPos;
        }

    }


    void Update()
    {

        if (SuperHotScript.instance == null || !SuperHotScript.instance.gameObject.activeInHierarchy)
            return;

        if (!dead)
        {



            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
            {
                //Debug.Log("is walking");
                transform.DOMove(targetPos, Vector3.Distance(transform.position, targetPos) * 2);
                isWalking = true;
            }
            else
            {
                if (isWalking)
                    transform.DOKill(false);
                isWalking = false;
            }

        }



    }

    public void Ragdoll()
    {
        anim.enabled = false;
        BodyPartScript[] parts = GetComponentsInChildren<BodyPartScript>();
        foreach (BodyPartScript bp in parts)
        {
            bp.rb.isKinematic = false;
            bp.rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        dead = true;

        if (weaponHolder.GetComponentInChildren<WeaponScript>() != null)
        {
            WeaponScript w = weaponHolder.GetComponentInChildren<WeaponScript>();
            w.Release();

        }
    }

    public void Shoot()
    {
        if (dead)
            return;

        if (SuperHotScript.instance == null || !SuperHotScript.instance.gameObject.activeSelf)
            return;

        if (weaponHolder.GetComponentInChildren<WeaponScript>() != null)
        {
            bool hasbullets = weaponHolder.GetComponentInChildren<WeaponScript>().Shoot(GetComponentInChildren<ParticleSystem>().transform.position, transform.rotation, true);
            if (!hasbullets)
            {
                //weaponHolder.GetComponentInChildren<WeaponScript>().Throw();
                anim.SetTrigger("walk");
                Debug.Log("start walking");
            }
        }
    }

    IEnumerator RandomAnimation()
    {
        while (SuperHotScript.instance == null || !SuperHotScript.instance.gameObject.activeSelf)
            yield return null;

        yield return new WaitForSecondsRealtime(Random.Range(1f, 3f));

        while (Vector3.Distance(transform.position, player.position) > activationRange)
            yield return null;

        while (true)
        {

            yield return null;
            // is in idle state

            if (dead)
            {
                continue;
            }



            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
            {
                continue;
            }

            yield return new WaitForSecondsRealtime(Random.Range(1f, 5f));

            anim.SetTrigger("shoot");

            yield return new WaitForSecondsRealtime(1f);

        }

        /*
        yield return null;
        anim.enabled = true;
        new WaitForSecondsRealtime(.1f);
        anim.enabled = false;

        while (SuperHotScript.instance == null || !SuperHotScript.instance.gameObject.activeSelf)
            yield return null;

        while (Vector3.Distance(transform.position, player.position) > activationRange)
            yield return null;

        isActivated = true;

        yield return new WaitForSecondsRealtime(Random.Range(1f, 6f));
        anim.enabled = true;
        */
    }
}
