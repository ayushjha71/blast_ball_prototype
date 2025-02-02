using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace blastBall.UI
{
    public class StartGame : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;
        [SerializeField]
        private Button quitButton;

        private void Start()
        {
            startButton.onClick.AddListener(OnClickStart);
            quitButton.onClick.AddListener(Quit);
        }

        public void OnClickStart()
        {
            SceneManager.LoadScene("Level_1");
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
