using System.Collections;
using System.Collections.Generic;
using TDR.Managers;
using TDR.Models;
using TDR.Models.NPCCar;
using TDR.Models.Pool;
using TDR.Models.Road;
using UnityEngine;

namespace TDR.Builder
{
    public class NPCCarBuilder : BaseBuilder
    {
        protected override PoolItemType[] RequiredPoolItemTypes => new[]
        {
        PoolItemType.Ambulance,
        PoolItemType.Minivan,
        PoolItemType.Police,
        PoolItemType.Truck
        };

        public NPCCar BuildCar(PoolItemType itemType)
        {
            if (!DoesMatchType(RequiredPoolItemTypes, itemType))
            {
                return null;
            }
            PoolItem poolItem = PoolManager.Instance.GetFromPool(itemType, null);
            NPCCar car = poolItem.GetComponent<NPCCar>();
            return car;
        }

        public void DestroyCar(NPCCar car)
        {
            PoolItem poolItem = car.GetComponent<PoolItem>();
            PoolManager.Instance.ResetPoolItem(poolItem);
        }

        public PoolItemType[] GetItemTypes()
        {
            return RequiredPoolItemTypes;
        }
    }

}
