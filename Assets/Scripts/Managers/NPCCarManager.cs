using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDR.Builder;
using TDR.Controllers;
using TDR.Models.Pool;
using System.Linq;
using TDR.Models.Player;
using TDR.Models.NPCCar;
using TDR.Models;
using TDR.Helpers;
using TDR.Models.Game;

namespace TDR.Managers
{
    public class NPCCarManager : MonoBehaviour
    {
        [SerializeField]
        private NPCCarBuilder carBuilder;
        [SerializeField]
        private NPCCarController carController;
        [SerializeField]
        private PlayerController playerController;
        [SerializeField]
        private Material lightMaterial;
        [SerializeField]
        private RoadManager roadManager;
        [SerializeField]
        private List<NPCCarItemRule> carItemRules;

        private List<NPCCarItem> listOfCarItemsOnRoad = new List<NPCCarItem>();
        private Dictionary<int, int> totalLaneIndexDictionary = new Dictionary<int, int>();
        

        public float npcCarSpawnPointY = -2.5f;
        public float npcCarOverlapYOffset = 1.0f;
        public float lineSpaceOffset = 15.0f;

        // Prevent the cars to go in same lane everytime (needed for shuffle)
        public int laneRegulationRuleWthMinimumCars = 2; 

        private bool canGenerateCars = false;

        void LateUpdate()
        {
            if (canGenerateCars)
            {
                int missingCount = CheckIfCarNeeded();
                if (missingCount != 0)
                {
                    GenerateCars(missingCount);
                }
            }
        }

        public void BuildInitialItems()
        {
            carBuilder.CreateItems();
        }

        public int CheckIfCarNeeded()
        {
            int currentCarNumber = carController.GetCurrentCars().Count;
            int maxCarByRule = GetMaxCarFromCarItemRules()+1;

            if (currentCarNumber >= maxCarByRule)
            {
                return 0;
            }
            return maxCarByRule - currentCarNumber;
        }

        void GenerateCars(int missingCount)
        {
            bool checkLightsOn = GameManager.Instance.GetDayTimeManager().IsNightTime();
            List<LaneInfo> lanePositions = GetLanePositions();
            List<NPCCar> listTempCars = new List<NPCCar>();
            for (int i = 0; i < missingCount; i++)
            {
                PoolItemType typeToBuild = SelectCarType();
                if(typeToBuild == PoolItemType.None)
                {
                    continue;
                }

                int totalCarTypeOnRoad = GetCarItemValueOnRoad(typeToBuild);
                if (totalCarTypeOnRoad != -1 &&
                    totalCarTypeOnRoad < GetMaxCountByRule(typeToBuild))
                {
                    int laneIndex = GetLaneIndexWithMinimumCars();

                    if(laneIndex == -1) // Wait for lanes to be empty
                    {
                        continue;
                    }

                    NPCCar car = carBuilder.BuildCar(typeToBuild);
                    if (car != null)
                    {
                        Material randomMaterial = GetRandomMaterialForCarType(typeToBuild);
                        if (randomMaterial != null)
                        {
                            car.GetRenderer().materials = new Material[] { randomMaterial , lightMaterial};
                        }
                        Vector3 lastCarPosition = GetLastCarPositionInLane(laneIndex, lanePositions[laneIndex].Position.z);

                        Vector3 carPosition = new Vector3(
                            lanePositions[laneIndex].Position.x,
                            lanePositions[laneIndex].Position.y,
                            lastCarPosition.z + UnityEngine.Random.Range(15.0f, 80.0f) + (lineSpaceOffset)
                        );
                       
                        car.transform.position = carPosition;
                        car.isMovingBackward = lanePositions[laneIndex].IsMovingBackward;
                        car.SetLaneIndex(laneIndex);
                        car.transform.rotation = car.isMovingBackward ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

                        carController.AddNewCarToList(car);
                        SetCarItemValueOnRoad(typeToBuild, totalCarTypeOnRoad + 1);
                        totalLaneIndexDictionary[laneIndex] += 1;
                        if (checkLightsOn)
                        {
                            car.TurnOnTheLigths();
                        }
                        else
                        {
                            car.TurnOffTheLigths();
                        }
                        listTempCars.Add(car);
                    }
                }
            }

            foreach(NPCCar car in listTempCars)
            {
                 ModifyCarPositionIfOverlaps(car);
            }
        }

