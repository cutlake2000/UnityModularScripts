using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UnityModularScripts.MVVM
{
    /// <summary>
    /// BaseModel은 데이터와 상태를 관리하는 기본 모델 클래스입니다. 
    /// INotifyPropertyChanged 인터페이스를 구현하여 속성 값이 변경될 때 변경 알림을 발생시킵니다.
    /// 이 클래스를 상속받는 모든 모델은 데이터 바인딩을 통해 UI에 변경 사항을 알릴 수 있습니다.
    /// </summary>
    public abstract class BaseModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 프로퍼티가 변경될 때 발생하는 이벤트입니다.
        /// UI는 이 이벤트를 구독하여 자동으로 변경 사항을 반영할 수 있습니다.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 프로퍼티 값이 변경될 때 PropertyChanged 이벤트를 발생시킵니다.
        /// [CallerMemberName]은 메서드 호출 시 호출된 멤버 이름을 자동으로 전달합니다.
        /// </summary>
        /// <param name="propertyName">변경된 프로퍼티의 이름</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 프로퍼티 값을 설정하는 헬퍼 메서드로, 값이 변경된 경우에만 이벤트를 발생시킵니다.
        /// 값이 같지 않으면 필드를 업데이트하고 변경 알림을 발생시킵니다.
        /// </summary>
        /// <typeparam name="T">프로퍼티 타입</typeparam>
        /// <param name="field">변경할 필드</param>
        /// <param name="value">새로운 값</param>
        /// <param name="propertyName">프로퍼티 이름</param>
        /// <returns>값이 변경되었는지 여부를 반환</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}