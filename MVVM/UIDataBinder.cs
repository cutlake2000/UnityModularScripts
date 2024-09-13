using System;
using TMPro;
using UnityEngine.UI;

namespace UnityModularScripts.MVVM
{
    /// <summary>
    /// UIDataBinder는 Unity UI와 ViewModel 간의 데이터를 바인딩하는 헬퍼 클래스입니다.
    /// TextMeshPro와 TextMeshProUGUI에 맞춰 텍스트 바인딩을 각각 관리합니다.
    /// </summary>
    public static class UIDataBinder
    {
        /// <summary>
        /// TextMeshProUGUI UI와 ViewModel의 문자열을 바인딩합니다.
        /// ViewModel의 값을 UI에 반영합니다.
        /// </summary>
        /// <param name="uiText">TextMeshProUGUI UI 요소</param>
        /// <param name="getValue">ViewModel에서 문자열 값을 가져오는 함수</param>
        /// <param name="setValue">필요시 문자열을 설정하는 액션 (선택적)</param>
        public static void BindText(TextMeshProUGUI uiText, Func<string> getValue, Action<string> setValue = null)
        {
            uiText.text = getValue();
        }

        /// <summary>
        /// TextMeshPro (3D 텍스트)와 ViewModel의 문자열을 바인딩합니다.
        /// ViewModel의 값을 3D 텍스트에 반영합니다.
        /// </summary>
        /// <param name="uiText">TextMeshPro UI 요소</param>
        /// <param name="getValue">ViewModel에서 문자열 값을 가져오는 함수</param>
        /// <param name="setValue">필요시 문자열을 설정하는 액션 (선택적)</param>
        public static void BindText(TextMeshPro uiText, Func<string> getValue, Action<string> setValue = null)
        {
            uiText.text = getValue();
        }

        /// <summary>
        /// InputField와 ViewModel의 문자열을 양방향으로 바인딩합니다.
        /// 사용자가 InputField에 입력한 값을 ViewModel에 반영합니다.
        /// </summary>
        /// <param name="inputField">InputField UI 요소</param>
        /// <param name="getValue">ViewModel에서 문자열 값을 가져오는 함수</param>
        /// <param name="setValue">입력된 값을 ViewModel에 설정하는 액션</param>
        public static void BindInputField(InputField inputField, Func<string> getValue, Action<string> setValue)
        {
            inputField.text = getValue();
            inputField.onValueChanged.AddListener(value => setValue?.Invoke(value));
        }

        /// <summary>
        /// Slider UI와 ViewModel의 float 값을 양방향으로 바인딩합니다.
        /// Slider의 값을 ViewModel에 반영하고, ViewModel의 값을 Slider에 반영합니다.
        /// </summary>
        /// <param name="slider">Slider UI 요소</param>
        /// <param name="getValue">ViewModel에서 float 값을 가져오는 함수</param>
        /// <param name="setValue">Slider 값이 변경될 때 ViewModel에 설정하는 액션</param>
        public static void BindSlider(Slider slider, Func<float> getValue, Action<float> setValue)
        {
            slider.value = getValue();
            slider.onValueChanged.AddListener(value => setValue?.Invoke(value));
        }

        /// <summary>
        /// Toggle UI와 ViewModel의 boolean 값을 양방향으로 바인딩합니다.
        /// Toggle의 상태를 ViewModel에 반영하고, ViewModel의 값을 Toggle에 반영합니다.
        /// </summary>
        /// <param name="toggle">Toggle UI 요소</param>
        /// <param name="getValue">ViewModel에서 bool 값을 가져오는 함수</param>
        /// <param name="setValue">Toggle 값이 변경될 때 ViewModel에 설정하는 액션</param>
        public static void BindToggle(Toggle toggle, Func<bool> getValue, Action<bool> setValue)
        {
            toggle.isOn = getValue();
            toggle.onValueChanged.AddListener(value => setValue?.Invoke(value));
        }
    }
}