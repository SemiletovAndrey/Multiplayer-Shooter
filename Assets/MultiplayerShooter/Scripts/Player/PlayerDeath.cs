using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private NetworkCharacterController2D _controller2D;
    [SerializeField] private ShotCharacterController _shotCharacterController;
    public void Death()
    {
        _weaponTransform.gameObject.SetActive(false);
        _controller2D.enabled = false;
        _shotCharacterController.enabled = false;
    }
}
