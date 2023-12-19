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
    #region �A���̃I�u�W�F�N�g�B�u�\���v�Ɓu�B���v��2�̏�Ԃ�؂�ւ���
    class PlantObject
    {
        // �C���X�y�N�^����ݒ肳���邱�Ƃ��\
        const float GrowthSpeed = 1.0f;

        Transform _transform;
        Vector3 _defaultScale;
        float _progress;

        public PlantObject(Transform transform)
        {
            _transform = transform;
            _defaultScale = transform.localScale;
        }

        // deltaTime�Ə�Z����1�ȏ�ɂȂ鐔��n���ăX�P�[����1�ɂ���
        public void Show() => TweenScale(65535);
        // deltaTime�Ə�Z����0�ȉ��ɂȂ鐔��n���ăX�P�[����0�ɂ���
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

    #region �i���x�̃N���X�B���͈͂̒l���Ƃ�float�^
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
        /// ���̃t���O��false�̊Ԃ́A�l�̕ύX���o���Ȃ��Ȃ�
        /// </summary>
        public bool Valid = true;

        /// <summary>
        /// 0����1�ŕ\��������
        /// </summary>
        public float Percent => _value / Max;
    }
    #endregion

    public struct Message { }

    [Header("UI�𑀍삷��")]
    [SerializeField] DoDirection _uiController;
    [Header("�q�G�����L�[��ɔz�u�����A��")]
    [SerializeField] Transform[] _plants;
    [Header(Const.PreColorTag + "1��̐i���ő��������" + Const.SufColorTag)]
    [SerializeField] float _progressPower = 5.0f;
    [Header(Const.PreColorTag + "���R������" + Const.SufColorTag)]
    [SerializeField] float _deltaProgress = 1.0f;

    Progress _progress = new();

    public float CurrentProgress => _progress.Value;

    void Awake()
    {
        // StepProgress���\�b�h���Ă΂�邽�тɐi������������
        MessageBroker.Default.Receive<Message>()
            .Subscribe(_ => _progress.Value += _progressPower).AddTo(this);

        PlantObject[] plants = _plants.Select(t => new PlantObject(t)).ToArray();     

        // �C���Q�[���J�n�ƃ��Z�b�g�̃^�C�~���O�ŃI�u�W�F�N�g��S����ʂ���B��
        EntryPoint.OnPreInGameStart += HideAll;
        EntryPoint.OnInGameReset += HideAll;

        this.OnDestroyAsObservable().Subscribe(_ => 
        {
            EntryPoint.OnPreInGameStart -= HideAll;
            EntryPoint.OnInGameReset -= HideAll;
        });

        HideAll();
        // �i���x�ɉ������X�P�[���𖈃t���[���\�����邾���Ȃ̂ŁA�C���Q�[�����ׂ��ŏ����������Ă����v
        UpdateAsync(_progress, plants, this.GetCancellationTokenOnDestroy()).Forget();

        // �i�������Z�b�g���ĐA���̃I�u�W�F�N�g��S���B��
        void HideAll()
        {
            // UI��������
            if (_uiController != null) _uiController.ErosionTextChange(0);

            _progress.Value = 0;
            foreach (PlantObject p in plants) p.Hide();
        }
    }

    // ���t���[���A�v���C���[�������Ă���Ԃ͐i�����������Ă���
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

    // �i���x�̊��������A���̃I�u�W�F�N�g�̃X�P�[�������X�ɕω������Ă���
    void Reflection(ref PlantObject[] plants, float percent)
    {
        int p = (int)(percent * _plants.Length);
        for (int i = 0; i < _plants.Length; i++)
        {
            if (i < p) plants[i].Growth();
            else plants[i].Wither();
        }
    }

    // UI��0����100%�ŕ\��
    void Print(float percent)
    {
        _uiController.ErosionTextChange(Mathf.CeilToInt(percent * 100));
    }

    /// <summary>
    /// �A���̐����̐i����i�߂�B
    /// �������g�ɑ΂��ă��b�Z�[�W�𑗐M���邱�ƂŁA�O�������static�N���X�̂悤�ɃA�N�Z�X�ł���B
    /// </summary>
    public static void StepProgress()
    {
        MessageBroker.Default.Publish(new Message());
    }
}
