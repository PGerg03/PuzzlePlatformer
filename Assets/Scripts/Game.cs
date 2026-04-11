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
    public FullScreenMode WindowMode;
    public int Resolution;
    public int Story;

    [Header("inGame")]
    public int SingleUnlockedMaps;
    public int MultiUnlockedMaps;
    public int SingleStoryCount;
    public int MultiStoryCount;
    public List<StarCollection> SinglePlayerStars = new();
    public List<StarCollection> MultiPlayerStars = new();
    [SerializeField] private Tilemap Tilemap;
    public bool inGame;
    public bool SinglePlayer;
    public int CurrentMap;
    public string CurrentStory;
    [SerializeField] private Text StarCounter;
    public int StarCount => CountStars();

    [Header("Game")]
    [SerializeField] private Button[] MapButtons;
    [SerializeField] private GameObject[] Maps;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Camera MapCamera;
    [SerializeField] private Transform PlayersObject;

    [Header("MenuPause")]
    [SerializeField] private GameObject MenuPausePanel;
    [SerializeField] private GameObject MenuPauseMenu;
    [SerializeField] private GameObject NotUnlockedPanel;
    [SerializeField] private Text NotUnlockedText;

    [Header("Pause")]
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private Button[] PauseMenuButtons;
    [SerializeField] private GameObject StoryPanel;
    [SerializeField] private Text StoryText;

    [Header("Menu Options")]
    [SerializeField] private GameObject MenuSettingsMenu;
    [SerializeField] private Slider MenuMainSoundSlider;
    [SerializeField] private Dropdown MenuWindowmodedropdown;
    [SerializeField] private Dropdown MenuResolutiondropdown;
    [SerializeField] private Dropdown MenuStoryOptions;

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

    [Header("Single Player Variables")]
    [SerializeField] private List<GameObject> PlayerModels;
    public GameObject NaturePlayer;
    public GameObject DesertPlayer;
    public GameObject IcePlayer;
    public PlayerMovement NaturePlayerMovement;
    public PlayerMovement DesertPlayerMovement;
    public PlayerMovement IcePlayerMovement;

    [Header("Multy Player Variables")]
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
        Windowmodedropdown.value = (int)WindowMode == 3 ? 2 : (int)WindowMode;
        Resolutiondropdown.value = Resolution;
        StoryOptions.value = Story;

        MenuResolutiondropdown.ClearOptions();
        MenuResolutiondropdown.AddOptions(res);
        MenuMainSoundSlider.value = MainVolume;
        MenuWindowmodedropdown.value = (int)WindowMode == 3 ? 2 : (int)WindowMode;
        MenuResolutiondropdown.value = Resolution;
        MenuStoryOptions.value = Story;
        Debug.Log("Options loaded");

        if (!SceneLoader.Instance.IsUnityNull())
            SceneLoader.Instance.LoadFinished();


        foreach (GameObject map in Maps)
        {
            map.SetActive(false);
        }
        StarCounter.text = $"{StarCount}";

        if (SceneManager.GetActiveScene().buildIndex == 1) // Singleplayer
        {
            Debug.Log("SinglePlayer");
            SinglePlayer = true;
            for (int i = 0; i <= SingleUnlockedMaps && i < MapButtons.Count(); i++)
            {
                MapButtons[i].enabled = true;
            }

            NaturePlayerMovement = NaturePlayer.GetComponent<PlayerMovement>();
            NaturePlayerMovement.PlayerAxes = "P1Horizontal";
            NaturePlayerMovement.JumpCode = KeyCode.W;
            NaturePlayerMovement.Active = true;

            Tilemap.GetComponent<TileMapController>().Players.Add(NaturePlayer);
        }
        else // MultiPlayer
        {
            Debug.Log("MultiPlayer");
            SinglePlayer = false;
            for (int i = 0; i <= MultiUnlockedMaps && i < MapButtons.Count(); i++)
            {
                MapButtons[i].enabled = true;
            }

            player1Movement = Player1.GetComponent<PlayerMovement>();
            player1Movement.PlayerAxes = "P1Horizontal";
            player1Movement.JumpCode = KeyCode.W;
            player1Movement.Active = true;
            player2Movement = Player2.GetComponent<PlayerMovement>();
            player2Movement.PlayerAxes = "P2Horizontal";
            player2Movement.JumpCode = KeyCode.UpArrow;
            player2Movement.Active = true;

            Tilemap.GetComponent<TileMapController>().Players.Add(Player1);
            Tilemap.GetComponent<TileMapController>().Players.Add(Player2);
        }

        MenuPausePanel.SetActive(false);
        MenuPauseMenu.SetActive(false);
        MenuSettingsMenu.SetActive(false);
        NotUnlockedPanel.SetActive(false);

    }

    void Update()
    {
        // Gyorsgombok
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentMap > -1) // inGame
            {
                if (PausePanel.activeSelf)
                {
                    if (PauseMenu.activeSelf) Pause(false);
                    if (OptionsPanel.activeSelf) Back();
                }
                else
                {
                    Pause(true);
                }
            }
            else // GameMenu
            {
                if (!NotUnlockedPanel.activeSelf)
                {
                    if (PausePanel.activeSelf)
                    {
                        if (PauseMenu.activeSelf) MenuPause(false);
                        if (MenuSettingsMenu.activeSelf) Back();
                    }
                    else
                    {
                        MenuPause(true);
                    }
                }
                else OKNotUnlocked();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CompletePanel.activeSelf) NextMap();
            if (LostPanel.activeSelf) ResetCurrentMap();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (CompletePanel.activeSelf || LostPanel.activeSelf || PauseMenu.activeSelf) ResetCurrentMap();
        }

        // Karakter váltás
        if (SinglePlayer && CurrentMap > 1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                NaturePlayerMovement.Active = true;
                DesertPlayerMovement.Active = false;
                if (!IcePlayer.IsUnityNull()) IcePlayerMovement.Active = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                NaturePlayerMovement.Active = false;
                DesertPlayerMovement.Active = true;
                if (!IcePlayer.IsUnityNull()) IcePlayerMovement.Active = false;
            }
            if (CurrentMap > 9)
            {
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    NaturePlayerMovement.Active = false;
                    DesertPlayerMovement.Active = false;
                    IcePlayerMovement.Active = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        NaturePlayerMovement.enabled = inGame;
        if (!DesertPlayer.IsUnityNull()) DesertPlayerMovement.enabled = inGame;
        if (!IcePlayer.IsUnityNull()) IcePlayerMovement.enabled = inGame;

        if (!Player1.IsUnityNull()) player1Movement.enabled = inGame;
        if (!Player2.IsUnityNull()) player2Movement.enabled = inGame;

        if (inGame)
        {
            if (SinglePlayer)
            {

            }
            else // MultyPlayer
            {

            }
        }
    }

    #endregion
    #region Menu Functions

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

        CurrentStory = TileMapController.instance.LoadMap(MapNumber + 1, SinglePlayer ? "Single" : "Multy", SinglePlayer ? SinglePlayerStars[MapNumber] : MultiPlayerStars[MapNumber]);

        CurrentMap = MapNumber;

        LoadStory();
    }

    // Story Functions
    public void LoadStory()
    {
        if (CurrentStory != "" && (Story == 2 || Story == 0 && SingleStoryCount < CurrentMap)) // Always or First time
        {
            StoryText.text = CurrentStory;
            StoryPanel.SetActive(true);
        }
        else
        {
            StoryOk();
        }

        if (SinglePlayer) SingleStoryCount = Math.Max(SingleStoryCount, CurrentMap);
        else MultiStoryCount = Math.Max(MultiStoryCount, CurrentMap);
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
        TileMapController.instance.LoadMap(CurrentMap + 1, SinglePlayer ? "Single" : "Multy", SinglePlayer ? SinglePlayerStars[CurrentMap] : MultiPlayerStars[CurrentMap]);

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

        MenuPauseMenu.SetActive(false);

        foreach (GameObject map in Maps)
        {
            map.SetActive(false);
        }

        if (SinglePlayer)
            for (int i = 0; i <= SingleUnlockedMaps && i < MapButtons.Count(); i++)
            {
                MapButtons[i].enabled = true;
            }
        else
            for (int i = 0; i <= MultiUnlockedMaps && i < MapButtons.Count(); i++)
            {
                MapButtons[i].enabled = true;
            }

        StarCounter.text = $"{StarCount}";

        CurrentMap = -1;
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
    public void ResolutionChange(int v)
    {
        Resolution = v;
        SettingsChange();
    }
    public void WindowModeChange(int v)
    {
        WindowMode = (FullScreenMode)(v == 2 ? 3 : v);
        SettingsChange();
    }
    public void SettingsChange()
    {
        string[] seged = Resolutiondropdown.options[Resolution].text.Split('x');
        int[] screensize =
        {
            Convert.ToInt32(seged[0]),
            Convert.ToInt32(seged[1])
        };

        Screen.SetResolution(screensize[0], screensize[1], WindowMode);
    }
    public void StorySetting(int v)
    {
        Story = v;
    }
    public void Back()
    {
        OptionsMenu(false);
    }

    #endregion
    #region InGame Functions

    // Player Create / Destroy
    public GameObject CreateNextPlayer(int number)
    {
        GameObject newPlayer;
        switch (number)
        {
            case 1:
                newPlayer = Instantiate(PlayerModels[1], PlayersObject);
                DesertPlayer = newPlayer;
                DesertPlayerMovement = DesertPlayer.GetComponent<PlayerMovement>();
                DesertPlayerMovement.PlayerAxes = "P1Horizontal";
                DesertPlayerMovement.JumpCode = KeyCode.W;
                return newPlayer;
            case 2:
                newPlayer = Instantiate(PlayerModels[2], PlayersObject);
                IcePlayer = newPlayer;
                IcePlayerMovement = IcePlayer.GetComponent<PlayerMovement>();
                IcePlayerMovement.PlayerAxes = "P1Horizontal";
                IcePlayerMovement.JumpCode = KeyCode.W;
                return newPlayer;
            case 3:
            default:
                return new();
        }
    }
    public void DestroyPlayer(int number)
    {
        switch (number)
        {
            case 1:
                DesertPlayerMovement = null;
                Destroy(DesertPlayer);
                DesertPlayer = null;
                break;
            case 2:
                IcePlayerMovement = null;
                Destroy(IcePlayer);
                IcePlayer = null;
                break;
            case 3:
            default:
                break;
        }
    }

    // Complete Panel Functions
    public void CompleteLevel()
    {
        if (!NaturePlayerMovement.Done || (!DesertPlayerMovement.IsUnityNull() && !DesertPlayerMovement.Done) || (!IcePlayerMovement.IsUnityNull() && !IcePlayerMovement.Done))
            return;

        if (SinglePlayer)
        {
            switch (CurrentMap)
            {
                case 3:
                    if (StarCount >= 6)
                        SingleUnlockedMaps = Math.Max(SingleUnlockedMaps, CurrentMap + 1);
                    break;
                case 8:
                    if (StarCount >= 20)
                        SingleUnlockedMaps = Math.Max(SingleUnlockedMaps, CurrentMap + 1);
                    break;
                case 11:
                    if (StarCount >= 30)
                        SingleUnlockedMaps = Math.Max(SingleUnlockedMaps, CurrentMap + 1);
                    break;
                default:
                    SingleUnlockedMaps = Math.Max(SingleUnlockedMaps, CurrentMap + 1);
                    break;
            }
        }
        else // Multyplayer
        {
            MultiUnlockedMaps = Math.Max(MultiUnlockedMaps, CurrentMap + 1);
        }

        SaveOptions();

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

        if (CurrentMap > (SinglePlayer ? SingleUnlockedMaps : MultiUnlockedMaps))
        {
            NotUnlocked();
        }
        else
        {
            CurrentStory = TileMapController.instance.LoadMap(CurrentMap + 1, SinglePlayer ? "Single" : "Multy", SinglePlayer ? SinglePlayerStars[CurrentMap] : MultiPlayerStars[CurrentMap]);
            LoadStory();
        }

        CompletePanel.SetActive(false);
        PausePanel.SetActive(false);
    }
    public void NotUnlocked()
    {
        int star = 0;
        if (SinglePlayer)
        {
            switch (CurrentMap)
            {
                case 3:
                    star = 6;
                    break;
                case 8:
                    star = 20;
                    break;
                case 11:
                    star = 30;
                    break;
            }
        }
        else // Multyplayer
        {

        }

        NotUnlockedText.text = $"Nincs elég csillagod. A {CurrentMap + 1}. pálya feloldásához {star} csillag kell összesen!";
        NotUnlockedPanel.SetActive(true);
        ChangeToMainCamera();
    }
    public void OKNotUnlocked()
    {
        NotUnlockedPanel.SetActive(false);
    }

    // Star
    public void Collect(int starid)
    {
        if (SinglePlayer)
        {
            SinglePlayerStars[CurrentMap][starid] = 1;
        }
        else
        {
            MultiPlayerStars[CurrentMap][starid] = 1;
        }
    }
    public int CountStars()
    {
        int stars = 0;
        if (SinglePlayer) SinglePlayerStars.ForEach(s => stars += s.Collected);
        else MultiPlayerStars.ForEach(s => stars += s.Collected);
        return stars;
    }

    #endregion
}
