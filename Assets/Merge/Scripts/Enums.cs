using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームクリアもしくはミスした場合に結果を返すための列挙型
/// </summary>
public enum PlayResult
{
    Clear, // クリア条件達成
    Miss,  // ミスした
}

/// <summary>
/// ゲームの状態の遷移でカメラを切り替えるための列挙型
/// </summary>
public enum CameraType
{
    Title,
    InGame,
    Ending,
}

/// <summary>
/// 再生する音を指定する列挙型
/// </summary>
public enum AudioKey
{
    BGM_Reality,
    BGM_Dream,
    BGM_Ending,
    SE_Futon,
    SE_EnemyAttack,
}