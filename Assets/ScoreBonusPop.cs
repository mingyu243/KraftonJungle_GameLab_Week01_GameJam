using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBonusPop : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Text bonusText;
    [SerializeField] Image boxImage;

    public async UniTask ShowAsync(string text)
    {
        bonusText.text = text;
        canvasGroup.alpha = 0;

        boxImage.transform.DOLocalRotate(new Vector3(0, 0, -405f), 0.8f, RotateMode.FastBeyond360).ToUniTask().Forget();

        await UniTask.WhenAll(
            this.transform.DOLocalMoveY(this.transform.localPosition.y + 100f, 0.2f).ToUniTask(),
            canvasGroup.DOFade(1, 0.2f).ToUniTask()
        );

        await UniTask.WaitForSeconds(1.2f);
        await canvasGroup.DOFade(0, 0.15f).ToUniTask();

        Destroy(this.gameObject);
    }
}
