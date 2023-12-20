using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

public class SpawnedItem : MonoBehaviour
{
    [Header("プレイヤーと接触する")]
    [SerializeField] Collider _trigger;

    void Awake()
    {
        // プレイヤーと接触した際はスケールを0にすることで見えなくする
        _trigger.OnTriggerEnterAsObservable()
            .Where(c => c.CompareTag(Const.PlayerTag))
            .Subscribe(_ => transform.localScale = Vector3.zero);

        #region 未使用 夢の世界でのみ視認が可能に
        // 夢の世界でのみ視認が可能に。
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
