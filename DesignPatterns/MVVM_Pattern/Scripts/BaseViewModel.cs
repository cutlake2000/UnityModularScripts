using UnityEngine;

namespace UnityModularScripts.DesignPatterns.MVVM_Pattern
{
    /// <summary>
    /// BaseViewModel은 모델 데이터를 처리하고 View와 연결하는 역할을 합니다.
    /// BaseModel을 상속받아 데이터 변경을 알릴 수 있는 기본적인 ViewModel 클래스를 제공합니다.
    /// </summary>
    public class BaseViewModel : BaseModel
    {
        /// <summary>
        /// 로깅 메서드: 중요한 상태 변경을 로그로 기록하는 로직을 추가할 수 있습니다.
        /// </summary>
        protected void Log(string message)
        {
            // 콘솔에 메시지를 출력하거나 파일로 저장 가능
            Debug.Log($"[BaseViewModel] {message}");
        }
    }
}