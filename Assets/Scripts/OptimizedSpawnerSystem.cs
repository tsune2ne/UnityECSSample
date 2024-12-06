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

        // �W���u�̐V�����C���X�^���X���쐬���A�K�v�ȃf�[�^�����蓖�āA�W���u����񂵂ăX�P�W���[�����܂��B
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

    // IJobEntity�́A����`Execute`���\�b�h�̃p�����[�^�Ɋ�Â��āA�R���|�[�l���g�f�[�^�N�G���𐶐����܂��B
    // ���̗�ł́A���ׂĂ�Spawner�R���|�[�l���g���N�G�����A`ref`���g�p���āA����ɓǂݎ��Ə������݂̃A�N�Z�X���K�v�ł��邱�Ƃ��w�肵�܂��B
    // Unity�́A�R���|�[�l���g�f�[�^�N�G���Ɉ�v����e�G���e�B�e�B�ɑ΂���`Execute`���������܂��B

    private void Execute([ChunkIndexInQuery] int chunkIndex, ref Spawner spawner)
    {
        // ���̃X�|�[�����Ԃ��o�߂��Ă���ꍇ
        if (spawner.NextSpawnTime < ElapsedTime)
        {
            // �V�����G���e�B�e�B�𐶐����A�X�|�i�[�ɔz�u���܂��B
            Entity newEntity = Ecb.Instantiate(chunkIndex, spawner.Prefab);
            Ecb.SetComponent(chunkIndex, newEntity, LocalTransform.FromPosition(spawner.SpawnPosition));

            // ���̃X�|�[�����Ԃ����Z�b�g���܂��B
            spawner.NextSpawnTime = (float)ElapsedTime + spawner.SpawnRate;
        }
    }
}
