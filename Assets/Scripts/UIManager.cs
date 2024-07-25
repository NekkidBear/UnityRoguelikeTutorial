using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public VisualTreeAsset titleCardTemplate; // Template for the title card
    public VisualTreeAsset gameOverTemplate;  // Template for the game over message

    private VisualElement rootElement;
    private VisualElement titleCardElement;
    private VisualElement gameOverElement;

    // Start is called before the first frame update
    void Start()
    {
        // Get the root VisualElement of the UI
        var uiDocument = GetComponent<UIDocument>();
        rootElement = uiDocument.rootVisualElement;

        // Create title card element from the template
        titleCardElement = titleCardTemplate.CloneTree();
        titleCardElement.style.display = DisplayStyle.None;
        rootElement.Add(titleCardElement);

        // Create game over element from the template
        gameOverElement = gameOverTemplate.CloneTree();
        gameOverElement.style.display = DisplayStyle.None;
        rootElement.Add(gameOverElement);
    }

    // Show the title card with the provided level text
    public void ShowTitleCard(string levelText)
    {
        if (titleCardElement == null) return;

        var titleLabel = titleCardElement.Q<Label>("TitleLabel");
        if (titleLabel != null)
        {
            titleLabel.text = levelText;
        }

        titleCardElement.style.display = DisplayStyle.Flex;
    }

    // Hide the title card
    public void HideTitleCard()
    {
        if (titleCardElement == null) return;

        titleCardElement.style.display = DisplayStyle.None;
    }

    // Show the game over message
    public void ShowGameOverMessage(string message)
    {
        if (gameOverElement == null) return;

        var gameOverLabel = gameOverElement.Q<Label>("GameOverLabel");
        if (gameOverLabel != null)
        {
            gameOverLabel.text = message;
        }

        gameOverElement.style.display = DisplayStyle.Flex;
    }
}
