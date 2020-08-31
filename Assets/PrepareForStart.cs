using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareForStart : MonoBehaviour
{

    [SerializeField]
    private GameObject[] movable;

    private Dictionary<GameObject, pState> movableState = new Dictionary<GameObject, pState>();

    struct pState 
    {
        public bool inited;
        public bool isKinematic;
        public bool useGravity;
        public RigidbodyInterpolation interpolation;

    }

    // Start is called before the first frame update
    void Start()
    {
        Prepare();
    }

    public void Prepare()
    {
        foreach (GameObject o in movable)
        {
            pState pS = new pState();
            Rigidbody r = o.GetComponent<Rigidbody>();

            if (r != null)
            {
                pS.inited = true;
                pS.isKinematic = r.isKinematic;
                pS.useGravity = r.useGravity;
                pS.interpolation = r.interpolation;

                r.isKinematic = true;
                r.useGravity = false;

            }

            movableState.Add(o, pS);

        }
    }

    public void Restore()
    {
        foreach (var k in movableState)
        {
            if (k.Value.inited)
            {
                var o = k.Key;
                Rigidbody r = o.GetComponent<Rigidbody>();

                r.isKinematic = k.Value.isKinematic;
                r.useGravity = k.Value.useGravity;
            }

            var c = k.Key.GetComponent<ManipulationHandler>();
            Destroy(c);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
