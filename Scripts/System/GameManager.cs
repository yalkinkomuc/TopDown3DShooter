using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    [Header("Settings")]
    public bool friendlyFire;

    [Space]

    public bool quickStart;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>();
    }

    public void GameStart()
    {
        SetDefaultWeaponsForPlayer();
        //LevelGenerator.instance.InitializieGeneration();

        //leveli yarattıktan sonra Levelgenerator içinden seçilen görevi başlatıyoruz.
    }
    public void SetDefaultWeaponsForPlayer()
    {
        List<WeaponData> newList = UI.instance.WeaponSelection.SelectedWeaponData();
        player.weaponController.SetDefaultWeapon(newList);
    }

    public void GameOver()
    {
        TimeManager.instance.SlowMotionFor(1);
        UI.instance.ShowGameOverUI();
        CameraManager.instance.ChangeCameraDistance(4);
    }

    //public void RestartScene() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

    public void GameCompleted ()
    {
        UI.instance.ShowVictoryScreenUI();
        ControlsManager.instance.controls.Character.Disable();
        player.health.currentHealth += 99999; // oyuncu son saniyede ölmesin diye
    }
}
