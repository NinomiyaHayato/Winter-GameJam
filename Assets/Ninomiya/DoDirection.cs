using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DoDirection : MonoBehaviour
{
    [SerializeField, Header("StartText")] private Image _startImage;

    [SerializeField, Header("TitlePanelŠi”[")] private GameObject[] _gamePanels;

    [SerializeField, Header("ƒXƒ^[ƒgImage‚ÌˆÚ“®‹——£")] private float _moveDirection = 10;

    [SerializeField, Header("ƒXƒ^[ƒgImage‚ð‰½•b‚Åˆêƒ‹[ƒv‚³‚¹‚é‚©")] private float _loopTime;

    [SerializeField, Header("–²‚ÆŒ»ŽÀ‚ÌƒƒS“ü‚ê‘Ö‚¦‚éŽžŠÔ(Á‚·)")] private float _changeTime;

    [SerializeField, Header("–²‚ÆŒ»ŽÀ‚ÌƒƒS“ü‚ê‘Ö‚¦‚éŽžŠÔ(•\Ž¦)")] private float _changeTime2;

    [SerializeField, Header("ZH—¦‚ÌText")] private Text _erosionText;

    [SerializeField, Header("ReallyImage‚ÆDreamImage")] private Image[] _images;

    [SerializeField, Header("Œ»ŽÀ‚Æ–²‚ÌImage‚ðØ‚è‘Ö‚¦‚éFlag")] private bool _dreamChange;

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
        _erosionText.text = $"ZH—¦ : {erosionCount} %";
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
