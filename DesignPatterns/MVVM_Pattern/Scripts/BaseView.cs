using System.ComponentModel;
using UnityEngine;

namespace UnityModularScripts.DesignPatterns.MVVM_Pattern
{
    /// <summary>
    /// BaseView는 뷰와 뷰모델을 연결하는 기본 클래스입니다. 
    /// ViewModel의 변경 사항을 감지하고 UI를 업데이트하는 역할을 합니다.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel의 타입. BaseViewModel을 상속받아야 합니다.</typeparam>
    public abstract class BaseView<TViewModel> : MonoBehaviour where TViewModel : BaseViewModel
    {
        /// <summary>
        /// ViewModel을 참조하는 프로퍼티입니다. UI와 연결된 ViewModel을 보관합니다.
        /// </summary>
        protected TViewModel viewModel;

        /// <summary>
        /// ViewModel을 바인딩하고 변경 사항을 감지할 수 있도록 이벤트를 연결합니다.
        /// </summary>
        /// <param name="vm">ViewModel 인스턴스</param>
        public virtual void BindViewModel(TViewModel vm)
        {
            viewModel = vm;
            vm.PropertyChanged += OnViewModelPropertyChanged;
            BindUIElements();
    
            // 초기 데이터를 반영하여 UI를 업데이트
            InitializeUIWithViewModelData();
        }

        /// <summary>
        /// ViewModel에 저장된 데이터를 기준으로 UI를 초기화합니다.
        /// </summary>
        private void InitializeUIWithViewModelData()
        {
            foreach (var property in typeof(TViewModel).GetProperties())
            {
                OnViewModelPropertyChanged(viewModel, new PropertyChangedEventArgs(property.Name));
            }
        }

        /// <summary>
        /// UI 요소를 ViewModel과 연결하는 메서드입니다. 
        /// 구체적인 바인딩 로직은 상속받는 클래스에서 구현됩니다.
        /// </summary>
        protected abstract void BindUIElements();

        /// <summary>
        /// ViewModel의 속성 변경 시 호출되는 메서드입니다.
        /// UI는 이 메서드를 통해 ViewModel의 변경 사항을 반영합니다.
        /// </summary>
        /// <param name="sender">이벤트를 발생시킨 객체</param>
        /// <param name="e">프로퍼티 변경 이벤트 데이터</param>
        protected abstract void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e);
    }
}