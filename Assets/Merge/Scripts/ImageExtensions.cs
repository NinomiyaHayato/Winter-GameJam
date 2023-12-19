using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public static class ImageExtensions
{
    public static async UniTask FadeInAsync(this Image image, float duration, CancellationToken token)
    {
        await image.FadeAsync(duration, 0, token);
    }

    public static async UniTask FadeOutAsync(this Image image, float duration, CancellationToken token)
    {
        await image.FadeAsync(duration, 1, token);
    }

    static async UniTask FadeAsync(this Image image, float duration, float endAlpha, CancellationToken token)
    {
        float progress = 0;
        float startAlpha = 1 - endAlpha;
        while (progress < 1)
        {
            Color c = image.color;
            c.a = Mathf.Lerp(startAlpha, endAlpha, progress);
            image.color = c;

            progress += Time.deltaTime / duration;
            await UniTask.Yield(token);
        }
    }
}
