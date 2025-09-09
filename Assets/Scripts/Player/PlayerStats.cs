using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
   CharacterScriptableObjects characterData;

   
   float currentHealth;
   
   float currentRecovery;
   
   float currentMoveSpeed;
   
   float currentMight;
   
   float currentProjectileSpeed;
   
   float currentMagnet;

   #region Current Stats Properties

   public float CurrentHealth
   {
      get { return currentHealth; }
      set 
      {
         //Checks if value has changed
         if (currentHealth != value)
         {
            currentHealth = value;
            if(GameManager.instance != null)
            {
               GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
            }
         }
      }
   }

   public float CurrentRecovery
   {
      get { return currentRecovery; }
      set 
      {
         //Checks if value has changed
         if (currentRecovery != value)
         {
            currentRecovery = value;
            if(GameManager.instance != null)
            {
               GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
            }
         }
      }
   }

   public float CurrentMoveSpeed
   {
      get { return currentMoveSpeed; }
      set 
      {
         //Checks if value has changed
         if (currentMoveSpeed != value)
         {
            currentMoveSpeed = value;
            if(GameManager.instance != null)
            {
               GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
            }
         }
      }
   }

   public float CurrentMight
   {
      get { return currentMight; }
      set 
      {
         //Checks if value has changed
         if (currentMight != value)
         {
            currentMight = value;
            if(GameManager.instance != null)
            {
               GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
            }
         }
      }
   }

   public float CurrentProjectileSpeed
   {
      get { return currentProjectileSpeed; }
      set 
      {
         //Checks if value has changed
         if (currentProjectileSpeed != value)
         {
            currentProjectileSpeed = value;
            if(GameManager.instance != null)
            {
               GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
            }
         }
      }
   }

   public float CurrentMagnet
   {
      get { return currentMagnet; }
      set 
      {
         //Checks if value has changed
         if (currentMagnet != value)
         {
            currentMagnet = value;
            if(GameManager.instance != null)
            {
               GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;
            }
         }
      }
   }
   #endregion

   [Header("Experience/Level")]
   public int experience = 0;
   public int level = 1;
   public int experienceCap;

   [System.Serializable]
   public class LevelRange
   {
      public int startLevel;
      public int endLevel;
      public int experienceCapIncrease;
   }

   [Header("I-Frames")]
   public float invincibilityDuration;
   float invincibilityTimer;
   bool isInvincible;

   public List<LevelRange> levelRanges;

   InventoryManager inventory;
   public int weaponIndex;
   public int passiveItemIndex;

   public GameObject secondWeaponTest;
   public GameObject firstPassiveItemTest, secondPassiveItemTest;

    void Awake()
   {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();
        
        CurrentHealth = characterData.MaxHealth;
        CurrentRecovery = characterData.Recovery;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentMight = characterData.Might;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMagnet = characterData.Magnet;

        //Spawn starting weapon
        SpawnWeapon(characterData.StartingWeapon);
        //SpawnWeapon(secondWeaponTest);
        SpawnPassiveItem(firstPassiveItemTest);
        SpawnPassiveItem(secondPassiveItemTest);
   }
   void Start()
   {
      experienceCap = levelRanges[0].experienceCapIncrease;

      GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
      GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
      GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
      GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
      GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
      GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;

      GameManager.instance.AssignChosenCharacterUI(characterData);
   }

   public void IncreaseExperience(int amount)
   {
      experience += amount;
      LevelUpChecker();
   }

   void LevelUpChecker()
   {
      if(experience >= experienceCap)
      {
         level++;
         experience -= experienceCap;

         int experienceCapIncrease = 0;
         foreach(LevelRange range in levelRanges)
         {
            if(level >= range.startLevel && level <= range.endLevel)
            {
               experienceCapIncrease = range.experienceCapIncrease;
               break;
            }
         }
         experienceCap += experienceCapIncrease;

         GameManager.instance.StartLevelUp();
      }
   }

   void Update()
   {
      if(invincibilityTimer > 0)
      {
         invincibilityTimer -= Time.deltaTime;
      }
      else if (isInvincible)
      {
         isInvincible = false;
      }

      Recover();
   }

   public void TakeDamage(float dmg)
   {
      if(!isInvincible)
      {
         CurrentHealth -= dmg;

         invincibilityTimer = invincibilityDuration;
         isInvincible = true;

         if(CurrentHealth <= 0)
         {
            Kill();
         }
      }
   }

   public void Kill()
   {
      if(!GameManager.instance.isGameOver)
      {
         GameManager.instance.AssignLevelReachedUI(level);
         GameManager.instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponUISlots, inventory.passiveItemUISlots);
         GameManager.instance.GameOver();
      }
   }

   public void RestoreHealth(float amount)
   {
      if(CurrentHealth < characterData.MaxHealth)
      {
         CurrentHealth += amount;

         if(CurrentHealth > characterData.MaxHealth)
         {
            CurrentHealth = characterData.MaxHealth;
         }
      }
   }

   void Recover()
   {
      if(CurrentHealth < characterData.MaxHealth)
      {
         CurrentHealth += CurrentRecovery * Time.deltaTime;

         //limits recovery to maxHealth number
         if(CurrentHealth > characterData.MaxHealth)
         {
            CurrentHealth = characterData.MaxHealth;
         }
      }
   }

   public void SpawnWeapon(GameObject weapon)
   {
      //Checks if the slots are full, if so it returns
      if(weaponIndex >= inventory.weaponSlots.Count - 1) //-1 since starts from 0
      {
         Debug.LogError("Inventory slots already full");
         return;
      }

      //Spawn starting weapon
      GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
      spawnedWeapon.transform.SetParent(transform); //Sets weapon as a child of the player
      inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());

      weaponIndex++;
   }

   public void SpawnPassiveItem(GameObject passiveItem)
   {
      //Checks if the slots are full, if so it returns
      if(passiveItemIndex >= inventory.passiveItemSlots.Count - 1) //-1 since starts from 0
      {
         Debug.LogError("Inventory slots already full");
         return;
      }

      //Spawn starting passive Item
      GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
      spawnedPassiveItem.transform.SetParent(transform); //Sets weapon as a child of the player
      inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>());

      passiveItemIndex++;
   }
}
