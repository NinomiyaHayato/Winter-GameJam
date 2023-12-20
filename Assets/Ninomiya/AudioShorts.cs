using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioShort : MonoBehaviour
{
    [SerializeField] AudioKey _key;

    [SerializeField, Header("�����܂ł̑؍ݎ���")] private float _destroyTime = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Const.PlayerTag)
        {
            AudioPlayer.PlaySE(_key);
            Destroy(gameObject, _destroyTime);
        }
    }
}

