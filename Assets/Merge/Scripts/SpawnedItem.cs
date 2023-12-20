using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

public class SpawnedItem : MonoBehaviour
{
    [Header("�v���C���[�ƐڐG����")]
    [SerializeField] Collider _trigger;

    void Awake()
    {
        // �v���C���[�ƐڐG�����ۂ̓X�P�[����0�ɂ��邱�ƂŌ����Ȃ�����
        _trigger.OnTriggerEnterAsObservable()
            .Where(c => c.CompareTag(Const.PlayerTag))
            .Subscribe(_ => transform.localScale = Vector3.zero);

        #region ���g�p ���̐��E�ł̂ݎ��F���\��
        // ���̐��E�ł̂ݎ��F���\�ɁB
        //Worldkeeper.OnDreamEnter += ScaleOne;
        //Worldkeeper.OnRealityEnter += ScaleZero;
        //this.OnDestroyAsObservable().Subscribe(_ => 
        //{
        //    Worldkeeper.OnDreamEnter -= ScaleOne;
        //    Worldkeeper.OnRealityEnter -= ScaleZero;
        //});

        //void ScaleOne() => transform.localScale = Vector3.one;
        //void ScaleZero() => transform.localScale = Vector3.zero;
        #endregion
    }
}
