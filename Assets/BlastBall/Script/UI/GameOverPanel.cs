using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] 
    private Button nextLevelButton;
    [SerializeField] 
    private Button restartButton;
    [SerializeField] 
    private TMP_Text completionText;

    private void Start()
    {
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClick);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClick);
    }

    private void OnNextLevelClick()
    {
        GameManager.Instance.NextLevel();
    }

    private void OnRestartClick()
    {
        GameManager.Instance.RestartLevel();
    }
}