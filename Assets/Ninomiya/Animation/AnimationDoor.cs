using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDoor : MonoBehaviour
{
    Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Const.PlayerTag)
        {
            _anim.SetTrigger("Door");
        }
    }
}
