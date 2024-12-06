using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;

[BurstCompile]
public partial struct OptimizedSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }

    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);

        // ジョブの新しいインスタンスを作成し、必要なデータを割り当て、ジョブを並列してスケジュールします。
        new ProcessSpawnerJob
        {
            ElapsedTime = SystemAPI.Time.ElapsedTime,
            Ecb = ecb
        }.ScheduleParallel();
    }

    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }
}

[BurstCompile]
public partial struct ProcessSpawnerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public double ElapsedTime;

    // IJobEntityは、その`Execute`メソッドのパラメータに基づいて、コンポーネントデータクエリを生成します。
    // この例では、すべてのSpawnerコンポーネントをクエリし、`ref`を使用して、操作に読み取りと書き込みのアクセスが必要であることを指定します。
    // Unityは、コンポーネントデータクエリに一致する各エンティティに対して`Execute`を処理します。

    private void Execute([ChunkIndexInQuery] int chunkIndex, ref Spawner spawner)
    {
        // 次のスポーン時間が経過している場合
        if (spawner.NextSpawnTime < ElapsedTime)
        {
            // 新しいエンティティを生成し、スポナーに配置します。
            Entity newEntity = Ecb.Instantiate(chunkIndex, spawner.Prefab);
            Ecb.SetComponent(chunkIndex, newEntity, LocalTransform.FromPosition(spawner.SpawnPosition));

            // 次のスポーン時間をリセットします。
            spawner.NextSpawnTime = (float)ElapsedTime + spawner.SpawnRate;
        }
    }
}
