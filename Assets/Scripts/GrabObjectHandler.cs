using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Linq;

public class GrabObjectHandler : MonoBehaviour, IMixedRealityPointerHandler
{

    [Header("Editable Properties")]
    [SerializeField]
    private GameObject handlerPrefab;

    [SerializeField]
    private float forceMultiplier = 10;


    #region Event handlers
    [Header("Manipulation Events")]
    [SerializeField]
    private ManipulationEvent onManipulationStarted = new ManipulationEvent();

    public ManipulationEvent OnManipulationStarted
    {
        get => onManipulationStarted;
        set => onManipulationStarted = value;
    }

    [SerializeField]
    private ManipulationEvent onManipulationEnded = new ManipulationEvent();

    public ManipulationEvent OnManipulationEnded
    {
        get => onManipulationEnded;
        set => onManipulationEnded = value;
    }


    [SerializeField]
    private ManipulationEvent onManipulationMove= new ManipulationEvent();

    public ManipulationEvent OnManipulationMove
    {
        get => onManipulationMove;
        set => onManipulationMove = value;
    }


    #endregion


    #region Private Properties
    /// <summary>
    /// Holds the pointer and the initial intersection point of the pointer ray 
    /// with the object on pointer down in pointer space
    /// </summary>
    private struct PointerData
    {
        public IMixedRealityPointer pointer;
        private Vector3 initialGrabPointInPointer;

        public PointerData(IMixedRealityPointer pointer, Vector3 worldGrabPoint) : this()
        {
            this.pointer = pointer;
            this.initialGrabPointInPointer = Quaternion.Inverse(pointer.Rotation) * (worldGrabPoint - pointer.Position);
        }

        public bool IsNearPointer => pointer is IMixedRealityNearPointer;

        /// Returns the grab point on the manipulated object in world space
        public Vector3 GrabPoint => (pointer.Rotation * initialGrabPointInPointer) + pointer.Position;
    }


    private Dictionary<uint, PointerData> pointerIdToPointerMap = new Dictionary<uint, PointerData>();

    bool isManipulationStarted;

    Rigidbody rb;

    #endregion

    #region Default Properties

    bool defaultUseGravity;
    bool defaultIsKinematic;
    RigidbodyInterpolation defaultInterpolation;

