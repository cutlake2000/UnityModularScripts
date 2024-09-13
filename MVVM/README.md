# UnityModularScripts - MVVM 패턴 모듈화 스크립트

이 프로젝트는 **Unity**에서 사용 가능한 **MVVM (Model-View-ViewModel)** 패턴을 기반으로 작성된 모듈화된 스크립트들로 구성되어 있습니다. 이 스크립트들은 게임 개발 과정에서 UI와 데이터 간의 명확한 분리를 돕고, 데이터 바인딩을 통한 효율적인 UI 업데이트를 제공합니다. 프로젝트의 각 스크립트는 재사용 가능하고 확장 가능한 형태로 설계되어 다양한 상황에서 활용할 수 있습니다.

---

## 주요 폴더 및 파일 설명

### 1. **MVVM** 폴더
이 폴더는 MVVM 패턴을 지원하는 기본적인 클래스와 헬퍼 스크립트로 구성되어 있습니다.

- **BaseModel.cs**  
  `BaseModel` 클래스는 `INotifyPropertyChanged` 인터페이스를 구현하여, 모델의 데이터가 변경될 때 알림을 발생시킵니다.  
  이를 통해 UI가 모델의 변화를 자동으로 반영할 수 있도록 돕습니다.

  ```csharp
  public abstract class BaseModel : INotifyPropertyChanged
  {
      public event PropertyChangedEventHandler PropertyChanged;

      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
      {
          if (EqualityComparer<T>.Default.Equals(field, value)) return false;
          field = value;
          OnPropertyChanged(propertyName);
          return true;
      }
  }

- **BaseViewModel.cs**  
  `BaseViewModel` 클래스는 모델 데이터를 처리하고, 변경 사항을 알릴 수 있는 기본적인 ViewModel 클래스입니다.
  UI와 모델 사이의 연결을 담당하며, 필요한 로직을 추가할 수 있습니다.

  ```csharp
  public class BaseViewModel : BaseModel
  {
      protected void Log(string message)
      {
          Debug.Log($"[BaseViewModel] {message}");
      }
  }

- **BaseView.cs**  
  `BaseView` 클래스는 View와 ViewModel을 연결하고, ViewModel의 변경 사항을 UI에 반영하는 역할을 합니다.
  `BindViewModel` 메서드를 통해 ViewModel을 바인딩하고, 변경 사항을 자동으로 감지하여 UI 요소를 업데이트할 수 있습니다.

  ```csharp
  public abstract class BaseView<TViewModel> : MonoBehaviour where TViewModel : BaseViewModel
  {
      protected TViewModel viewModel;
  
      public virtual void BindViewModel(TViewModel vm)
      {
          viewModel = vm;
          vm.PropertyChanged += OnViewModelPropertyChanged;
          BindUIElements();
          InitializeUIWithViewModelData();
      }
  
      private void InitializeUIWithViewModelData()
      {
          foreach (var property in typeof(TViewModel).GetProperties())
          {
              OnViewModelPropertyChanged(viewModel, new PropertyChangedEventArgs(property.Name));
          }
      }
  
      protected abstract void BindUIElements();
  
      protected abstract void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e);
  }

- **UIDataBinder.cs**  
  `UIDataBinder` 클래스는 Unity UI 요소와 ViewModel의 데이터를 바인딩하는 헬퍼 클래스입니다.
  텍스트, 슬라이더, 토글 등의 UI 요소와 ViewModel 간의 양방향 데이터 바인딩을 쉽게 구현할 수 있도록 도와줍니다.

  ```csharp
  public static class UIDataBinder
  {
      public static void BindText(TextMeshProUGUI uiText, Func<string> getValue, Action<string> setValue = null)
      {
          uiText.text = getValue();
      }
  
      public static void BindText(TextMeshPro uiText, Func<string> getValue, Action<string> setValue = null)
      {
          uiText.text = getValue();
      }
  
      public static void BindInputField(InputField inputField, Func<string> getValue, Action<string> setValue)
      {
          inputField.text = getValue();
          inputField.onValueChanged.AddListener(value => setValue?.Invoke(value));
      }
  
      public static void BindSlider(Slider slider, Func<float> getValue, Action<float> setValue)
      {
          slider.value = getValue();
          slider.onValueChanged.AddListener(value => setValue?.Invoke(value));
      }
  
      public static void BindToggle(Toggle toggle, Func<bool> getValue, Action<bool> setValue)
      {
          toggle.isOn = getValue();
          toggle.onValueChanged.AddListener(value => setValue?.Invoke(value));
      }
  }

---

## 주요 기능
### 1. **MVVM** 패턴 적용
이 프로젝트는 MVVM 패턴을 적용하여, 데이터와 UI의 분리 및 효율적인 데이터 바인딩을 제공합니다.

### 2. 확장성
각 클래스는 모듈화되어 재사용 가능하며, 다양한 게임 프로젝트에서 확장하여 사용할 수 있습니다.

### 3. 양방향 데이터 바인딩
`UIDataBinder` 클래스를 사용하여 UI 요소와 ViewModel 간의 양방향 데이터 바인딩을 쉽게 구현할 수 있습니다.