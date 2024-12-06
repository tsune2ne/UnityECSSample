using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine.UIElements;

/// <summary>
/// ECSのSystem
/// このスクリプトはアタッチをしなくても動作します。
/// 実行すると、指定した時間ごとにPrefabが生成されるているのがわかると思います。
/// </summary>
[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }

    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // すべてのSpawnerコンポーネントをクエリします。このシステムは、
        // コンポーネントから読み取りと書き込みを行う必要があるため、RefRWを使用します。
        // システムが読み取り専用のアクセスのみを必要とする場合は、RefROを使用します。
        foreach (RefRW<Spawner> spawner in SystemAPI.Query<RefRW<Spawner>>())
        {
            ProcessSpawner(ref state, spawner);
        }
        //UnityEngine.Debug.LogError("tsune: Spawner::OnUpdate");
    }

    private void ProcessSpawner(ref SystemState state, RefRW<Spawner> spawner)
    {
        // 次のスポーン時間が経過している場合    
        if (spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
        {
            // スポナーの持つプレファブを使って、新しいエンティティを生成します。
            Entity newEntity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
            // LocalPosition.FromPositionは、指定された位置で初期化されたTransformを返します。
            state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(spawner.ValueRO.SpawnPosition));

            // 次のスポーン時間をリセットします。
            spawner.ValueRW.NextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.SpawnRate;
        }
    }
}