        private void ModifyCarPositionIfOverlaps(NPCCar car)
        {
            float incrementDistance = 10.0f + car.GetLength();

            while (IsOverlapping(car))
            {
                car.transform.position += new Vector3(0, 0, incrementDistance); 
            }
         
        }

        private bool IsOverlapping(NPCCar car)
        {
            Vector3 halfExtents = car.simplifiedCollider.size / 2;
            Vector3 boxPosition = car.simplifiedCollider.transform.position + car.simplifiedCollider.center;
            Quaternion boxOrientation = car.transform.rotation;

            RaycastHit[] hitColliders = Physics.BoxCastAll(boxPosition, halfExtents, car.transform.forward, boxOrientation, 0.01f);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.collider.gameObject.layer == LayerMask.GetMask("NPCCar") && hitCollider.collider.gameObject != car.gameObject)
                {
                    Debug.Log($"npc car overlaps: hitting collider {hitCollider.collider.GetComponentInParent<PoolItem>().GetPoolItemType()} on {car.GetComponent<PoolItem>().GetPoolItemType()}");
                    return true;
                }
            }
            return false;
        }


        public List<LaneInfo> GetLanePositions()
        {
            List<LaneInfo> laneInfos = new List<LaneInfo>();
            Vector3 carPos = playerController.GetPlayerCurrentPosition();
            List<float> laneList = roadManager.laneList;

            for (int i = 0; i < laneList.Count; i++)
            {
                float laneX = laneList[i];
                Vector3 lanePosition = new Vector3(laneX, npcCarSpawnPointY, carPos.z);

                bool isMovingBackward = (i == 0 || i == 1);

                laneInfos.Add(new LaneInfo { IsMovingBackward = isMovingBackward, Position = lanePosition });
            }

            return laneInfos;
        }

        private Vector3 GetLastCarPositionInLane(int laneIndex, float defaultZ)
        {
            NPCCar lastCar = null;
            float furthestZ = defaultZ;

            foreach (var car in carController.GetCurrentCars())
            {
                if (car.GetLaneIndex() == laneIndex)
                {
                    float carZ = car.transform.position.z;
                    if (carZ > furthestZ)
                    {
                        furthestZ = carZ;
                        lastCar = car;
                    }
                }
            }

            if (lastCar != null)
            {
                return lastCar.transform.position;
            }
            else
            {
                return new Vector3(0, 0, defaultZ);
            }
        }

        private void TriggerRemoveCar(NPCCar car)
        {
            PoolItem poolItem = car.GetComponent<PoolItem>();
            if (poolItem != null && DoesCarItemExistOnRoad(poolItem.GetPoolItemType()))
            {
                int totalCarItemOnRoad = GetCarItemValueOnRoad(poolItem.GetPoolItemType());
                SetCarItemValueOnRoad(poolItem.GetPoolItemType(), totalCarItemOnRoad - 1);
                if (totalLaneIndexDictionary.ContainsKey(car.GetLaneIndex()))
                {
                    totalLaneIndexDictionary[car.GetLaneIndex()] -= 1; 
                }
                carBuilder.DestroyCar(car);
            }
        }


        private PoolItemType SelectCarType()
        {
            List<PoolItemType> availableTypes = new List<PoolItemType>();
            foreach (var npcCarItem in listOfCarItemsOnRoad)
            {
                if (npcCarItem.Value < GetMaxCountByRule(npcCarItem.Key))
                {
                    availableTypes.Add(npcCarItem.Key);
                }
            }

            if (availableTypes.Count > 0)
            {
                int index = Random.Range(0, availableTypes.Count);
                return availableTypes[index];
            }

            return availableTypes.Count > 0 ? availableTypes[Random.Range(0, availableTypes.Count)] : PoolItemType.None;
        }

        private int GetMaxCountByRule(PoolItemType type)
        {
            for(int i = 0; i < carItemRules.Count; i++)
            {
                if (carItemRules[i].Key == type)
                {
                    return carItemRules[i].Value;
                }
            }

            return 0;
        }

        private int GetLaneIndexWithMinimumCars()
        {
            int minCount = totalLaneIndexDictionary.Values.Min();
            List<int> leastPopulatedLanes = new List<int>();

            foreach (var lane in totalLaneIndexDictionary)
            {
                if (lane.Value == minCount)
                {
                    leastPopulatedLanes.Add(lane.Key);
                }
            }

            if (leastPopulatedLanes.Count < laneRegulationRuleWthMinimumCars)
            {
                return -1;
            }

            if (leastPopulatedLanes.Count > 0)
            {
                return leastPopulatedLanes[Random.Range(0, leastPopulatedLanes.Count)];
            }

            return Random.Range(0, roadManager.laneList.Count);
        }

        public void BuildControlDictionaries()
        {
            foreach (PoolItemType type in carBuilder.GetItemTypes())
            {
                listOfCarItemsOnRoad.Add(new NPCCarItem { Key = type, Value = 0 });
            }
            for (int i = 0; i < roadManager.laneList.Count; i++)
            {
                totalLaneIndexDictionary.Add(i, 0);
            }
        }

        private int GetCarItemValueOnRoad(PoolItemType type)
        {
            foreach (var item in listOfCarItemsOnRoad)
            {
                if (item.Key == type)
                {
                    return item.Value;
                }
            }
            Debug.LogWarning($"Key {type} not found in the list.");
            return -1;
        }

        private Material GetRandomMaterialForCarType(PoolItemType type)
        {
            foreach (var rule in carItemRules)
            {
                if (rule.Key == type && rule.Materials.Count > 0)
                {
                    int randomIndex = Random.Range(0, rule.Materials.Count);
                    return rule.Materials[randomIndex];
                }
            }
            return null;
        }

        private int GetMaxCarFromCarItemRules()
        {
            return carItemRules.Select(item => item.Value).Sum();
        }

        private void SetCarItemValueOnRoad(PoolItemType type,int newValue)
        {
            foreach (var item in listOfCarItemsOnRoad)
            {
                if (item.Key == type)
                {
                    item.Value = newValue;
                    break;
                }
            }
        }

        private bool DoesCarItemExistOnRoad(PoolItemType type)
        {
            foreach (var item in listOfCarItemsOnRoad)
            {
                if (item.Key == type && item.Value > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void ControlIfGenerateCars(GameState state)
        {
            if (state == GameState.GameOver)
            {
                canGenerateCars = false;
                ResetNpcCars();
            }
            if(state == GameState.Playing)
            {
                canGenerateCars = true;
            }
            else
            {
                canGenerateCars = false;
            }
        }

        public void ApplySettingChangesToCarItemRules(GameSettingsData data)
        {
            foreach (NPCCarItemRule itemRule in carItemRules)
            {
                
                if(itemRule.Key == PoolItemType.Police)
                {
                    itemRule.Value = data.PoliceCarNumber;
                }
                else if (itemRule.Key == PoolItemType.Truck)
                {
                    itemRule.Value = data.TruckNumber;
                }
                else if(itemRule.Key == PoolItemType.Ambulance)
                {
                    itemRule.Value = data.AmbulanceNumber;
                }
                else if (itemRule.Key == PoolItemType.Minivan)
                {
                    itemRule.Value = data.MinivanNumber;
                }
            }
        }

        public void ResetNpcCars()
        {
            carController.ResetAllCars();
            listOfCarItemsOnRoad = new List<NPCCarItem>();
            totalLaneIndexDictionary = new Dictionary<int, int>();
        }

        private void OnEnable()
        {
            carController.onCarDisappers += TriggerRemoveCar;
            GameManager.Instance.OnGameStateChanged += ControlIfGenerateCars;
            GameManager.Instance.OnGameSettingsChanged += ApplySettingChangesToCarItemRules;
        }

        private void OnDisable()
        {
            carController.onCarDisappers -= TriggerRemoveCar;
            if(GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= ControlIfGenerateCars;
                GameManager.Instance.OnGameSettingsChanged -= ApplySettingChangesToCarItemRules;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var car in carController.GetCurrentCars())
            {
                Vector3 halfExtents = car.simplifiedCollider.size / 2;
                Vector3 boxPosition = car.simplifiedCollider.transform.position;
                Gizmos.matrix = car.transform.localToWorldMatrix;
                Gizmos.DrawWireCube(car.simplifiedCollider.center, car.simplifiedCollider.size);
            }
        }
    }

}
