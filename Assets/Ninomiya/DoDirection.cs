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

    [SerializeField, Header("夢と現実のロゴ入れ替える時間(消す)")] private float _changeTime;

    [SerializeField, Header("夢と現実のロゴ入れ替える時間(表示)")] private float _changeTime2;

    [SerializeField, Header("浸食率のText")] private Text _erosionText;

    [SerializeField, Header("ReallyImageとDreamImage")] private Image[] _images;

    [SerializeField, Header("現実と夢のImageを切り替えるFlag")] private bool _dreamChange;

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
        _erosionText.text = $"浸食率 : {erosionCount} %";
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
