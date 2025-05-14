using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}
