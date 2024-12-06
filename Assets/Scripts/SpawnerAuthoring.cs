using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// ECS��Component
/// �R���|�[�l���g�̓f�[�^���������ŁA�U�镑���͎����܂���B
/// </summary>
public struct Spawner : IComponentData
{
    public Entity Prefab;
    public float3 SpawnPosition;
    public float NextSpawnTime;
    public float SpawnRate;
}

/// <summary>
/// ECS��Entity
/// Spawner�Ƃ����Q�[���I�u�W�F�N�g��Entity�ɕϊ�����SpawnerAuthoring�X�N���v�g���쐬���܂��B
/// Baker���g�p���邱�ƂŁA�Q�[���I�u�W�F�N�g��ECS�Ŏg����悤�ɕϊ����邱�Ƃ��ł��܂��B
/// </summary>
class SpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public float SpawnRate;
}

class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Spawner
        {
            // �f�t�H���g�ł́A�e�I�[�T�����OGameObject��Entity�ɕϊ�����܂��B
            // GameObject�i�܂��̓I�[�T�����O�R���|�[�l���g�j���^������ƁAGetEntity�͐��������Entity���������܂��B
            Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            SpawnPosition = authoring.transform.position,
            NextSpawnTime = 0.0f,
            SpawnRate = authoring.SpawnRate
        });
    }
}
