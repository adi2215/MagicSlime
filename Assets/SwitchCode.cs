using UnityEngine;

public class SwitchCode : MonoBehaviour
{
    [Header("Привязанная доска")]
    public FallingBoard board; // сюда перетащи доску в инспекторе

    public void ActivateDrop()
    {
        board.Drop();
    }
}
