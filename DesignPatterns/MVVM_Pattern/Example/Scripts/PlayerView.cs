using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityModularScripts.DesignPatterns.MVVM_Pattern;

namespace TestScripts.Test_MVVM
{
    public class PlayerView : BaseView<PlayerViewModel>
    {
        public TextMeshProUGUI levelText;  // 레벨을 표시하는 텍스트
        public Slider expSlider;  // 경험치를 표시하는 슬라이더

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                viewModel.GainExp();  // 스페이스바를 누르면 경험치 추가
            }
        }

        protected override void BindUIElements()
        {
            // Text와 Slider를 ViewModel과 바인딩
            UIDataBinder.BindText(levelText, () => "Lv. " + viewModel.Level);
            UIDataBinder.BindSlider(expSlider, () => viewModel.CurrentExp, null);
            expSlider.maxValue = viewModel.MaxExp;  // 슬라이더의 최대값 설정
        }

        protected override void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                // ViewModel의 변경 사항을 UI에 반영
                case nameof(viewModel.Level):
                    levelText.text = "Lv. " + viewModel.Level;
                    break;
                case nameof(viewModel.CurrentExp):
                    expSlider.value = viewModel.CurrentExp;
                    break;
                case nameof(viewModel.MaxExp):
                    expSlider.maxValue = viewModel.MaxExp;
                    break;
            }
        }
    }
}