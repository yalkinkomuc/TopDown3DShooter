using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Car Delivery Mission ", menuName = "Missions/Car Delivery")]

public class MissionCarDelivery : Mission
{

    private bool carWasDelivered;
    [SerializeField] private GameObject car;
    private Car_Controller carController;
    public override void StartMission()
    {
        

        FindObjectOfType<MissionObjectCarDeliveryZone>(true).gameObject.SetActive(true);

        string missionText = "Çalışan bir araç bul ve aracı tahliye noktasına götürdükten sonra uçağa bin.";
        string missionDetails = "";

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);

        carWasDelivered = false;
        MissionObjectCarToDeliver.OnCarDelivery += CarDeliveryCompleted;

        Car_Controller[] cars = FindObjectsOfType<Car_Controller>();

        foreach (var car in cars)
        {
            car.AddComponent<MissionObjectCarToDeliver>();
            car.gameObject.SetActive(true);
        }

    }

    public override bool MissionCompleted()
    {
        return carWasDelivered;
    }

    private void CarDeliveryCompleted()
    {
        carWasDelivered = true;
        MissionObjectCarToDeliver.OnCarDelivery -= CarDeliveryCompleted;

        UI.instance.inGameUI.UpdateMissionInfo("Tahliye noktasına git.");
    }

}
