using System.Collections;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private GameObject gemEffectPrefab; 
    public void Remove()
    {
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        var gemEffectGo = 
            Instantiate(gemEffectPrefab, 
                transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        Destroy(gameObject);
        yield return new WaitForSeconds(2.5f);
        Destroy(gemEffectGo);
    }
}
