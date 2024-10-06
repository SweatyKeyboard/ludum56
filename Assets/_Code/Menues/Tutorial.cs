using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Code.Menues
{
    public sealed class Tutorial : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private TMP_Text _text1;
        [SerializeField] private TMP_Text _text2;
        [SerializeField] private Transform[] _step1;
        [SerializeField] private Transform[] _step2;
        [SerializeField] private Transform[] _step3;
        [SerializeField] private Transform[] _step4;
        [SerializeField] private string[] _text1Values;
        [SerializeField] private string[] _text2Values;

        private int _stepIndex = 0;
        public void NextStep()
        {
            _stepIndex++;

            switch (_stepIndex)
            {
                case 1:
                    NextStepAsync(_step1, _step2).Forget();
                    break;
                
                case 2:
                    NextStepAsync(_step2, _step3).Forget();
                    break;
                
                case 3:
                    NextStepAsync(_step3, _step4).Forget();
                    break;
                
                case 4:
                    NextStepAsync(_step4, Array.Empty<Transform>()).Forget();
                    break;
                
                case 5:
                    SceneManager.LoadScene(0);
                    break;
            }
        }

        private async UniTask NextStepAsync(Transform[] step1, Transform[] step2)
        {
            var time = 0.5f;
            foreach (var en in step1)
            {
                en.DOScale(Vector3.zero, time).SetEase(Ease.InSine);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(time / 2));
            _camera.transform.DOMoveX(_camera.transform.position.x + 12f, time).SetEase(Ease.InSine);
            await UniTask.Delay(TimeSpan.FromSeconds(time / 2));
            _text1.text = _text1Values[_stepIndex - 1];
            _text2.text = _text2Values[_stepIndex - 1];

            if (step2.Length > 0)
            {
                foreach (var dis in step2)
                {
                    dis.gameObject.SetActive(true);
                    dis.DOScale(Vector3.one, time).SetEase(Ease.InSine);
                }
            }
            else
            {
                _text1.transform.DOScale(Vector3.one, time).SetEase(Ease.InSine);
                _text2.transform.DOScale(Vector3.one, time).SetEase(Ease.InSine);
            }
        }
    }
}