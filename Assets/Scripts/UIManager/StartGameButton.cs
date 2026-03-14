using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public void DoSomething()
    {
        GameManager.ChangeScene("TechnoBuild");
        Debug.Log("DoSomething");
    }
    public void DoAnotherSomething()
    {
        Debug.Log("DoAnotherSomething");
    }
}
