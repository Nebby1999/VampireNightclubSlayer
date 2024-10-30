using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VampireSlayer
{
    public class LoadingScreenBehaviour : MonoBehaviour
    {
        [Header("Text Settings")]
        public TextMeshProUGUI textMeshProUGUI;
        public float timeBetweenTextChanges;
        public string baseText;

        [Header("Spinner Settings")]
        public RectTransform spinner;
        public float spinnerSpeed;

        private string _formatText;
        private int _dotCount;

        private Coroutine _coroutine;

        private void OnEnable()
        {
            _coroutine = StartCoroutine(ElipsesCoroutine());    
        }

        private void OnDisable()
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        private void Update()
        {
            if(spinner)
            {
                spinner.rotation = Quaternion.Euler(0, 0, spinner.rotation.eulerAngles.z + (spinnerSpeed * Time.deltaTime));
            }
        }

        private IEnumerator ElipsesCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeBetweenTextChanges);
                _dotCount++;
                if (_dotCount > 3)
                {
                    _dotCount = 0;
                }
                char[] dots = new char[_dotCount];
                for(int i = 0; i < _dotCount; i++)
                {
                    dots[i] = '.';
                }
                textMeshProUGUI.SetText(baseText + new string(dots));
            }
        }
    }
}
