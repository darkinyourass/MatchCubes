using System;
using TMPro;
using UnityEngine;

namespace Common.View.Tutorial
{
    public class TutorialPanel : MonoBehaviour
    {
        // [SerializeField] private TMP_Text _cameraTutorialText;
        [SerializeField] private TMP_Text _whiteCubeTutorialText;

        // [SerializeField] private string CAMERATEXT = "YOU CAN ROTATE CUBE HOLDING ONE FINGER AND MOVING TO THE CERTAIN DIRECTION";
        [SerializeField] private string TUTORIALTEXT = "YOU CAN CLICK ON WHITE CUBE TO FINISH THE LEVEL";


        private void OnEnable()
        {
            // _cameraTutorialText.text = CAMERATEXT;
            _whiteCubeTutorialText.text = TUTORIALTEXT;
        }

        private void Update()
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                gameObject.SetActive(false);
            }
        }
    }
}