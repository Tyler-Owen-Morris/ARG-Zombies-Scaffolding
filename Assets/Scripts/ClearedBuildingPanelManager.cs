using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using LitJson;

public class ClearedBuildingPanelManager : MonoBehaviour {

    public Text trapStatusText, barrelStatusText, greenhouseStatusText;
    public Slider trapProgressSlider, greenhouseProgressSlider, waterBarrelProgressSlider;
    private MapLevelManager currentMapManager;
    public GameObject lootConfirmationPanel;

    private string lootByTypeURL = GameManager.serverURL + "/LootByType.php";
    private string placeGearURL = GameManager.serverURL + "/PlaceGear.php";
    private string loot_type;
    private int loot_qty;
    
    void Start ()
    {
        currentMapManager = MapLevelManager.FindObjectOfType<MapLevelManager>();
    }	

    public void InitilizeMyText ()
    {
        currentMapManager = MapLevelManager.FindObjectOfType<MapLevelManager>();
        if (currentMapManager.activeBuilding.has_traps == true)
        {
            trapStatusText.text = "trap is planted";
            trapProgressSlider.gameObject.SetActive(true);
            CalculateTrapStatus();
        }else
        {
            trapStatusText.text = "";
            trapProgressSlider.gameObject.SetActive(false);
        }
        if (currentMapManager.activeBuilding.has_barrel == true)
        {
            barrelStatusText.text = "barrel is in place";
            waterBarrelProgressSlider.gameObject.SetActive(true);
            CalculateBarrelStatus();
        }else
        {
            barrelStatusText.text = "";
            waterBarrelProgressSlider.gameObject.SetActive(false);
        }
        if(currentMapManager.activeBuilding.has_greenhouse == true)
        {
            greenhouseStatusText.text = "greenhouse is growing";
            greenhouseProgressSlider.gameObject.SetActive(true);
            CalculateGreenhouseStatus();
        }else
        {
            greenhouseStatusText.text = "";
            greenhouseProgressSlider.gameObject.SetActive(false);
        }
    }

    public void LootRequest(string to_loot) {
        if (to_loot == "supply")
        {
            //confirm there is supply to loot.
            if (currentMapManager.activeBuilding.supply_inside > 0)
            {
                loot_type = to_loot;
                loot_qty = currentMapManager.activeBuilding.supply_inside;
                lootConfirmationPanel.SetActive(true);
            }
        }else if (to_loot == "food")
        {
            //confirm there is food
            if (currentMapManager.activeBuilding.food_inside > 0)
            {
                loot_type = to_loot;
                loot_qty = currentMapManager.activeBuilding.food_inside;
                lootConfirmationPanel.SetActive(true);
            }
        }else if (to_loot == "water")
        {
            if (currentMapManager.activeBuilding.water_inside > 0)
            {
                loot_type = to_loot;
                loot_qty = currentMapManager.activeBuilding.water_inside;
                lootConfirmationPanel.SetActive(true);
            }
        }else
        {
            lootConfirmationPanel.SetActive(false);
            Debug.Log("Your button is rigged wrong breh...");
        }
    }

    public void CancelLootRequest ()
    {
        loot_type = "";
        lootConfirmationPanel.SetActive(false);
    }

    private bool lootRequestInProgress = false;
    public void ConfirmLootRequest ()
    {
        if (lootRequestInProgress == false)
        {
            lootRequestInProgress = true;
            StartCoroutine(LootBldgByType(loot_type));
        }
    }

    IEnumerator LootBldgByType(string type)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");

        form.AddField("type", type);
        form.AddField("qty", loot_qty);
        form.AddField("bldg_name", currentMapManager.activeBuilding.buildingName);
        form.AddField("bldg_id", currentMapManager.activeBuilding.buildingID);

        WWW www = new WWW(lootByTypeURL, form);
        yield return www;
        Debug.Log(www.text);

