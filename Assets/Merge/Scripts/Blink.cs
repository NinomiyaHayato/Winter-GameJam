using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Blink : MonoBehaviour
{
    [Header("瞬きのフェード演出の設定")]
    [SerializeField] Image _blickImage;
    [SerializeField] float _blickSpeed = 1.0f;

    void Awake()
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        Worldkeeper.OnDreamEnter += Run;
        Worldkeeper.OnRealityEnter += Run;
        this.OnDestroyAsObservable().Subscribe(_ => 
        {
            Worldkeeper.OnDreamEnter -= Run;
            Worldkeeper.OnRealityEnter -= Run;
        });

        void Run() => BlinkAsync(token).Forget();
    }

    async UniTask BlinkAsync(CancellationToken token)
    {
        await _blickImage.FadeOutAsync(_blickSpeed / 2, token);
        await _blickImage.FadeInAsync(_blickSpeed / 2, token);
    }
}
