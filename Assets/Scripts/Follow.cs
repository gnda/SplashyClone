using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform targetTransform;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private bool useXAxis, useYAxis, useZAxis;
    
    private void Update()
    {
        transform.position = positionOffset + new Vector3(
            (useXAxis ? targetTransform.position.x : transform.position.x), 
            (useYAxis ? targetTransform.position.y : transform.position.y), 
            (useZAxis ? targetTransform.position.z : transform.position.z)
        );
    }
}