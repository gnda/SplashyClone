using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScorePoint : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        transform.DOMoveY(transform.position.y + 2, 0.5f).SetEase(Ease.Linear);
        GetComponent<TMP_Text>().DOFade(0, 1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}