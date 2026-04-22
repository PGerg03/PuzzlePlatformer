using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    #region "Menu Variables"
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button Placeholder;
    [SerializeField] private Button ExitButton;
    [SerializeField] private Text ButtonInfoText;

    [SerializeField] private GameObject PlayMenu;
    [SerializeField] private Button Singleplayer;
    [SerializeField] private Button Multiplayer;

    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] private Slider MainSoundSlider;
    [SerializeField] private Dropdown Windowmodedropdown;
    [SerializeField] private Dropdown Resolutiondropdown;

    [SerializeField] private GameObject PlayerPanel;
    [SerializeField] private Sprite[] Background;
    [SerializeField] private Sprite[] Players;
    #endregion
    private Game game;

    void Start()
    {
        game = gameObject.GetComponent<Game>();

        List<Dropdown.OptionData> res = new();
        for (int i = 0; i < Screen.resolutions.Count(); i++)
        {
            Dropdown.OptionData data = new(Screen.resolutions[i].ToString().Split('@')[0]);
            res.Add(data);
        }
        res.Reverse();

        Resolutiondropdown.ClearOptions();
        Resolutiondropdown.AddOptions(res);

        MainSoundSlider.value = game.MainVolume;
        Resolutiondropdown.value = game.Resolution;
        int WindowMode = (int)game.WindowMode;
        Windowmodedropdown.value = WindowMode == 3 ? 2 : WindowMode;

        string[] seged = Resolutiondropdown.options[game.Resolution].text.Split('x');
        int[] screensize =
        {
            Convert.ToInt32(seged[0]),
            Convert.ToInt32(seged[1])
        };

        Screen.SetResolution(screensize[0], screensize[1], game.WindowMode);
    }

    public void OnHoverStart(string buttontag)
    {
        ChangeInfoPanel(buttontag);
    }
    public void OnHoverEnd()
    {
        ChangeInfoPanel("Off");
    }
    private void ChangeInfoPanel(string input)
    {
        switch (input)
        {
            case "Off":
                ButtonInfoText.text = "";
                break;
            case "Play":
                ButtonInfoText.text = "Játék indítása. Válassz játékmódot a játék kezdése előtt.";
                break;
            case "Settings":
                ButtonInfoText.text = "Beállításokban módosítható a felbontás, az ablak mód és a hangerő.";
                break;
            case "Exit":
                ButtonInfoText.text = "Kilépés a játékból.";
                break;
        }
    }
    public void PlayClick()
    {
        PlayMenu.SetActive(true);
    }
    public void SettingsClick()
    {
        SettingsMenu.SetActive(true);
        MainMenu.SetActive(false);
    }
    public void BackClick()
    {
        SettingsMenu.SetActive(false);
        PlayMenu.SetActive(false);
        MainMenu.SetActive(true);
    }
    public void QuitClick()
    {
        game.SaveOptions();
        Application.Quit();
    }

    public async void StartGame(int playercount)
    {
        if (playercount == 1) // SinglePlayer
        {
            game.SaveOptions();
            await SceneLoader.Instance.LoadScene(1);
        }
        else // MultiPlayer
        {
            game.SaveOptions();
            await SceneLoader.Instance.LoadScene(2);
        }
    }

    public void SettingsChange(float v)
    {
        game.MainVolume = v;
    }
    public void SettingsChange(int v)
    {
        game.WindowMode = (FullScreenMode)(Windowmodedropdown.value == 2 ? 3 : Windowmodedropdown.value);
        game.Resolution = Resolutiondropdown.value;

        string[] seged = Resolutiondropdown.options[game.Resolution].text.Split('x');
        int[] screensize =
        {
            Convert.ToInt32(seged[0]),
            Convert.ToInt32(seged[1])
        };

        Screen.SetResolution(screensize[0], screensize[1], game.WindowMode);
    }

    int timer = 0;
    void FixedUpdate()
    {
        timer++;

        if (timer >= 600)
        {
            timer = 0;
            gameObject.GetComponent<Image>().sprite = Background[2];
            PlayerPanel.GetComponent<SpriteRenderer>().sprite = Players[2];
        }
        else if (timer == 400)
        {
            gameObject.GetComponent<Image>().sprite = Background[1];
            PlayerPanel.GetComponent<SpriteRenderer>().sprite = Players[1];
        }
        else if (timer == 200)
        {
            gameObject.GetComponent<Image>().sprite = Background[0];
            PlayerPanel.GetComponent<SpriteRenderer>().sprite = Players[0];
        }
    }
}