    #endregion

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
        defaultUseGravity = rb.useGravity;
        defaultIsKinematic = rb.isKinematic;
        defaultInterpolation = rb.interpolation;
    }

    private void Update()
    {

        if (handler != null && isManipulationStarted)
        {
            PointerData pointerData = GetFirstPointer();
            IMixedRealityPointer pointer = pointerData.pointer;

            // Calculate relative transform from object to grip.
            Quaternion gripRotation;
            TryGetGripRotation(pointer, out gripRotation);
            Quaternion worldToGripRotation = Quaternion.Inverse(gripRotation);
            Quaternion objectToGripRotation = worldToGripRotation * transform.rotation;

            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            MixedRealityPose hostPose = new MixedRealityPose(transform.position, transform.rotation);

            handler.transform.position = pointer.Position;
            handler.transform.rotation = pointer.Rotation;

        }


    }


    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {



        if (eventData.used || (eventData.Pointer as IMixedRealityNearPointer == null))
        {
            return;
        }

        if (eventData.Pointer is SpherePointer)
        {
            Debug.Log($"Grab start from {eventData.Pointer.PointerName}");
        }

        // If we only allow one handed manipulations, check there is no hand interacting yet. 
        if (pointerIdToPointerMap.Count == 0)
        {
            uint id = eventData.Pointer.PointerId;
            // Ignore poke pointer events
            if (!pointerIdToPointerMap.ContainsKey(id))
            {
                // cache start ptr grab point
                pointerIdToPointerMap.Add(id, new PointerData(eventData.Pointer, eventData.Pointer.Result.Details.Point));

                // Call manipulation started handlers

                //else if (IsOneHandedManipulationEnabled)
                {
                    if (!isManipulationStarted)
                    {
                        HandleManipulationStarted();
                    }
                    HandleOneHandMoveStarted();
                }
            }
        }

        if (pointerIdToPointerMap.Count > 0)
        {
            // Always mark the pointer data as used to prevent any other behavior to handle pointer events
            // as long as the ObjectManipulator is active.
            // This is due to us reacting to both "Select" and "Grip" events.
            eventData.Use();
        }
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        // Get pointer data before they are removed from the map
        Vector3 grabPoint = GetPointersGrabPoint();
        Vector3 velocity = GetPointersVelocity();
        Vector3 angularVelocity = GetPointersAngularVelocity();

        uint id = eventData.Pointer.PointerId;
        if (pointerIdToPointerMap.ContainsKey(id))
        {
            pointerIdToPointerMap.Remove(id);
        }

        // Call manipulation ended handlers
        var handsPressedCount = pointerIdToPointerMap.Count;

        if (isManipulationStarted && handsPressedCount == 0)
        {
            HandleManipulationEnded(grabPoint, velocity, angularVelocity);
        }

        eventData.Use();
    }



    private void HandleOneHandMoveStarted()
    {
        /*
        PointerData pointerData = GetFirstPointer();
        IMixedRealityPointer pointer = pointerData.pointer;

        // Calculate relative transform from object to grip.
        Quaternion gripRotation;
        TryGetGripRotation(pointer, out gripRotation);
        Quaternion worldToGripRotation = Quaternion.Inverse(gripRotation);
        Quaternion objectToGripRotation = worldToGripRotation * transform.rotation;

        MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
        MixedRealityPose hostPose = new MixedRealityPose(transform.position, transform.rotation);
        //moveLogic.Setup(pointerPose, pointerData.GrabPoint, hostPose, HostTransform.localScale);
        */

    }

    GameObject handler;

    private void HandleManipulationStarted()
    {
        if (!isManipulationStarted)
        {

            PointerData pointerData = GetFirstPointer();
            IMixedRealityPointer pointer = pointerData.pointer;
            if (handlerPrefab == null)
                handler = new GameObject("handler");
            else
                handler = Instantiate<GameObject>(handlerPrefab);
            
            handler.transform.SetParent(transform.parent);
            handler.transform.position = pointer.Position;
            handler.transform.rotation = pointer.Rotation;
            transform.SetParent(handler.transform,true);


            rb.isKinematic = true;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.None;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

        }

        isManipulationStarted = true;
        //isNearManipulation = true;
        //isNearManipulation = IsNearManipulation();
        // TODO: If we are on HoloLens 1, push and pop modal input handler so that we can use old
        // gaze/gesture/voice manipulation. For HoloLens 2, we don't want to do this.
        if (OnManipulationStarted != null)
        {
            OnManipulationStarted.Invoke(new ManipulationEventData
            {
                ManipulationSource = gameObject,
                IsNearInteraction = true,
                Pointer = GetFirstPointer().pointer,
                PointerCentroid = GetPointersGrabPoint(),
                PointerVelocity = GetPointersVelocity(),
                PointerAngularVelocity = GetPointersAngularVelocity()
            });
        }
    }

    private void HandleManipulationEnded(Vector3 pointerGrabPoint, Vector3 pointerVelocity, Vector3 pointerAnglularVelocity)
    {
        if (handler != null)
        {
            transform.SetParent(handler.transform.parent);
            Destroy(handler);
            handler = null;

            rb.isKinematic = defaultIsKinematic;
            rb.useGravity = defaultUseGravity;
            rb.WakeUp();
            rb.velocity = pointerVelocity * forceMultiplier;
            rb.angularVelocity = pointerAnglularVelocity;
            rb.interpolation = defaultInterpolation;
        }

        isManipulationStarted = false;
        // TODO: If we are on HoloLens 1, push and pop modal input handler so that we can use old
        // gaze/gesture/voice manipulation. For HoloLens 2, we don't want to do this.
        if (OnManipulationEnded != null)
        {
            OnManipulationEnded.Invoke(new ManipulationEventData
            {
                ManipulationSource = gameObject,
                IsNearInteraction = true,
                PointerCentroid = pointerGrabPoint,
                PointerVelocity = pointerVelocity,
                PointerAngularVelocity = pointerAnglularVelocity
            });
        }

    }

    public void ForceEndManipulation()
    {
        // end manipulation
        if (isManipulationStarted)
        {
            HandleManipulationEnded(GetPointersGrabPoint(), GetPointersVelocity(), GetPointersAngularVelocity());
        }
        pointerIdToPointerMap.Clear();
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        if (handler != null)
        {
            OnManipulationMove.Invoke(new ManipulationEventData
            {
                ManipulationSource = gameObject,
                IsNearInteraction = true,
                Pointer = GetFirstPointer().pointer,
                PointerCentroid = GetPointersGrabPoint(),
                PointerVelocity = GetPointersVelocity(),
                PointerAngularVelocity = GetPointersAngularVelocity()
            });
        }
        //throw new System.NotImplementedException();
        /*
        if (handler != null)
        {
            PointerData pointerData = GetFirstPointer();
            IMixedRealityPointer pointer = pointerData.pointer;

            // Calculate relative transform from object to grip.
            Quaternion gripRotation;
            TryGetGripRotation(pointer, out gripRotation);
            Quaternion worldToGripRotation = Quaternion.Inverse(gripRotation);
            Quaternion objectToGripRotation = worldToGripRotation * transform.rotation;

            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            MixedRealityPose hostPose = new MixedRealityPose(transform.position, transform.rotation);

            handler.transform.position = pointer.Position;
            handler.transform.rotation = pointer.Rotation;

        }
        */
    }


    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }


    private Vector3 GetPointersGrabPoint()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (var p in pointerIdToPointerMap.Values)
        {
            sum += p.GrabPoint;
            count++;
        }
        return sum / Math.Max(1, count);
    }

    private MixedRealityPose GetPointersPose()
    {
        Vector3 sumPos = Vector3.zero;
        Vector3 sumDir = Vector3.zero;
        int count = 0;
        foreach (var p in pointerIdToPointerMap.Values)
        {
            sumPos += p.pointer.Position;
            sumDir += p.pointer.Rotation * Vector3.forward;
            count++;
        }

        return new MixedRealityPose
        {
            Position = sumPos / Math.Max(1, count),
            Rotation = Quaternion.LookRotation(sumDir / Math.Max(1, count))
        };
    }

    private Vector3 GetPointersVelocity()
    {
        Vector3 sum = Vector3.zero;
        int numControllers = 0;
        foreach (var p in pointerIdToPointerMap.Values)
        {
            // Check pointer has a valid controller (e.g. gaze pointer doesn't)
            if (p.pointer.Controller != null)
            {
                numControllers++;
                sum += p.pointer.Controller.Velocity;
            }
        }
        return sum / Math.Max(1, numControllers);
    }

    private Vector3 GetPointersAngularVelocity()
    {
        Vector3 sum = Vector3.zero;
        int numControllers = 0;
        foreach (var p in pointerIdToPointerMap.Values)
        {
            // Check pointer has a valid controller (e.g. gaze pointer doesn't)
            if (p.pointer.Controller != null)
            {
                numControllers++;
                sum += p.pointer.Controller.AngularVelocity;
            }
        }
        return sum / Math.Max(1, numControllers);
    }


    private PointerData GetFirstPointer()
    {
        // We may be able to do this without allocating memory.
        // Moving to a method for later investigation.
        return pointerIdToPointerMap.Values.First();
    }

    private bool TryGetGripRotation(IMixedRealityPointer pointer, out Quaternion rotation)
    {
        for (int i = 0; i < pointer.Controller.Interactions.Length; i++)
        {
            if (pointer.Controller.Interactions[i].InputType == DeviceInputType.SpatialGrip)
            {
                rotation = pointer.Controller.Interactions[i].RotationData;
                return true;
            }
        }
        rotation = Quaternion.identity;
        return false;
    }

}
