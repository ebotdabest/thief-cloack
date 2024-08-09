using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;
using System;
using System.IO;
using UnitySA;
using static PlayerInventory;
#if UnityEditor
using UnityEditor;
#endif

namespace TheifCloack
{
    //Unique named bullshit
    [BepInPlugin("hu.ebot.tc", "Thief Cloack", "1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool in_dis;  
        private Scene prev_scene;
        private float timeSinceLastUpdate = 0.0f;
        private float updateInterval = 0.5f;
        private float charge = 100;
        private void MakePlayerInvisible() {
            PlayerController.instance.isGhostCheat = true;
            PlayerController.instance.isCloaked = true;
            PlayerController.instance.TurnOnTheCloakingDevice();
            PlayerController.instance.fpsObject.tag = "Untagged";
        }

        private void MakePlayerVisible() {
            PlayerController.instance.isGhostCheat = false;
            PlayerController.instance.isCloaked = false;
            PlayerController.instance.TurnOffTheCloakingDevice();
            PlayerController.instance.fpsObject.tag = "Player";
        }
        private void Remove()
        {
            if (timeSinceLastUpdate >= updateInterval) {
                charge -= 1;
                timeSinceLastUpdate = 0.0f;
            }
        }
        private void Add()
        {
            if (timeSinceLastUpdate >= updateInterval) {
                charge += 1;
                timeSinceLastUpdate = 0.0f;
            }
        }

        private bool labelVisible = false;
        private void OnGUI()
        {
            if (Shared.owns_cloacking_tool)
            {
                if (!Shared.nonos.Contains(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name))
                {
                    GUI.Label(new Rect(10, Screen.height - 50,300, 60), "Charge: " + charge);
                    GUI.Label(new Rect(10, Screen.height - 30, 300, 60), "Activated: " + in_dis);
                }
            }
            if (labelVisible)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 40;
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height + 10), popup_text, style);
                Invoke("HideLabel", 0.5f);
            }
        }

        private string popup_text {get; set;}
        private void PopUp(string text)
        {
            labelVisible = false;
            popup_text = text;
            labelVisible = true;
        }

        private void HideLabel()
        {
            labelVisible = false;
        }


        private void Update() {
            // if (Input.GetKeyDown(KeyCode.RightAlt))
            // {
            //     Renderer renderer = PlayerController.instance.GetComponentInChildren<Renderer>();
            //     Material mat = renderer.material;
            //     try
            //     {
            //         #if UnityEditor
            //         string path = AssetDatabase.GetAssetPath(mat);
            //         Debug.Log("Material path: " + path);
            //         #endif
            //     }catch (System.Exception ex)
            //     {
            //         Debug.Log(ex);
            //     }
            // }
            if (!Shared.nonos.Contains(SceneManager.GetActiveScene().name))
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    if (Shared.owns_cloacking_tool && !Shared.nonos.Contains(Shared.GetScene().name))
                    {
                        if (PlayerInventory.cash >= 10 && charge < Shared.max_charge)
                        {
                            PlayerInventory.instance.SpendCash(10);
                            UnityEngine.Object.Instantiate<GameObject>(GameController.instance.cashSound);
                            charge = Shared.max_charge;
                        }
                        
                    }
                }
                prev_scene = PlayerController.instance.gameObject.scene;
                if (PlayerController.instance.gameObject.scene != prev_scene) {
                    charge = 100;
                    in_dis = false;
                    MakePlayerVisible();
                }
                timeSinceLastUpdate += Time.deltaTime;
                if (in_dis)
                {
                    if (charge >= 1)
                    {
                        Remove();
                    }else{
                        MakePlayerVisible();
                        in_dis = false;
                        PopUp("Ran out of energy");
                    }
                }else{
                    if (charge <= 0)
                    {
                        Add();
                    }else if (charge == Shared.max_charge)
                    {
                        //Pass
                    }else{
                        Add();
                    }
                }            

                if (Input.GetKeyDown(KeyCode.X))
                {
                    if (Shared.owns_cloacking_tool)
                    {
                        if (charge >= 1)
                        {
                            try
                            {
                                switch (in_dis)
                                {
                                    case true:
                                        MakePlayerVisible();
                                        UnityEngine.Object.Instantiate<GameObject>(PlayerController.instance.cloakSound);
                                        Logger.LogInfo("Turned off!");   
                                        in_dis = false;             
                                        break;
                                    case false:
                                        MakePlayerInvisible();
                                        PlayerController controller = PlayerController.instance;
                                        // Renderer renderer = controller.GetComponent<Renderer>();
                                        // Material newMaterial = Instantiate(renderer.material); // Create a copy of the existing material
                                        // renderer.material = newMaterial; 
                                        Logger.LogInfo("Turned on!");
                                        in_dis = true;
                                        break;
                                }
                            }catch (System.Exception ex)
                            {
                                Logger.LogInfo("Cannot do boss!");
                                Logger.LogInfo(ex);
                            }
                        }
                    }               
                    
                }

                if (Input.GetKeyDown(KeyCode.M))
                {
                    Logger.LogInfo(Shared.owns_cloacking_tool);
                }
                //Logger.LogInfo($"Scene : {scene.name}");
            }

            if (Input.GetKeyUp(KeyCode.P))
            {
                DataApi data = new DataApi();
            }
            try
            {
                if (old_profile != SaveController.currentProfileId)
                {
                    //Console.WriteLine(SaveController.currentProfileId);
                    int id = SaveController.currentProfileId;
                    string fn = "data_cl.sv";
                    DataApi api = new DataApi();
                    Dictionary<int, Dictionary<string, string>> smth = api.Read(fn);
                    Dictionary<string, string> ld = smth[id];
                    Console.WriteLine($"Data of profile {ld["has_watch"]}");
                    Console.WriteLine($"and {ld["max_charge"]}");
                    if (ld["has_watch"].Contains("true"))
                    {
                        Shared.owns_cloacking_tool = true;
                    }else {
                        Shared.owns_cloacking_tool = false;
                    }
                    Shared.max_charge = float.Parse(ld["max_charge"]);
                    charge = Shared.max_charge;
                    old_profile = SaveController.currentProfileId;
                }
            }catch (KeyNotFoundException)
            {
                //Pass
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                int counter = 0;
                foreach (ItemAsset asset in GameController.instance.toolsForShopAssets)
                {
                    Logger.LogInfo($"{counter}:{asset.itemName}, {asset.itemDesc}, {asset.itemType}, {asset.toolId}");
                    counter += 1;
                }
            }
        }

        private int old_profile;
        private void Awake()
        {

            Harmony harmony = new Harmony("hu.ebot.tc");
            harmony.PatchAll();
            string fn = "data_cl.sv";
            if (!File.Exists("data_cl.sv"))
            {
                Console.WriteLine("Cannot find default data_cl.sv file, creating one!");
                DataApi data = new DataApi();
                File.Create("data_cl.sv").Close();
                Console.WriteLine("Generating!");
                data.GenDef(fn);
                Console.WriteLine("Done!");
            }
            //this.gameObject.AddComponent<ShopMenu>();
            in_dis = false;
            Logger.LogInfo("Plugin Theif Cloack is loaded!");
        }
    }
}
