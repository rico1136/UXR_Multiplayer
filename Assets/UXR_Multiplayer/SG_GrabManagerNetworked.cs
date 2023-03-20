using System;
using Photon.Pun;
using UltimateXR.Core.StateSync;
using UltimateXR.Manipulation;
using UnityEngine;

public class SG_GrabManagerNetworked : MonoBehaviour
{
    private PhotonView _pv;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        UxrGrabManager.Instance.StateChanged += StateChanged;
    }

    private void OnDisable()
    {
        UxrGrabManager.Instance.StateChanged -= StateChanged;
    }

    private void StateChanged(object sender, UxrStateSyncEventArgs e)
    {
        if (!(e is UxrManipulationSyncEventArgs syncArgs))
        {
            return;
        }

        var vars = new object[4];
        switch (syncArgs.EventType)
        {
            case UxrManipulationSyncEventType.Grab:
                var grabberViewGrab = syncArgs.EventArgs.Grabber.GetComponent<PhotonView>();
                vars[0] = grabberViewGrab.ViewID;
                vars[1] = syncArgs.EventArgs.GrabbableObject.GetComponent<PhotonView>().ViewID;
                vars[2] = syncArgs.EventArgs.GrabPointIndex;
                vars[3] = true;

                if (grabberViewGrab.IsMine)
                {
                    PhotonView.Find((int) vars[1]).RequestOwnership();
                    Debug.Log("Grab");
                
                    _pv.RPC("GrabObjectRPC", RpcTarget.OthersBuffered, vars);
                }
                
                break;

            case UxrManipulationSyncEventType.Release:
                var grabberViewRelease = syncArgs.EventArgs.Grabber.GetComponent<PhotonView>();
                vars[0] = grabberViewRelease.ViewID;
                vars[1] = syncArgs.EventArgs.GrabbableObject.GetComponent<PhotonView>().ViewID;
                vars[2] = true;

                if (grabberViewRelease.IsMine)
                {
                    Debug.Log("Release");
                    _pv.RPC("ReleaseObjectRPC", RpcTarget.OthersBuffered, vars);
                }

                break;

            case UxrManipulationSyncEventType.Place:
                var grabberAnchor = syncArgs.EventArgs.GrabbableAnchor.GetComponent<PhotonView>();
                var grabbableObject = syncArgs.EventArgs.GrabbableObject.GetComponent<PhotonView>();
                vars[0] = grabberAnchor.ViewID;
                vars[1] = grabbableObject.ViewID;
                vars[2] = syncArgs.EventArgs.PlacementOptions.ToString();
                vars[3] = true;
                
                if (grabbableObject.IsMine)
                {
                    Debug.Log("Place");
                    _pv.RPC("PlaceObjectRPC", RpcTarget.OthersBuffered, vars);
                }
                break;

            case UxrManipulationSyncEventType.Remove:
                vars[0] = syncArgs.EventArgs.GrabbableObject.GetComponent<PhotonView>().ViewID;
                vars[1] = true;

                if (PhotonView.Find((int) vars[0]).IsMine)
                {
                    Debug.Log("Remove anchor");
                    _pv.RPC("RemoveObjectFromAnchorRPC", RpcTarget.OthersBuffered, vars);
                }

                break;
        }
    }

    [PunRPC]
    public void GrabObjectRPC(object[] vars)
    {
        UxrGrabber grabber = null;
        UxrGrabbableObject grabbable = null;
        if (vars[0] != null)
        {
            grabber = PhotonView.Find((int) vars[0]).GetComponent<UxrGrabber>();
        }

        if (vars[1] != null)
        {
            grabbable = PhotonView.Find((int) vars[1]).GetComponent<UxrGrabbableObject>();
        }

        if (grabber == null || grabbable == null) return;

        UxrGrabManager.Instance.GrabObject(grabber, grabbable, (int) vars[2], (bool) vars[3]);

        Debug.Log("Grab RPC");
    }

    [PunRPC]
    public void ReleaseObjectRPC(object[] vars)
    {
        UxrGrabber grabber = null;
        UxrGrabbableObject grabbable = null;
        if (vars[0] != null)
        {
            grabber = PhotonView.Find((int) vars[0]).GetComponent<UxrGrabber>();
        }

        if (vars[1] != null)
        {
            grabbable = PhotonView.Find((int) vars[1]).GetComponent<UxrGrabbableObject>();
        }

        if (grabber == null || grabbable == null) return;
        
        UxrGrabManager.Instance.ReleaseObject(grabber, grabbable, (bool) vars[2]);
        Debug.Log("ReleasObject RPC");
    }

    [PunRPC]
    public void PlaceObjectRPC(object[] vars)
    {
        UxrGrabbableObjectAnchor anchor = null;
        UxrGrabbableObject grabbable = null;
        if (vars[0] != null)
        {
            anchor = PhotonView.Find((int)vars[0]).GetComponent<UxrGrabbableObjectAnchor>();
        }

        if (vars[1] != null)
        {
            grabbable = PhotonView.Find((int)vars[1]).GetComponent<UxrGrabbableObject>();
        }
        
        Enum.TryParse((string)vars[2], out UxrPlacementOptions placementType);

        if (anchor == null || grabbable == null) return;
        UxrGrabManager.Instance.PlaceObject( grabbable,anchor,placementType, (bool) vars[3]);
        Debug.Log("Place object RPC");
    }

    [PunRPC]
    public void RemoveObjectFromAnchorRPC(object[] vars)
    {
        UxrGrabbableObject grabbable = null;
        
        if (vars[0] != null)
        {
            grabbable = PhotonView.Find((int) vars[0]).GetComponent<UxrGrabbableObject>();
        }

        if (grabbable == null) return;

        UxrGrabManager.Instance.RemoveObjectFromAnchor(grabbable, (bool)vars[1]);
        Debug.Log("Remove anchor RPC");
    }
}