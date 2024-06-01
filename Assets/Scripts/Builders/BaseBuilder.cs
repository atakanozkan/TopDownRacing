using System;
using System.Collections.Generic;
using TDR.Managers;
using TDR.Models.Pool;
using UnityEngine;

namespace TDR.Models
{
    public abstract class BaseBuilder: MonoBehaviour
    {
        public int maxItemTypeCount = 5;
        protected abstract PoolItemType[] RequiredPoolItemTypes { get; }

        public void CreateItems()
        {
            foreach (PoolItem poolItem in PoolManager.Instance.prefabList)
            {
                if (!DoesMatchType(RequiredPoolItemTypes, poolItem.GetPoolItemType()))
                {
                    continue;
                }

                for (int index = 0; index < maxItemTypeCount; index++)
                {
                    PoolItem instantiatedItem = Instantiate(poolItem);
                    PoolManager.Instance.AddToAvailable(instantiatedItem);
                }
            }
        }

        public bool DoesMatchType(PoolItemType[] poolItemTypes, PoolItemType targetpoolItemType)
        {
            foreach (PoolItemType itemType in poolItemTypes)
            {
                if (itemType == targetpoolItemType)
                {
                    return true;
                }
            }
            return false;
        }
    }

}

