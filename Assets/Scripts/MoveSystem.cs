using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct MoveSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }

    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) 
    {
        foreach (var (move, xform) in SystemAPI.Query<RefRO<Move>, RefRW<LocalTransform>>())
        {
            ProcessMove(ref state, move, xform);
        }
    }

    private void ProcessMove(ref SystemState state, RefRO<Move> move, RefRW<LocalTransform> xform)
    {
        xform.ValueRW.Position.x = xform.ValueRW.Position.x + move.ValueRO.Speed;
    }
}
