using UnityEngine;


public abstract class SpawnFactory : MonoBehaviour
{
    public abstract GameObject Spawn(Vector2 position);
}