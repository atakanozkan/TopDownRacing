using System.Collections;
using System.Collections.Generic;
using TDR.Managers;
using TDR.Models;
using TDR.Models.Pool;
using TDR.Models.Road;
using UnityEngine;

namespace TDR.Builder
{
    public class RoadBuilder : BaseBuilder
    {
        protected override PoolItemType[] RequiredPoolItemTypes => new[]
{
        PoolItemType.RoadCity1,
        PoolItemType.RoadCity2,
        PoolItemType.RoadCity3
        };

        public int nextRoadIndexOnTypes = 0;

        public List<Road> BuildInitialRoad()
        {
            PoolItem poolItemRoadCity1 = PoolManager.Instance.GetFromPool(PoolItemType.RoadCity1, this.transform);
            PoolItem poolItemRoadCity2 = PoolManager.Instance.GetFromPool(PoolItemType.RoadCity2, this.transform);
            PoolItem poolItemRoadCity3 = PoolManager.Instance.GetFromPool(PoolItemType.RoadCity3, this.transform);

            Road roadCity1 = poolItemRoadCity1.GetComponent<Road>();
            Road roadCity2 = poolItemRoadCity2.GetComponent<Road>();
            Road roadCity3 = poolItemRoadCity3.GetComponent<Road>();

            List<Road> initialRoadList = new List<Road>();
            initialRoadList.Add(roadCity1);
            initialRoadList.Add(roadCity2);
            initialRoadList.Add(roadCity3);

            float previousDistance = 0;
            for(int i = 0; i< initialRoadList.Count; i++)
            {
                if(i == 0)
                {
                    previousDistance = initialRoadList[0].GetRoadWidth();
                    initialRoadList[0].transform.position =
                       new Vector3(initialRoadList[0].transform.position.x, initialRoadList[0].transform.position.y,0);
                }
                else
                {
                    initialRoadList[i].transform.position =
                        new Vector3(
                            initialRoadList[i-1].transform.position.x,
                            initialRoadList[i-1].transform.position.y,
                            initialRoadList[i-1].transform.position.z - previousDistance
                            );
                    previousDistance = initialRoadList[i].GetRoadWidth();
                }
            }

            return initialRoadList;
        }

        public Road BuildNextRoad(Road lastRoadOnList)
        {
            PoolItem poolItemRoadCity = PoolManager.Instance.GetFromPool(RequiredPoolItemTypes[nextRoadIndexOnTypes], this.transform);
            Road roadCity = poolItemRoadCity.GetComponent<Road>();

            float previousDistance = lastRoadOnList.GetRoadWidth();

            roadCity.transform.position = new Vector3(
                            lastRoadOnList.transform.position.x,
                            lastRoadOnList.transform.position.y,
                            lastRoadOnList.transform.position.z - previousDistance
                            );

            nextRoadIndexOnTypes = (nextRoadIndexOnTypes + 1) % RequiredPoolItemTypes.Length;
            return roadCity;
        }

        public void DestroyRoad(Road road)
        {
            PoolItem poolItem = road.GetComponent<PoolItem>();
            PoolManager.Instance.ResetPoolItem(poolItem);
        }
    }

}
