using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UltimateXR.Animation.Transforms;
using UltimateXR.Avatar;
using UltimateXR.Avatar.Controllers;
using UltimateXR.Core;
using UltimateXR.Core.StateSync;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class SG_UXRAvatarNetworked : MonoBehaviourPun,IPunObservable
{
    public UxrAvatar avatar;
    private void Awake()
    {
        avatar.AvatarMode = photonView.IsMine ? UxrAvatarMode.Local : UxrAvatarMode.UpdateExternally;
        if (avatar.AvatarMode == UxrAvatarMode.UpdateExternally)
        {
            avatar.CameraComponent.enabled = false;
            avatar.CameraComponent.GetComponent<TrackedPoseDriver>().enabled = false;
            avatar.CameraComponent.GetComponent<AudioListener>().enabled = false;
            avatar.CameraController.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        avatar.StateChanged += StateChanged;
        UxrManager.AvatarsUpdated += UxrManagerOnAvatarsUpdated;
    }
    private void OnDisable()
    {
        avatar.StateChanged -= StateChanged;
        UxrManager.AvatarsUpdated -= UxrManagerOnAvatarsUpdated;
    }

    private void LateUpdate()
    {
        if (avatar.AvatarController is UxrStandardAvatarController controller && photonView.IsMine)
        {
            if (photonView == null)
            {
                controller.gameObject.GetPhotonView();
            }
            photonView.RPC("UpdateIkRPC",RpcTarget.Others);
        }
    }

    private void StateChanged(object sender, UxrStateSyncEventArgs e)
    {
        if (photonView.IsMine)
        {
            if (!(e is UxrAvatarSyncEventArgs syncArgs))
            {
                return;
            }

            var vars = new object[4];
            switch (syncArgs.EventType)
            {
                case UxrAvatarSyncEventType.AvatarMove:
                    //do nothing
                    break;
                case UxrAvatarSyncEventType.HandPose:
                    vars[0] = syncArgs.HandPoseChangeEventArgs.HandSide.ToString();
                    vars[1] = syncArgs.HandPoseChangeEventArgs.PoseName;
                    vars[2] = syncArgs.HandPoseChangeEventArgs.BlendValue;
                    vars[3] = true;
                    photonView.RPC("HandPoseRPC",RpcTarget.Others,vars);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    private void UxrManagerOnAvatarsUpdated()
    {
        
    }

    [PunRPC]
    public void HandPoseRPC(object[] vars)
    {
        if (vars == null || vars.Length == 0 )
        {
            return;
        } 
        
        Enum.TryParse((string)vars[0], out UxrHandSide handSide);

       avatar.SetCurrentHandPose(handSide, (string)vars[1], (float)vars[2], (bool)vars[3]);
    }
    
    [PunRPC]
    public void UpdateIkRPC()
    {
        if (avatar.AvatarController is UxrStandardAvatarController controller)
        {
            Debug.Log(controller.name + ": Getting IK prompt");
            controller.SolveBodyIK();
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
