using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool IsLast { get; set; }

    public void Remove()
    {
        var bonusTarget = transform.Find("BonusTarget");
        if (bonusTarget != null)
        {
            bonusTarget.DOScaleX(0, 0.3f).SetEase(Ease.Linear);
            bonusTarget.DOScaleZ(0, 0.3f).SetEase(Ease.Linear);
        }
        StartCoroutine(RemoveCoroutine());
    }
    
    private IEnumerator RemoveCoroutine()
    {
        transform.DOMoveY(1, 0.4f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(0.4f);
        transform.DOMoveY(-5, 0.8f).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(0.8f);
    }
}
