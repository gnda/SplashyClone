using System;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public bool IsDisabled { get; set; }
    
    private void Update()
    {
        if (IsDisabled) return;
        var t = transform;
        var hInput = Vector3.right * Math.Sign(Input.GetAxis("Horizontal"));
        var mHInput = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 50;

        if (Input.GetAxis("Mouse X") != 0)
        {
            t.DOKill();
            transform.DOMoveX(t.position.x + mHInput, 0.1f);
            return;
        }

        if ((Input.GetAxis("Horizontal") == 0)) return;
        t.DOKill();
        transform.DOMove(t.position + hInput, 0.3f);
    }
}