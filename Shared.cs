using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheifCloack
{
    public static class Shared
    {

        public static float max_charge = 100;

        public static string has(bool xd)
        {
            string tr = "false";
            switch (xd){
                case true:
                    tr = "true";
                    break;
                case false:
                    tr = "false";
                    break;
            }
            return tr;                
        }
        public static string fn = "data_cl.sv";
        public static bool owns_cloacking_tool = false;
        public static List<string> nonos = new List<string>{
            "mainmenu",
            "play way logo",
            "nm logo"
        };

        public static Scene GetScene()
        {
            return SceneManager.GetActiveScene();
        }

        public static int GetProfileId()
        {
            return SaveController.currentProfileId;
        }
    }
}