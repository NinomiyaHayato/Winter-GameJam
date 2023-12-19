using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DoDirection : MonoBehaviour
{
    [SerializeField, Header("StartText")] private Image _startImage;

    [SerializeField, Header("TitlePanel格納")] private GameObject[] _gamePanels;

    [SerializeField, Header("スタートImageの移動距離")] private float _moveDirection = 10;

    [SerializeField, Header("スタートImageを何秒で一ループさせるか")] private float _loopTime;

    [SerializeField, Header("浸食率のText")] private Text _erosionText;

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
        _erosionText.text = $"浸食率 : {erosionCount} %";
    }
}
public enum Scenes
{
    TitleScene,
    GameScene,
    GameEndScene
}
