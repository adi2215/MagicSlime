using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HeroineCode : MonoBehaviour
{
    [Header("Диалоги / Доп. Ссылки")]
    public DialogBox dialog;

    [Header("Движение героини")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float stopDistance = 0.1f;
    public float waitAfterTeleport = 0.5f;

    [Header("Анимация")]
    public Animator animator;

    private int currentPoint = 0;
    private bool isMoving = false;
    private bool waitingForBoard = false;

    private void StartMoving()
    {
        if (waypoints.Length > 0)
            MoveToNextPoint();
        else
            Debug.LogWarning("⚠️ Heroine has no waypoints assigned!");
    }

    public void PlayAnim()
    {
        animator.Play("Idle");
        Invoke(nameof(StartMoving), 1f);
    }

    private void Update()
    {
        if (!isMoving || waitingForBoard || waypoints.Length == 0) return;

        Transform target = waypoints[currentPoint];
        Vector3 direction = (target.position - transform.position).normalized;

        // Движение к цели
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Разворот
        if (direction.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1) * 0.62f;
        else if (direction.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1) * 0.62f;

        // Проверяем, достигли ли точки
        if (Vector3.Distance(transform.position, target.position) <= stopDistance)
        {
            ReachPoint();
        }
    }

    private void MoveToNextPoint()
    {
        if (currentPoint >= waypoints.Length)
        {
            isMoving = false;
            animator.Play("Idle");
            return;
        }

        Transform target = waypoints[currentPoint];

        // Проверяем "прыжковую" точку
        if (target.CompareTag("JumpPoint") || target.CompareTag("DownPoint"))
        {
            transform.position = target.position;
            animator.Play("Idle");
            currentPoint++;
            Invoke(nameof(MoveToNextPoint), waitAfterTeleport);
            return;
        }

        // Проверяем точку с тегом Switch1 (где ждём падение доски)
        if (target.CompareTag("Switch1"))
        {
            StartCoroutine(WaitForBoardToFall(target));
            return;
        }

        // Обычное движение
        isMoving = true;
        animator.Play("Walking");
    }

    private void ReachPoint()
    {
        isMoving = false;
        animator.Play("Idle");

        currentPoint++;
        if (currentPoint < waypoints.Length)
            Invoke(nameof(MoveToNextPoint), 1f);
    }

    // 👇 Ожидание падения доски
    private IEnumerator WaitForBoardToFall(Transform target)
    {
        waitingForBoard = true;
        animator.Play("Idle");

        // Ищем доску рядом с этой плиткой
        FallingBoard board = target.GetComponentInChildren<FallingBoard>();
        if (board == null && board.isFalling==true)
        {
            Debug.LogWarning("⚠️ На точке Switch1 нет FallingBoard!");
            waitingForBoard = false;
            currentPoint++;
            yield return new WaitForSeconds(1f);
            MoveToNextPoint();
            yield break;
        }

        // Ждём, пока доска перестанет падать
        yield return new WaitUntil(() => !board.isFalling);

        waitingForBoard = false;
        currentPoint++;
        yield return new WaitForSeconds(1f);
        MoveToNextPoint();
    }

    // 💎 Когда героиня входит в зону кристалла
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crystal"))
        {
            isMoving = false;
            animator.Play("Idle");

            dialog.StartDialogue(2); // Запускаем диалог (пример)

            Debug.Log("✨ Crystal найден! Конец игры!");
            // Тут можно вызвать сцену конца, или что нужно:
            Invoke("CallEnd", 4f);
        }
    }

    private void CallEnd()
    {
        SceneManager.LoadScene("End");
    }
}
