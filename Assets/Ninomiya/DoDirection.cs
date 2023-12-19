using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DoDirection : MonoBehaviour
{
    [SerializeField, Header("StartText")] private Image _startImage;

    [SerializeField, Header("TitlePanel�i�[")] private GameObject[] _gamePanels;

    [SerializeField, Header("�X�^�[�gImage�̈ړ�����")] private float _moveDirection = 10;

    [SerializeField, Header("�X�^�[�gImage�����b�ňꃋ�[�v�����邩")] private float _loopTime;

    [SerializeField, Header("���ƌ����̃��S����ւ��鎞��(����)")] private float _changeTime;

    [SerializeField, Header("���ƌ����̃��S����ւ��鎞��(�\��)")] private float _changeTime2;

    [SerializeField, Header("�Z�H����Text")] private Text _erosionText;

    [SerializeField, Header("ReallyImage��DreamImage")] private Image[] _images;

    [SerializeField, Header("�����Ɩ���Image��؂�ւ���Flag")] private bool _dreamChange;

    private void Start()
    {
        StartImageMove();
    }
    public void StartImageMove()
    {
        _startImage.transform.DOLocalMoveY(_moveDirection, _loopTime).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
    }

    public void Action(Scenes scenes)
    {
        foreach(var panels in _gamePanels) { panels.SetActive(false); }
        _gamePanels[(int)scenes].SetActive(true);
    }

    public void ErosionTextChange(int erosionCount)
    {
        _erosionText.text = $"�Z�H�� : {erosionCount} %";
    }

    public void ImageChange()
    {
        _dreamChange = !_dreamChange;

        if(_dreamChange)
        {
            _images[1].DOFade(0f, _changeTime).OnComplete(() =>
            {
                _images[0].DOFade(1f, _changeTime2).SetLink(gameObject);
            });
        }
        else
        {
            _images[0].DOFade(0f, _changeTime).OnComplete(() =>
            {
                _images[1].DOFade(1f, _changeTime2).SetLink(gameObject);
            });
        }
    }
}
public enum Scenes
{
    TitleScene,
    GameScene,
    GameEndScene
}
