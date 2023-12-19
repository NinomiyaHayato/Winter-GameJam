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

    [SerializeField, Header("�Z�H����Text")] private Text _erosionText;

    private void Start()
    {
        StartImageMove();
        ErosionTextChange(0);
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
}
public enum Scenes
{
    TitleScene,
    GameScene,
    GameEndScene
}