        if (www.error == null)
        {
            JsonData lootJSON = JsonMapper.ToObject(www.text);
            if (lootJSON[0].ToString() == "Success")
            {
                if (type == "supply")
                {
                    GameManager.instance.supply += loot_qty;
                    trapProgressSlider.value = 0;
                    currentMapManager.activeBuilding.supply_inside = 0;
                    currentMapManager.activeBuilding.last_looted_supply = DateTime.Now;
                    currentMapManager.clearedBldgSupplyText.text = "0";
                    yield return new WaitForSeconds(0.1f);
                    currentMapManager.UpdateTheUI();
                    CalculateTrapStatus();
                }else if (type == "food")
                {
                    GameManager.instance.foodCount += loot_qty;
                    greenhouseProgressSlider.value = 0;
                    currentMapManager.activeBuilding.food_inside = 0;
                    currentMapManager.activeBuilding.last_looted_food = DateTime.Now;
                    currentMapManager.clearedBldgFoodText.text = "0";
                    yield return new WaitForSeconds(0.1f);
                    CalculateGreenhouseStatus();
                    currentMapManager.UpdateTheUI();
                }else if (type == "water")
                {
                    GameManager.instance.waterCount += loot_qty;
                    waterBarrelProgressSlider.value = 0;
                    currentMapManager.activeBuilding.water_inside = 0;
                    currentMapManager.activeBuilding.last_looted_water = DateTime.Now;
                    currentMapManager.clearedBldgWaterText.text = "0";
                    yield return new WaitForSeconds(0.1f);
                    CalculateBarrelStatus();
                    currentMapManager.UpdateTheUI();
                }
            }else
            {
                Debug.Log(lootJSON[1].ToString());
            }
        }else
        {
            Debug.Log(www.error);
        }
        lootRequestInProgress = false;
        lootConfirmationPanel.SetActive(false);
    }

    public void CalculateTrapStatus ()
    {
        //calculate days trap has been in place.
        int days_to_fill = 5;
        float interval = 1.0f/days_to_fill;
        int days_active = Mathf.FloorToInt((float)(DateTime.Now - currentMapManager.activeBuilding.last_looted_supply).TotalDays);
        Debug.Log(days_active.ToString()+": days active <<<<<<<<<<<<<<<<<<<<<<");
        float slider_value = interval * (float)days_active;
        Debug.Log("Trap slider value calculated to be: " + slider_value.ToString());

        //set the slider between 0.0 and 1.0f
        if (slider_value > 1.0f)
        {
            trapProgressSlider.value = 1.0f;
        } else if (slider_value > 0)
        {
            trapProgressSlider.value = slider_value;
        } else
        {
            trapProgressSlider.value = 0;
            Debug.Log("Slider value calculated as a negative");
        }

        //store the "active supply" amounts in game data based on full days.
        if (days_active < 1)
        {
            currentMapManager.activeBuilding.supply_inside = 0;
            currentMapManager.clearedBldgSupplyText.text = "0";
        }else if (days_active < 2)
        {
            currentMapManager.activeBuilding.supply_inside = 5;
            currentMapManager.clearedBldgSupplyText.text = "5";
        }
        else if (days_active < 3)
        {
            currentMapManager.activeBuilding.supply_inside = 10;
            currentMapManager.clearedBldgSupplyText.text = "10";
        }
        else if (days_active < 4)
        {
            currentMapManager.activeBuilding.supply_inside = 20;
            currentMapManager.clearedBldgSupplyText.text = "20";
        }
        else if (days_active< 5)
        {
            currentMapManager.activeBuilding.supply_inside = 35;
            currentMapManager.clearedBldgSupplyText.text = "35";
        }
        else
        {
            currentMapManager.activeBuilding.supply_inside = 45;
            currentMapManager.clearedBldgSupplyText.text = "45";
        }
    }

    public void CalculateBarrelStatus()
    {
        //calculate days barrel has been in place.
        int days_to_fill = 10;
        float interval = 1.0f / days_to_fill;
        int days_active = Mathf.FloorToInt((float)(DateTime.Now - currentMapManager.activeBuilding.last_looted_water).TotalDays);
        float slider_value = interval * days_active;
        Debug.Log("Barrel slider value calculated to be: " + slider_value.ToString());

        //set the slider between 0.0 and 1.0f
        if (slider_value > 1.0f)
        {
            
            waterBarrelProgressSlider.value = 1.0f;
        }
        else if (slider_value > 0)
        {
            waterBarrelProgressSlider.value = slider_value;
        }
        else
        {
            waterBarrelProgressSlider.value = 0;
            Debug.Log("Slider value calculated as a negative");
        }

        //store the "active water" amounts in game data based on full days.
        if (days_active < 1)
        {
            currentMapManager.activeBuilding.water_inside = 0;
            currentMapManager.clearedBldgWaterText.text = "0";
        }
        else if (days_active < 2)
        {
            currentMapManager.activeBuilding.water_inside = 2;
            currentMapManager.clearedBldgWaterText.text = "2";
        }
        else if (days_active < 3)
        {
            currentMapManager.activeBuilding.water_inside = 4;
            currentMapManager.clearedBldgWaterText.text = "4";
        }
        else if (days_active < 4)
        {
            currentMapManager.activeBuilding.water_inside = 6;
            currentMapManager.clearedBldgWaterText.text = "6";
        }
        else if (days_active < 5)
        {
            currentMapManager.activeBuilding.water_inside = 8;
            currentMapManager.clearedBldgWaterText.text = "8";
        }
        else if (days_active < 6)
        {
            currentMapManager.activeBuilding.water_inside = 10;
            currentMapManager.clearedBldgWaterText.text = "10";
        }
        else if (days_active < 7)
        {
            currentMapManager.activeBuilding.water_inside = 12;
            currentMapManager.clearedBldgWaterText.text = "12";
        }
        else if (days_active < 8)
        {
            currentMapManager.activeBuilding.water_inside = 14;
            currentMapManager.clearedBldgWaterText.text = "14";
        }
        else if (days_active < 9)
        {
            currentMapManager.activeBuilding.water_inside = 16;
            currentMapManager.clearedBldgWaterText.text = "16";
        }
        else if (days_active < 10)
        {
            currentMapManager.activeBuilding.water_inside = 18;
            currentMapManager.clearedBldgWaterText.text = "18";
        }
        else
        {
            currentMapManager.activeBuilding.water_inside = 20;
            currentMapManager.clearedBldgWaterText.text = "20";
        }
    }

    public void CalculateGreenhouseStatus ()
    {
        //calculate days barrel has been in place.
        int days_to_fill = 20;
        float interval = 1.0f / days_to_fill;
        int days_active = Mathf.FloorToInt((float)(DateTime.Now - currentMapManager.activeBuilding.last_looted_food).TotalDays);
        float slider_value = interval * days_active;
        Debug.Log("Greenhouse slider value calculated to be: " + slider_value.ToString());

        //set the slider between 0.0 and 1.0f
        if (slider_value > 1.0f)
        {

            greenhouseProgressSlider.value = 1.0f;
        }
        else if (slider_value > 0.00f)
        {
            greenhouseProgressSlider.value = slider_value;
        }
        else
        {
            greenhouseProgressSlider.value = 0;
            Debug.Log("Slider value calculated as a negative");
        }

        //store the "active water" amounts in game data based on full days.
        if (days_active < 10)
        {
            currentMapManager.activeBuilding.food_inside = 0;
            currentMapManager.clearedBldgFoodText.text = "0";
        }
        else if (days_active < 11)
        {
            currentMapManager.activeBuilding.food_inside = 1;
            currentMapManager.clearedBldgFoodText.text = "1";
        }
        else if (days_active < 12)
        {
            currentMapManager.activeBuilding.food_inside = 1;
            currentMapManager.clearedBldgFoodText.text = "1";
        }
        else if (days_active < 13)
        {
            currentMapManager.activeBuilding.food_inside = 2;
            currentMapManager.clearedBldgFoodText.text = "2";
        }
        else if (days_active < 14)
        {
            currentMapManager.activeBuilding.food_inside = 4;
            currentMapManager.clearedBldgSupplyText.text = "4";
        }
        else if (days_active < 15)
        {
            currentMapManager.activeBuilding.food_inside = 8;
            currentMapManager.clearedBldgFoodText.text = "8";
        }
        else if (days_active < 16)
        {
            currentMapManager.activeBuilding.food_inside = 8;
            currentMapManager.clearedBldgFoodText.text = "8";
        }
        else if (days_active < 17)
        {
            currentMapManager.activeBuilding.food_inside = 14;
            currentMapManager.clearedBldgFoodText.text = "14";
        }
        else if (days_active < 20)
        {
            currentMapManager.activeBuilding.food_inside = 20;
            currentMapManager.clearedBldgFoodText.text = "20";
        }
        else if (days_active < 30)
        {   
            //greenhouse has reached full harvest.
            currentMapManager.activeBuilding.food_inside = 35;
            currentMapManager.clearedBldgFoodText.text = "35";
        }
        else
        {
            //if more than 30 days have passed, food is perished
            currentMapManager.activeBuilding.food_inside = 0;
            currentMapManager.clearedBldgFoodText.text = "0";
            greenhouseProgressSlider.value = 0.0f;
        }
    }

    public void PlantZombieTrap ()
    {
        currentMapManager = MapLevelManager.FindObjectOfType<MapLevelManager>();
        if (currentMapManager.activeBuilding.has_traps == false) {
            if (GameManager.instance.trap > 0)
            {
                //start the coroutine to send this item to the server
                Debug.Log("this will be sending a trap to the server");
                StartCoroutine(PlaceGearAtBuilding(currentMapManager.activeBuilding.buildingName, currentMapManager.activeBuilding.buildingID
                    ,"trap"));
            } else
            {
                StartCoroutine(PostTempTrapText("you have no traps to place"));
            }
        }else
        {
            trapStatusText.text = "trap is planted";
        }
    }

    public void PlaceWaterBarrel()
    {
        currentMapManager = MapLevelManager.FindObjectOfType<MapLevelManager>();
        if (currentMapManager.activeBuilding.has_barrel == false)
        {
            if (GameManager.instance.barrel > 0)
            {
                //start the coroutine to send the barrel to the server
                Debug.Log("this is sending a barrel to the server");
                StartCoroutine(PlaceGearAtBuilding(currentMapManager.activeBuilding.buildingName, currentMapManager.activeBuilding.buildingID, "barrel"));
            }else
            {
                StartCoroutine(PostTempBarrelText("you dont have the barrel"));
            }
        }else
        {
            barrelStatusText.text = "barrel is in place";
        }
    }

    public void PlaceGreenhouse()
    {
        currentMapManager = MapLevelManager.FindObjectOfType<MapLevelManager>();
        if (currentMapManager.activeBuilding.has_greenhouse == false)
        {
            if (GameManager.instance.greenhouse > 0)
            {
                //start the coroutine to send the greenhouse to the server
                Debug.Log("This will send a greenhouse to the building");
                StartCoroutine(PlaceGearAtBuilding(currentMapManager.activeBuilding.buildingName, currentMapManager.activeBuilding.buildingID, "greenhouse"));
            }
            else
            {
                StartCoroutine(PostTempGreenhouseText("You have no greenhouse"));
            }
        }
        else
        {
            barrelStatusText.text = "greenhouse is growing";
        }
    }

    private bool gearPlacementInProgress = false;
    IEnumerator PlaceGearAtBuilding(string bldg_name, string bldg_id, string type)
    {
        if (gearPlacementInProgress == false)
        {
            gearPlacementInProgress = true;
            WWWForm form = new WWWForm();
            form.AddField("id", GameManager.instance.userId);
            form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
            form.AddField("client", "mob");

            form.AddField("bldg_name", bldg_name);
            form.AddField("bldg_id", bldg_id);
            form.AddField("type", type);

            
            Debug.Log(placeGearURL);
            WWW www = new WWW(placeGearURL, form);
            yield return www;
            Debug.Log(www.text);

            if (www.error == null)
            {
                JsonData gearPlaceJSON = JsonMapper.ToObject(www.text);
                if (gearPlaceJSON[0].ToString() == "Success")
                {
                    currentMapManager = FindObjectOfType<MapLevelManager>();//reassure you have current maplvlmgr
                    Debug.Log(gearPlaceJSON[1].ToString());
                    //update the UI manually to avoid costly server pulls
                    if (type == "trap")
                    {
                        GameManager.instance.trap--;
                        currentMapManager.activeBuilding.has_traps = true;
                        currentMapManager.activeBuilding.trap_indicator_image.SetActive(true);
                        currentMapManager.activeBuilding.last_looted_supply = DateTime.Now;
                        InitilizeMyText();
                    }
                    else if (type == "barrel")
                    {
                        GameManager.instance.barrel--;
                        currentMapManager.activeBuilding.has_barrel = true;
                        currentMapManager.activeBuilding.barrel_indicator_image.SetActive(true);
                        currentMapManager.activeBuilding.last_looted_water = DateTime.Now;
                        InitilizeMyText();
                    }
                    else if (type == "greenhouse")
                    {
                        GameManager.instance.greenhouse--;
                        currentMapManager.activeBuilding.has_greenhouse = true;
                        currentMapManager.activeBuilding.greenhouse_indicator_image.SetActive(true);
                        currentMapManager.activeBuilding.last_looted_food = DateTime.Now;
                        InitilizeMyText();
                    }
                }
                else
                {
                    Debug.Log(gearPlaceJSON[3].ToString());
                }
            }
            else
            {
                Debug.Log(www.error);
            }
            gearPlacementInProgress = false;
        }
    }

    private bool trapTextActive = false;
    IEnumerator PostTempTrapText (string myText)
    {
        if (trapTextActive == false)
        {
            trapTextActive = true;
            trapStatusText.text = myText;
            yield return new WaitForSeconds(1.9f);
            trapStatusText.text = "";
            trapTextActive = false;
        }
    }

    private bool barrelTextActive = false;
    IEnumerator PostTempBarrelText (string myText)
    {
        if (barrelTextActive == false)
        {
            barrelTextActive = true;
            barrelStatusText.text = myText;
            yield return new WaitForSeconds(1.9f);
            barrelStatusText.text = "";
            barrelTextActive = false;
        }
    }

    private bool greenhouseTextActive = false;
    IEnumerator PostTempGreenhouseText (string myText)
    {
        if (greenhouseTextActive == false)
        {
            greenhouseTextActive = true;
            greenhouseStatusText.text = myText;
            yield return new WaitForSeconds(1.9f);
            greenhouseStatusText.text = "";
            greenhouseTextActive = false;
        }
    }

}
