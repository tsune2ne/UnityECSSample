using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct MoveRotate : IComponentData
{
    public float Angle;
    public float Speed;
    public float3 BasePosition;
    public bool IsInitialized;
}

public class MoveRotateAuthoring : MonoBehaviour
{
    public float Angle;
    public float Speed;

    class RotateBaker : Baker<MoveRotateAuthoring>
    {
        public override void Bake(MoveRotateAuthoring authoring)
        {
            // LocalTransformÉAÉäÇ≈EntityÇê∂ê¨
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveRotate
            {
                Angle = authoring.Angle,
                Speed = authoring.Speed,
            });
        }
    }
}
