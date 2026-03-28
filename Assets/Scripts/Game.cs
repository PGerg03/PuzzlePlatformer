using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    #region Variables

    [Header("Settings")]
    public float MainVolume;
    public int WindowMode;
    public int Resolution;
    public int Story;

    [Header("inGame")]
    public int SingleUnlockedMaps;
    public int MultiUnlockedMaps;
    public int SingleStoryCount;
    public int MultiStoryCount;
    [SerializeField] private Tilemap Tilemap;
    public bool inGame;
    public bool SinglePlayer;
    public int CurrentMap;
    public string CurrentStory;

    [Header("Game")]
    [SerializeField] private Button[] MapButtons;
    [SerializeField] private GameObject[] Maps;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Camera MapCamera;

    [Header("MenuPause")]
    [SerializeField] private GameObject MenuPausePanel;
    [SerializeField] private GameObject MenuPauseMenu;
    [SerializeField] private GameObject MenuSettingsMenu;
    
    [Header("Pause")]
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private Button[] PauseMenuButtons;
    [SerializeField] private GameObject StoryPanel;
    [SerializeField] private Text StoryText;

    [Header("Options")]
    [SerializeField] private GameObject OptionsPanel;
    [SerializeField] private Slider MainSoundSlider;
    [SerializeField] private Dropdown Windowmodedropdown;
    [SerializeField] private Dropdown Resolutiondropdown;
    [SerializeField] private Dropdown StoryOptions;
    
    [Header("Complete")]
    [SerializeField] private GameObject CompletePanel;
    [SerializeField] private GameObject LostPanel;
    [SerializeField] private Button[] CompletePanelButtons;
    
    [Header("Player Variables")]
    public GameObject Player1;
    public GameObject Player2;
    public PlayerMovement player1Movement;
    public PlayerMovement player2Movement;

    #endregion

    #region Load & Save

    void Awake()
    {
        Debug.Log("Game Awake");

        MainVolume = 0.5f;
        WindowMode = 0;
        Resolution = 0;

        SingleUnlockedMaps = 1;
        MultiUnlockedMaps = 1;

        SingleStoryCount = 0;
        MultiStoryCount = 0;

        inGame = false;

        // Load if exists
        if (File.Exists("Options.json")) Saver.ReadOptionsFile("Options.json", this);
    }
    public void SaveOptions()
    {
        Saver.SaveOptions("Options.json", this);
    }

    #endregion
    #region Main

    void Start()
    {
        List<Dropdown.OptionData> res = new();
        for (int i = 0; i < Screen.resolutions.Count(); i++)
        {
            Dropdown.OptionData data = new(Screen.resolutions[i].ToString().Split('@')[0]);
            res.Add(data);
        }
        res.Reverse();

        Resolutiondropdown.ClearOptions();
        Resolutiondropdown.AddOptions(res);

        MainSoundSlider.value = MainVolume;
        Windowmodedropdown.value = WindowMode;
        Resolutiondropdown.value = Resolution;
        Debug.Log("Options loaded");
        
        if(!SceneLoader.Instance.IsUnityNull()) 
            SceneLoader.Instance.LoadFinished();

        
        foreach (GameObject map in Maps)
        {
            map.SetActive(false);
        }

        if (SceneManager.GetActiveScene().buildIndex == 1) // Singleplayer
        {
            Debug.Log("SinglePlayer");
            SinglePlayer = true;
            for (int i = 0; i <= SingleUnlockedMaps && i < MapButtons.Count(); i++)
            {
                MapButtons[i].enabled = true;
            }
        }
        else // MultiPlayer
        {
            Debug.Log("MultiPlayer");
            SinglePlayer = false;
            for (int i = 0; i <= MultiUnlockedMaps && i < MapButtons.Count(); i++)
            {
                MapButtons[i].enabled = true;
            }
        }
        
        MenuPausePanel.SetActive(false);
        MenuPauseMenu.SetActive(false);
        MenuSettingsMenu.SetActive(false);

        player1Movement = Player1.GetComponent<PlayerMovement>();
        player1Movement.PlayerAxes = "P1Horizontal";
        player1Movement.JumpCode = KeyCode.W;
        if(!Player2.IsUnityNull())
        {
            player2Movement = Player2.GetComponent<PlayerMovement>();
            player2Movement.PlayerAxes = "P2Horizontal";
            player2Movement.JumpCode = KeyCode.UpArrow;
        }
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        player1Movement.enabled = inGame;
        if(!Player2.IsUnityNull()) player2Movement.enabled = inGame;
        
        if(inGame)
        {
            if(SinglePlayer)
            {

            }
            else // MultyPlayer
            {
                
            }
        }
    }

    #endregion
    #region Menu Fucntions

    // Game Menu Function
    public void ChangeToMapCamera(int MapNumber)
    {
        MapCamera.depth = 1;
        MapCamera.gameObject.SetActive(true);

        MainCamera.depth = 0;
        MainCamera.gameObject.SetActive(false);
      
        foreach (GameObject map in Maps)
        {
            map.SetActive(false);
        }
        Maps[MapNumber].SetActive(true);

        PausePanel.SetActive(false);
        PauseMenu.SetActive(false);
        OptionsPanel.SetActive(false);
        CompletePanel.SetActive(false);
        LostPanel.SetActive(false);

        CurrentStory = TileMapController.instance.LoadMap(MapNumber+1, SinglePlayer ? "Single" : "Multy");
        
        LoadStory();

        CurrentMap = MapNumber;
    }

    // Story Functions
    public void LoadStory()
    {
        if(Story == 2 || Story == 0 && SingleStoryCount < SingleUnlockedMaps) // Always or First time
        {
            StoryText.text = CurrentStory;
            StoryPanel.SetActive(true);
        }
        else
        {
            StoryOk();
        }

        if(SinglePlayer) SingleStoryCount = Math.Max(SingleStoryCount, SingleUnlockedMaps);
        else MultiStoryCount = Math.Max(MultiStoryCount, MultiUnlockedMaps);
    }
    public void StoryOk()
    {
        StoryPanel.SetActive(false);
        
        inGame = true;
    }

    // Pause Menu Functions
    /// <param name="on">true to pause, false to continue</param>
    public void Pause(bool on)
    {
        PausePanel.SetActive(on);
        PauseMenu.SetActive(on);

        inGame = !on;
    }
    public void MenuPause(bool on)
    {
        MenuPausePanel.SetActive(on);
        MenuPauseMenu.SetActive(on);

        // inGame = !on;
    }
    /// <param name="on">true to open, false to close</param>
    public void OptionsMenu(bool on)
    {
        PauseMenu.SetActive(!on);
        MenuPauseMenu.SetActive(!on);
        OptionsPanel.SetActive(on);
        MenuSettingsMenu.SetActive(on);
    }
    public void ResetCurrentMap()
    {
        TileMapController.instance.LoadMap(CurrentMap+1, SinglePlayer ? "Single" : "Multy");

        CompletePanel.SetActive(false);
        LostPanel.SetActive(false);
        PausePanel.SetActive(false);
        PauseMenu.SetActive(false);
        MenuPausePanel.SetActive(false);
        OptionsPanel.SetActive(false);
        
        inGame = true;
    }
    public void ChangeToMainCamera()
    {
        MainCamera.depth = 1;
        MainCamera.gameObject.SetActive(true);

        MapCamera.depth = 0;
        MapCamera.gameObject.SetActive(false);

        foreach (GameObject map in Maps)
        {
            map.SetActive(false);
        }

        inGame = false;
    }
    public async void ToMainMenu()
    {
        SaveOptions();
        await SceneLoader.Instance.LoadScene(0);
    }

    // Options Menu Functions
    public void SettingsChange(float v)
    {
        MainVolume = v;
    }
    public void SettingsChange(int v)
    {
        WindowMode = Windowmodedropdown.value;
        Resolution = Resolutiondropdown.value;

        string[] seged = Resolutiondropdown.options[Resolution].text.Split('x');
        int[] screensize =
        {
            Convert.ToInt32(seged[0]),
            Convert.ToInt32(seged[1])
        };

        Screen.SetResolution(screensize[0], screensize[1], WindowMode != 2);
    }
    public void StorySetting(int v)
    {
        Story = v;
    }
    public void Back()
    {
        OptionsMenu(false);
    }

    // Complete Panel Functions
    public void CompleteLevel()
    {
        if (SinglePlayer) SingleUnlockedMaps++;
        else MultiUnlockedMaps++;

        PausePanel.SetActive(true);
        CompletePanel.SetActive(true);

        inGame = false;
    }
    public void LoseLevel()
    {
        PausePanel.SetActive(true);
        LostPanel.SetActive(true);

        inGame = false;
    }
    public void NextMap()
    {
        Maps[CurrentMap].SetActive(false);
        CurrentMap++;
        Maps[CurrentMap].SetActive(true);

        CurrentStory = TileMapController.instance.LoadMap(CurrentMap+1, SinglePlayer ? "Single" : "Multy");
        
        LoadStory();

        CompletePanel.SetActive(false);
        PausePanel.SetActive(false);
    }

    #endregion

}
