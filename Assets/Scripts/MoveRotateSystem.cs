using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct MoveRotateSystem : ISystem
{
    /*
    public void OnUpdate(ref SystemState state)
    {
        // MoveRotateとLocalTransformが付与されてるEntityを検索して処理
        foreach (var (rotate, xform) in SystemAPI.Query<
                RefRW<MoveRotate>,
                RefRW<LocalTransform>>())
        {
            // 初期化処理
            if (!rotate.ValueRW.IsInitialized)
            {
                rotate.ValueRW.Speed = Random.Range(0.1f, 10f);
                rotate.ValueRW.BasePosition = xform.ValueRO.Position;
                rotate.ValueRW.IsInitialized = true;
            }

            // 移動処理
            rotate.ValueRW.Angle += Time.deltaTime * rotate.ValueRW.Speed;
            xform.ValueRW.Position.y = Mathf.Cos(rotate.ValueRW.Angle) + rotate.ValueRW.BasePosition.y;
        }
    }
    //*/

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 初期化処理
        foreach (var (rotate, xform) in SystemAPI.Query<
            RefRW<MoveRotate>,
            RefRW<LocalTransform>>())
        {
            if (!rotate.ValueRW.IsInitialized)
            {
                // UnityEngine.Randomはシングルスレッド限定なので先に決めてしまう
                rotate.ValueRW.Speed = Random.Range(0.1f, 10f);
                rotate.ValueRW.BasePosition = xform.ValueRO.Position;
                rotate.ValueRW.IsInitialized = true;
            }
        }

        // ジョブインスタンスを生成して並列実行予約
        var ecb = GetEntityCommandBuffer(ref state);
        new ProcessRotateMoveJob
        {
            DeltaTime = Time.deltaTime,
            Ecb = ecb
        }.ScheduleParallel();
    }

    EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }

    [BurstCompile]
    public partial struct ProcessRotateMoveJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter Ecb;

        // 引数のComponentを持つEntityを検索して実行される
        void Execute([ChunkIndexInQuery] int chunkIndex, ref MoveRotate rotate, ref LocalTransform xform)
        {
            rotate.Angle += DeltaTime * rotate.Speed;
            xform.Position.y = Mathf.Cos(rotate.Angle) + rotate.BasePosition.y;
        }
    }
    //*/
}
