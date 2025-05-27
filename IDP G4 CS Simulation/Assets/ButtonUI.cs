using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonUI : MonoBehaviour
{
    [SerializeField] private string newGameLevel = "Level";
    public void NewGameButton()
    {
        SceneManager.LoadScene(newGameLevel);
    }
}
