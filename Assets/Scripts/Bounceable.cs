using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Bounceable : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float movementDuration;
    private bool _isMoving;
    private Vector3 _previousPosition;
    private GameManager _gameManager;

    private void Start()
    {
        _previousPosition = transform.position;
        _gameManager = FindObjectOfType<GameManager>();
        targetPosition += _previousPosition;
    }
    
    private void Update()
    {
        if (_isMoving) return;
        StartCoroutine(BounceCoroutine());
    }

    private IEnumerator BounceCoroutine()
    {
        _isMoving = true;
        transform.DOMoveY(targetPosition.y, movementDuration).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(movementDuration);
        transform.DOMoveY(_previousPosition.y, movementDuration).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(movementDuration);
        if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo))
        {
            var otherGo = hitInfo.collider.gameObject;
            _gameManager.HandleCollision(gameObject, otherGo, hitInfo.point);
        }
        else
        {
            _gameManager.Lose();
        }
        _isMoving = false;
    }
}