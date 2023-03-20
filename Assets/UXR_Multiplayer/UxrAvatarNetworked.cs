using System.Collections;
using System.Collections.Generic;
using UltimateXR.Avatar;
using UnityEngine;
using Photon.Pun;
using UltimateXR.Core.Components;
using UltimateXR.Core.StateSync;


public class UxrAvatarNetworked : UxrAvatar
{
    protected override void Start()
    {
        AvatarMode = GetComponent<PhotonView>().IsMine ? UxrAvatarMode.Local : UxrAvatarMode.UpdateExternally;
        base.Start();
    }
}