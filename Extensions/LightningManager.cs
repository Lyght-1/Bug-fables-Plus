using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace BFPlus.Extensions
{
    public class LightningManager : MonoBehaviour
    {
        GradientColorKey[] colorKeyAmbient = new GradientColorKey[7]
        {
            new GradientColorKey(new Color(0.24f,0.31f,0.878f),0f),
            new GradientColorKey(new Color(0.24f,0.31f,0.878f),0.132f),
            new GradientColorKey(new Color(0.768f,0.694f,0.5764f),0.25f),
            new GradientColorKey(new Color(0.4862f,0.4627f,0.5647f),0.5f),
            new GradientColorKey(new Color(0.7647f,0.6862f,0.4745f),0.732f),
            new GradientColorKey(new Color(0.196f,0.2745f,0.8549f),0.8f),
            new GradientColorKey(new Color(0.2078f,0.2901f,0.9059f),1f),
        };

        GradientColorKey[] colorKeyDirectional = new GradientColorKey[3]
        {
            new GradientColorKey(Color.black,0f),
            new GradientColorKey(Color.white,0.5f),
            new GradientColorKey(Color.black,1f),
        };

        GradientColorKey[] colorKeyFog = new GradientColorKey[8]
        {
            new GradientColorKey(new Color(0.04f,0.04f,0.04f),0f),
            new GradientColorKey(new Color(0.133f,0.133f,0.133f),0.171f),
            new GradientColorKey(new Color(0.6274f,0.5058f,0.1960f),0.25f),
            new GradientColorKey(Color.white,0.347f),
            new GradientColorKey(Color.white,0.641f),
            new GradientColorKey(new Color(0.8588f,0.8509f,0.3450f),0.724f),
            new GradientColorKey(Color.black,82.1f),
            new GradientColorKey(Color.black,1f)
        };

        GradientColorKey[] colorKeySky = new GradientColorKey[3]
        {
            new GradientColorKey(new Color(0.04f,0.04f,0.04f),0f),
            new GradientColorKey(new Color(0.6980f,0.6862f,0.6862f),0.5f),
            new GradientColorKey(Color.black,1f)
        };

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2]
        {
            new GradientAlphaKey(1f,0f),
            new GradientAlphaKey(1f,1f)
        };

        public Gradient ambientColor = new Gradient();
        public Gradient directionalColor = new Gradient();
        public Gradient fogColor = new Gradient();
        public Gradient skyBoxColor = new Gradient();
        private Light directionalLight;
        public static float timeOfDay;
        public float timeModifier = 0.01f;
        
        void Start()
        {
            ambientColor.SetKeys(colorKeyAmbient, alphaKeys);
            directionalColor.SetKeys(colorKeyDirectional, alphaKeys);
            fogColor.SetKeys(colorKeyFog, alphaKeys);
            skyBoxColor.SetKeys(colorKeySky, alphaKeys);
        }

        void Update()
        {
            if (directionalLight == null)
                FindMapLight();

            if (Application.isPlaying)
            {
                timeOfDay += Time.deltaTime * timeModifier;
                timeOfDay %= 24;
                UpdateLighting(timeOfDay / 24f);
            }
            else
            {
                UpdateLighting(timeOfDay / 24f);
            }
        }


        void UpdateLighting(float timePercent)
        {
            RenderSettings.ambientLight = ambientColor.Evaluate(timePercent);
            RenderSettings.fogColor = fogColor.Evaluate(timePercent);
            RenderSettings.skybox.SetColor("_Tint", skyBoxColor.Evaluate(timePercent));
            
            
            if (directionalLight != null)
            {
                directionalLight.color = directionalColor.Evaluate(timePercent);
                //DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
            }

        }

        void FindMapLight()
        {
            if (directionalLight != null || MainManager.map == null)
                return;

            var mapLight = MainManager.map.transform.GetComponentInChildren<Light>();
            if (mapLight != null && mapLight.type == LightType.Directional)
            {
                directionalLight = mapLight;
                return;
            }
        }
    }
}
