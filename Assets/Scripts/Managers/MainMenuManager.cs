using UnityEngine;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        private void Awake()
        {
            UIManager.LoadUISceneIfNotLoaded();
        }
    }
}
