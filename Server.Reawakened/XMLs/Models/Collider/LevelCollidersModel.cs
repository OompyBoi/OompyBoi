using UnityEngine;

public class LevelCollidersModel(int levelId, List<Collider> colliderList)
{
    public int LevelId { get; } = levelId;

    public List<Collider> ColliderList { get; } = colliderList;
}
