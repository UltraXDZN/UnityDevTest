using UnityEngine;

namespace UDT.Gameplay.UI
{
    using static UDT.Settings.UDT_Constants;
    public class UDT_PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject m_pauseMenu;

        /// <summary>
        /// Toggles the pause menu (currently only toggled when the player is dead).
        /// </summary>
        public void TogglePauseMenu()
        {
            m_pauseMenu.SetActive(!m_pauseMenu.activeSelf);
            Time.timeScale = m_pauseMenu.activeSelf ? TIME_SCALE_PAUSE : TIME_SCALE_DEFAULT;
        }
    }
}