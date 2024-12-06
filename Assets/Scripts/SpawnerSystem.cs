using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine.UIElements;

/// <summary>
/// ECS��System
/// ���̃X�N���v�g�̓A�^�b�`�����Ȃ��Ă����삵�܂��B
/// ���s����ƁA�w�肵�����Ԃ��Ƃ�Prefab�����������Ă���̂��킩��Ǝv���܂��B
/// </summary>
[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }

    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // ���ׂĂ�Spawner�R���|�[�l���g���N�G�����܂��B���̃V�X�e���́A
        // �R���|�[�l���g����ǂݎ��Ə������݂��s���K�v�����邽�߁ARefRW���g�p���܂��B
        // �V�X�e�����ǂݎ���p�̃A�N�Z�X�݂̂�K�v�Ƃ���ꍇ�́ARefRO���g�p���܂��B
        foreach (RefRW<Spawner> spawner in SystemAPI.Query<RefRW<Spawner>>())
        {
            ProcessSpawner(ref state, spawner);
        }
        //UnityEngine.Debug.LogError("tsune: Spawner::OnUpdate");
    }

    private void ProcessSpawner(ref SystemState state, RefRW<Spawner> spawner)
    {
        // ���̃X�|�[�����Ԃ��o�߂��Ă���ꍇ    
        if (spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
        {
            // �X�|�i�[�̎��v���t�@�u���g���āA�V�����G���e�B�e�B�𐶐����܂��B
            Entity newEntity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
            // LocalPosition.FromPosition�́A�w�肳�ꂽ�ʒu�ŏ��������ꂽTransform��Ԃ��܂��B
            state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(spawner.ValueRO.SpawnPosition));

            // ���̃X�|�[�����Ԃ����Z�b�g���܂��B
            spawner.ValueRW.NextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.SpawnRate;
        }
    }
}
