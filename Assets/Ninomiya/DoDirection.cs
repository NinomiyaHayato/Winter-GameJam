using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DoDirection : MonoBehaviour
{
    [SerializeField, Header("TitlePanel格納")] private GameObject[] _gamePanels;

    [SerializeField, Header("スタートImageの移動距離")] private float _moveDirection = 10;

    [SerializeField, Header("スタートImageを何秒で一ループさせるか")] private float _loopTime;

    [SerializeField, Header("夢と現実のロゴ入れ替える時間(消す)")] private float _changeTime;

    [SerializeField, Header("夢と現実のロゴ入れ替える時間(表示)")] private float _changeTime2;

    [SerializeField, Header("ReallyImageとDreamImage")] private Image[] _images;

    [SerializeField, Header("BackGroundのImageを格納")] private Image[] _backGrounds;

    [SerializeField, Header("現実と夢のImageを切り替えるFlag")] private bool _dreamChange;

    private int _erosionCount = 0; //浸食率の一時保存

    private int _count = 0;

    public void Action(Scenes scenes)
    {
        foreach(var panels in _gamePanels) { panels.SetActive(false); }
        _gamePanels[(int)scenes].SetActive(true);
    }

    public void ErosionTextChange(int erosionCount)
    {
        if(erosionCount == 0)
        {
            foreach(var backGrounds in _backGrounds) { backGrounds.enabled = false; }
            _backGrounds[_count].enabled = true;
        }
        if(erosionCount > _erosionCount && _count+ 1 < _backGrounds.Length)
        {
            _erosionCount = erosionCount;
            _backGrounds[_count].enabled = false;
            _count++;
            _backGrounds[_count].enabled = true;
        }
        else if(erosionCount < _erosionCount && _count - 1 >= 0)
        {
            _erosionCount = erosionCount;
            _backGrounds[_count].enabled = false;
            _count--;
            _backGrounds[_count].enabled = true;
        }

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

    public void  CountReset()
    {
        _count = 0;
    }
}
public enum Scenes
{
    TitleScene,
    GameScene,
    GameEndScene
}
