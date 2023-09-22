using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public enum ItemType {Potion}
public enum ItemEffectedStatus {HP, MP}

[System.Serializable]
public class GeneratedItem{
    public int type;
    public int effectedStatus;
    public float grants;
    public float coolDown;
    public int amount;
    // public static bool isThrowable;
    public float remainedTimeToActive;
    public int remainedItem;

    public void SetValues(int sourceType, int sourceEffectedStatus, float sourceGrants, float sourceCoolDown, int sourceAmount)
    {
        this.type = sourceType;
        this.effectedStatus = sourceEffectedStatus;
        this.grants = sourceGrants;
        this.coolDown = sourceCoolDown;
        this.amount = sourceAmount;
        this.remainedTimeToActive = 0f;
        this.remainedItem = amount;
    }

    public void ResetItemUsed()
    {
        this.remainedTimeToActive = 0f;
        this.remainedItem = amount;
    }

    public void itemUsed()
    {
        this.remainedTimeToActive = coolDown;
        if (this.remainedItem != 0)
        {
            this.remainedItem -= 1;
        }
    }

    public void CountDownTheCoolDown(float howMuch)
    {
        this.remainedTimeToActive = Mathf.Clamp(remainedTimeToActive-Time.deltaTime,0f,coolDown);
    }

    public bool CanIUseThisItem()
    {
        bool yesOrNo = false;
        if(this.remainedTimeToActive == 0 || this.remainedItem != 0)
        {
            yesOrNo = true;
        }
        return yesOrNo;
    }
    public bool IsItSame(int compareType, int compareStatus, float compareGrants, float compareCoolDown, int compareAmount)
    {
        bool yesOrNo = false;
        if(this.type == compareType || this.effectedStatus == compareStatus ||
        this.grants == compareGrants || this.coolDown == compareCoolDown ||
        this.amount == compareAmount)
        {
            yesOrNo = true;
        }
        return yesOrNo;
    }
}

public class ItemGenerator : MonoBehaviour
{
    [Header("Item Value Threshold")]
    public List<GeneratedItem> currentlyGeneratedItems = new List<GeneratedItem>();
    public int[] typeMinMax = {0, System.Enum.GetValues(typeof(ItemType)).Length};
    public int[] effectedStatusMinMax = {0, System.Enum.GetValues(typeof(ItemEffectedStatus)).Length};
    public float[] grantsMinMax = {-100f, 100f};
    public float[] coolDownMinMax = {0f,600f};
    public int[] amountMinMax = {0,100};
    bool canChangeItemConfiguration = true;

    [Header("Manually Generate The Item")]
    public ItemType type;
    public ItemEffectedStatus effectedStatus;
    public float grants;
    public float coolDown;
    public int amount;
    // public bool isThrowable = false;
    // Start is called before the first frame update

    void FixedUpdate() {
        TickingCoolDown();
    }

    public void resetAllUsedItem()
    {
        for(int i=0; i<currentlyGeneratedItems.Count;i++)
        {
            currentlyGeneratedItems[i].ResetItemUsed();
        }
    }

    void TickingCoolDown(){
        for(int i=0; i<currentlyGeneratedItems.Count;i++)
        {
            currentlyGeneratedItems[i].CountDownTheCoolDown(0.1f);
        }
    }

    public int IsThisItemGeneratedEarlier(int sourceType, int sourceEffectedStatus, float sourceGrants, float sourceCoolDown, int sourceAmount)
    {
        int isAlreadyGenerated = -1;
        for(int i=0; i<currentlyGeneratedItems.Count;i++)
        {
            if(currentlyGeneratedItems[i].IsItSame(
                sourceType, 
                sourceEffectedStatus, 
                sourceGrants, 
                sourceCoolDown, 
                sourceAmount
                ))
            {
                isAlreadyGenerated = i;
            }
        }
        return isAlreadyGenerated;
    }

    public int GenerateItemWithParameters(int sourceType, int sourceEffectedStatus, float sourceGrants, float sourceCoolDown, int sourceAmount)
    {
        sourceType = Mathf.Min(typeMinMax[1], sourceType);
        sourceType = Mathf.Max(typeMinMax[0], sourceType);
        
        sourceEffectedStatus = Mathf.Min(effectedStatusMinMax[1], sourceEffectedStatus);
        sourceEffectedStatus = Mathf.Max(effectedStatusMinMax[0], sourceEffectedStatus);
        
        sourceGrants = Mathf.Min(grantsMinMax[1], sourceGrants);
        sourceGrants = Mathf.Max(grantsMinMax[0], sourceGrants);
        
        sourceCoolDown = Mathf.Min(coolDownMinMax[1], sourceCoolDown);
        sourceCoolDown = Mathf.Max(coolDownMinMax[0], sourceCoolDown);
        
        sourceAmount = Mathf.Min(amountMinMax[1], sourceAmount);
        sourceAmount = Mathf.Max(amountMinMax[0], sourceAmount);

        int isAlreadyGenerated = IsThisItemGeneratedEarlier(sourceType, sourceEffectedStatus, sourceGrants, sourceCoolDown, sourceAmount);
        isAlreadyGenerated = GenerateItem(isAlreadyGenerated, sourceType, sourceEffectedStatus, sourceGrants, sourceCoolDown, sourceAmount);
        return 0;
    }

    public int GenerateItem(int isAlreadyGenerated, int sourceType, int sourceEffectedStatus, float sourceGrants, float sourceCoolDown, int sourceAmount)
    {
        if (isAlreadyGenerated == -1)
        {
            GeneratedItem tmpGenedItem = new GeneratedItem();
            tmpGenedItem.SetValues(
                sourceType,
                sourceEffectedStatus, 
                sourceGrants, 
                sourceCoolDown, 
                sourceAmount
            );
            currentlyGeneratedItems.Add(tmpGenedItem);
        }
        else
        {
            if (canChangeItemConfiguration)
            {
                Debug.Log($"The Item is already generated but changes the configuration : {isAlreadyGenerated}");
                currentlyGeneratedItems[isAlreadyGenerated].SetValues(
                    sourceType,
                    sourceEffectedStatus, 
                    sourceGrants, 
                    sourceCoolDown, 
                    sourceAmount
                );
            }
            else
            {
                Debug.Log($"The Item is already generated : {isAlreadyGenerated}");
            }
        }
        return isAlreadyGenerated;
    }

    public void UseItem(int whichItemGoingToUse, AbstractAgent target)
    {
        AbstractStatus status = target._status;
        if(whichItemGoingToUse >= 0)
        {
            if(this.currentlyGeneratedItems[whichItemGoingToUse].CanIUseThisItem()){
                if(this.currentlyGeneratedItems[whichItemGoingToUse].effectedStatus == (int)ItemEffectedStatus.HP)
                {
                    status.health.current += (int)this.currentlyGeneratedItems[whichItemGoingToUse].grants;
                    // target.SetHealth(target.GetHealth()+(int)this.currentlyGeneratedItems[whichItemGoingToUse].grants);
                }
                else
                {
                    status.mana.current += (int)this.currentlyGeneratedItems[whichItemGoingToUse].grants;
                }
                this.currentlyGeneratedItems[whichItemGoingToUse].itemUsed();
            }
            else
            {
                Debug.Log($"Check the cooldown or remaining amount of the item : {this.currentlyGeneratedItems[whichItemGoingToUse].remainedTimeToActive}, {this.currentlyGeneratedItems[whichItemGoingToUse].remainedItem}");
            }

        }
        else
        {
            Debug.Log($"Their is no Usable Item : {whichItemGoingToUse}");
        }

    }
}

