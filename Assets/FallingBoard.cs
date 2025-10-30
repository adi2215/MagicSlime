using UnityEngine;
using System.Collections;

public class FallingBoard : MonoBehaviour
{
    [Header("Параметры падения")]
    public float fallHeight = 5f;      // Насколько высоко доска "висит" перед падением
    public float fallSpeed = 10f;      // Скорость падения
    public bool canFall = true;

    private Vector3 targetPos;         // Позиция, куда должна упасть доска
    public bool isFalling = false;

    private void Start()
    {
        // Запоминаем конечную позицию (куда должна упасть)
        targetPos = transform.position;

        // Поднимаем доску повыше
        transform.position = targetPos + Vector3.up * fallHeight;
    }

    public void Drop()
    {
        if (!canFall || isFalling) return;
        StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        isFalling = true;

        // Пока не достигнет нижней точки — падаем
        while (transform.position.y > targetPos.y + 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                fallSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos; // точно выставляем позицию
        isFalling = false;
    }
}
