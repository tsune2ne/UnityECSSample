using Unity.Mathematics;
using UnityEngine;

public class OldMoveRotate : MonoBehaviour
{
    float Angle;
    float Speed;
    float3 BasePosition;

    private void Start()
    {
        Speed = UnityEngine.Random.Range(0.1f, 10f);
        BasePosition = transform.position;
    }

    private void Update()
    {
        Angle += Time.deltaTime * Speed;
        transform.position = new Vector3(BasePosition.x, Mathf.Cos(Angle) + BasePosition.y, BasePosition.z);
    }
}
