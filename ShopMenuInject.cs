using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine.UI;

namespace TheifCloack
{
    [HarmonyPatch(typeof(ToolsShopCanvasWindow), "OnOpen")]
    public class AddBtn
    {
        static bool Prefix(ToolsShopCanvasWindow __instance)
        {
            try
            {
                Debug.Log("Done!");

                __instance.GetComponent<CustomBuyBtn>().enabled = true;
                __instance.GetComponent<CustomBuyBtn>().enabled = true;
            }catch (System.Exception)
            {
                Debug.Log("Done!");
                __instance.gameObject.AddComponent<CustomBuyBtn>();
                __instance.GetComponent<CustomBuyBtn>().enabled = true;
                __instance.GetComponent<CustomBuyBtn>().enabled = true;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ToolsShopCanvasWindow), "OnClose")]
    public class RmBtn
    {
        static bool Prefix(ToolsShopCanvasWindow __instance)
        {
            try
            {
                Debug.Log("Done bye!");

                __instance.GetComponent<CustomBuyBtn>().enabled = false;
            }catch (System.Exception)
            {
                //Pass
            }
            return true;
        }
    }

    public class CustomBuyBtn : MonoBehaviour
    {
        public bool is_visible = false;

        public void Destroy()
        {
            is_visible = false;
        }

        public void OnGUI()
        {
            float labelWidth = 250f;
            float labelHeight = 40f;
            float labelPosX = (Screen.width - labelWidth) / 2;
            float labelPosY = 110f;
            
            GUIStyle style = new GUIStyle();
            style.fontSize = 36;
            GUIStyleState statexd = new GUIStyleState();
            statexd.textColor = Color.black;
            style.border = new RectOffset(1,1,1,1);
            style.alignment = TextAnchor.MiddleCenter;
            GUIStyleState state_hover = new GUIStyleState();
            state_hover.textColor = Color.white;
            GUIStyleState active_state = new GUIStyleState();
            active_state.textColor = Color.gray;

            style.hover = state_hover;
            style.active = active_state;
            style.normal = statexd;


            
            GUIStyle label_style = new GUIStyle(GUI.skin.button);
            GUIStyleState col = new GUIStyleState();
            col.textColor = Color.green;
            label_style.fontSize = 23;
            label_style.normal = col;
            label_style.alignment = TextAnchor.MiddleCenter;

            string text;
            if (!Shared.owns_cloacking_tool)
            {
                if (GUI.Button(new Rect(labelPosX, labelPosY, labelWidth, labelHeight), "Buy watch!", style))
                {
                    if (PlayerInventory.cash >= 200000)
                    {
                        Object.Instantiate<GameObject>(GameController.instance.cashSound);
                        PlayerInventory.instance.SpendCash(200000);
                        Shared.owns_cloacking_tool = true;
                        new DataApi().Write("data_cl.sv", Shared.GetProfileId(), "true", Shared.max_charge.ToString());
                    }
                }
                
                text = "It's priced at 200,000$";
            }else
            {
                text="Press be to do nothing more!";
            }
            GUI.Label(new Rect(labelPosX, labelPosY + 30, labelWidth, labelHeight), text, label_style);

        }
    }
}