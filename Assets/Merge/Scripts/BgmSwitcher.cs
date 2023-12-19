using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class BgmSwitcher : MonoBehaviour
{
    void Awake()
    {
        EntryPoint.OnTitleEnter += AudioPlayer.StopBGM;
        EntryPoint.OnPreInGameStart += () => AudioPlayer.PlayBGM(AudioKey.BGM_Reality);
        EntryPoint.OnEndingEnter += () => AudioPlayer.PlayBGM(AudioKey.BGM_Ending);
        Worldkeeper.OnDreamEnter += () => AudioPlayer.PlayBGM(AudioKey.BGM_Dream);
        Worldkeeper.OnRealityEnter += () => AudioPlayer.PlayBGM(AudioKey.BGM_Reality);

        this.OnDestroyAsObservable().Subscribe(_ => 
        {
            EntryPoint.OnTitleEnter -= AudioPlayer.StopBGM;
            EntryPoint.OnPreInGameStart -= () => AudioPlayer.PlayBGM(AudioKey.BGM_Reality);
            EntryPoint.OnEndingEnter -= () => AudioPlayer.PlayBGM(AudioKey.BGM_Ending);
            Worldkeeper.OnDreamEnter -= () => AudioPlayer.PlayBGM(AudioKey.BGM_Dream);
            Worldkeeper.OnRealityEnter -= () => AudioPlayer.PlayBGM(AudioKey.BGM_Reality);
        });
    }
}
