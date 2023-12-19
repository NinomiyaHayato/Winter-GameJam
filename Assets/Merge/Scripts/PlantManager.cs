using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Threading;
using UnityEngine.UI;
using System.Linq;

public class PlantManager : MonoBehaviour
{
    #region 植物のオブジェクト。「表示」と「隠す」の2つの状態を切り替える
    class PlantObject
    {
        // インスペクタから設定させることも可能
        const float GrowthSpeed = 1.0f;

        Transform _transform;
        Vector3 _defaultScale;
        float _progress;

        public PlantObject(Transform transform)
        {
            _transform = transform;
            _defaultScale = transform.localScale;
        }

        // deltaTimeと乗算して1以上になる数を渡してスケールを1にする
        public void Show() => TweenScale(65535);
        // deltaTimeと乗算して0以下になる数を渡してスケールを0にする
        public void Hide() => TweenScale(-65535);

        public void Growth() => TweenScale(GrowthSpeed);
        public void Wither() => TweenScale(-GrowthSpeed);

        void TweenScale(float f)
        {
            _progress += Time.deltaTime * f;
            _progress = Mathf.Clamp01(_progress);
            _transform.localScale = Vector3.Lerp(Vector3.zero, _defaultScale, _progress);
        }
    }
    #endregion

    #region 進捗度のクラス。一定範囲の値をとるfloat型
    class Progress
    {
        public const float Max = 100.0f;

        float _value = new();
        public float Value
        {
            get => _value;
            set
            {
                if (!Valid) return;

                _value = value;
                _value = Mathf.Clamp(_value, 0, Max);
            }
        }

        /// <summary>
        /// このフラグがfalseの間は、値の変更が出来なくなる
        /// </summary>
        public bool Valid = true;

        /// <summary>
        /// 0から1で表した割合
        /// </summary>
        public float Percent => _value / Max;
    }
    #endregion

    public struct Message { }

    [Header("UIを操作する")]
    [SerializeField] DoDirection _uiController;
    [Header("ヒエラルキー上に配置した植物")]
    [SerializeField] Transform[] _plants;
    [Header(Const.PreColorTag + "1回の進捗で増加する量" + Const.SufColorTag)]
    [SerializeField] float _progressPower = 5.0f;
    [Header(Const.PreColorTag + "自然減少量" + Const.SufColorTag)]
    [SerializeField] float _deltaProgress = 1.0f;

    Progress _progress = new();

    public float CurrentProgress => _progress.Value;

    void Awake()
    {
        // StepProgressメソッドが呼ばれるたびに進捗が増加する
        MessageBroker.Default.Receive<Message>()
            .Subscribe(_ => _progress.Value += _progressPower).AddTo(this);

        PlantObject[] plants = _plants.Select(t => new PlantObject(t)).ToArray();     

        // インゲーム開始とリセットのタイミングでオブジェクトを全部画面から隠す
        EntryPoint.OnPreInGameStart += HideAll;
        EntryPoint.OnInGameReset += HideAll;

        this.OnDestroyAsObservable().Subscribe(_ => 
        {
            EntryPoint.OnPreInGameStart -= HideAll;
            EntryPoint.OnInGameReset -= HideAll;
        });

        HideAll();
        // 進捗度に応じたスケールを毎フレーム表示するだけなので、インゲームを跨いで処理し続けても大丈夫
        UpdateAsync(_progress, plants, this.GetCancellationTokenOnDestroy()).Forget();

        // 進捗をリセットして植物のオブジェクトを全部隠す
        void HideAll()
        {
            // UIを初期化
            if (_uiController != null) _uiController.ErosionTextChange(0);

            _progress.Value = 0;
            foreach (PlantObject p in plants) p.Hide();
        }
    }

    // 毎フレーム、プレイヤーが動いている間は進捗が減少していく
    async UniTask UpdateAsync(Progress progress, PlantObject[] plants, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (progress.Valid)
            {
                progress.Value -= Time.deltaTime * _deltaProgress;
                Reflection(ref plants, progress.Percent);
                Print(progress.Percent);
            }

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    // 進捗度の割合だけ植物のオブジェクトのスケールを徐々に変化させていく
    void Reflection(ref PlantObject[] plants, float percent)
    {
        int p = (int)(percent * _plants.Length);
        for (int i = 0; i < _plants.Length; i++)
        {
            if (i < p) plants[i].Growth();
            else plants[i].Wither();
        }
    }

    // UIに0から100%で表示
    void Print(float percent)
    {
        _uiController.ErosionTextChange(Mathf.CeilToInt(percent * 100));
    }

    /// <summary>
    /// 植物の成長の進捗を進める。
    /// 自分自身に対してメッセージを送信することで、外部からはstaticクラスのようにアクセスできる。
    /// </summary>
    public static void StepProgress()
    {
        MessageBroker.Default.Publish(new Message());
    }
}
