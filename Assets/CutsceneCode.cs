using System.Collections;
using UnityEngine;

public class CutsceneCode : MonoBehaviour
{
    [Header("Основные ссылки")]
    public HeroineCode player;                // Герой
    public Animator playerAnimator;          // Аниматор героя
    public ParticleSystem particleEffect;    // Партикль эффект
    public SpriteRenderer slimeRenderer;     // Спрайт слайма (для альфа-анимации)
    public float slimeFadeTime = 2f;         // Время появления слайма

    public PlayerMovements[] playerController; // Скрипт, отвечающий за движение
    private bool isCutscenePlaying = false;

    void Start()
    {
        // Прячем слайма в начале
        if (slimeRenderer != null)
        {
            slimeRenderer.enabled = false;
        }

        // Запускаем катсцену при старте (можно вызвать позже, если нужно)
        StartCoroutine(PlayBeginCutscene());
    }

    IEnumerator PlayBeginCutscene()
    {
        isCutscenePlaying = true;
        playerAnimator.Play("SittingAnim");
        foreach (var player in playerController)
            if (player != null)
                player.enabled = false;
                
        yield return new WaitForSeconds(1.5f);

        // 2️⃣ Меняем состояние анимации героя
        playerAnimator.Play("MagicAnim"); 
        yield return new WaitForSeconds(0.2f);

        // 3️⃣ Запускаем particle effect
        if (particleEffect != null)
            particleEffect.Play();

        yield return new WaitForSeconds(1f);
        
        if (slimeRenderer != null)
        {
            slimeRenderer.enabled = true;
        }

        yield return new WaitForSeconds(0.5f); // ждём немного

        playerAnimator.Play("SittingAnim");
        player.dialog.StartDialogue(0);

        yield return new WaitForSeconds(0.2f);

        // 5️⃣ Включаем управление обратно
        foreach(var player in playerController)
            if (player != null)
                player.enabled = true;

        isCutscenePlaying = false;
    }
}
