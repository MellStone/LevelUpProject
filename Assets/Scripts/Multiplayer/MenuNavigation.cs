using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuNavigation : MonoBehaviour
{
    public Button startButton; // Ссылка на кнопку Start
    public Button optionsButton; // Ссылка на кнопку Options
    public Button confirmButton; // Ссылка на кнопку Confirm

    private Button currentButton; // Текущая выбранная кнопка

    private void Start()
    {
        // Устанавливаем начальную выбранную кнопку
        currentButton = startButton;
        EventSystem.current.SetSelectedGameObject(currentButton.gameObject);

        // Привязываем обработчик к кнопке подтверждения
        confirmButton.onClick.AddListener(OnConfirm);
    }

    private void Update()
    {
        // Обработка перемещения по кнопкам
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            NavigateLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NavigateRight();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            OnConfirm();
        }
    }

    private void NavigateLeft()
    {
        // Если текущая кнопка - Options, переключаемся на Start
        if (currentButton == optionsButton)
        {
            currentButton = startButton;
            EventSystem.current.SetSelectedGameObject(currentButton.gameObject);
        }
    }

    private void NavigateRight()
    {
        // Если текущая кнопка - Start, переключаемся на Options
        if (currentButton == startButton)
        {
            currentButton = optionsButton;
            EventSystem.current.SetSelectedGameObject(currentButton.gameObject);
        }
    }

    private void OnConfirm()
    {
        // Вызываем действие текущей выбранной кнопки
        currentButton.onClick.Invoke();
    }
}