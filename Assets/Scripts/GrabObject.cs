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

public class GrabObject : MonoBehaviour
{
    [SerializeField]
    private Handedness handedness;
    [SerializeField]
    private Transform trackedTransform;
    [SerializeField]
    private TrackedHandJoint joint = TrackedHandJoint.Palm;



    private void Awake()
    {
        if (trackedTransform == null)
            trackedTransform = this.transform;
    }

    private void Update()
    {
        if (trackedTransform == null)
            return;


        bool retrieved = false;

        var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        if (handJointService != null)
        {
            Transform jointTransform = handJointService.RequestJointTransform(joint, handedness);
            if (jointTransform != null)
            {
                trackedTransform.position = jointTransform.position;
                trackedTransform.rotation = jointTransform.rotation;
                retrieved = true;
                // ...
            }
        }
        
        if (!retrieved)
        if (HandJointUtils.TryGetJointPose(joint, handedness, out MixedRealityPose pose))
        {
            trackedTransform.position = pose.Position;
            trackedTransform.rotation = pose.Rotation;
        }
    }


    public void Attach(ManipulationEventData manipulationEventData)
    {
        handedness = manipulationEventData.Pointer.Controller.ControllerHandedness;
    }


    public void Detach()
    {
        handedness = Handedness.None;
    }

}
