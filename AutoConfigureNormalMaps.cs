using System.Text.RegularExpressions;
using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System;

namespace NeosMods
{
    class AutoConfigureNormalMaps : NeosMod
    {
        public override string Name => "AutoSetNormalMaps";
        public override string Author => "";
        public override string Version => "1.0.0";
        public override string Link => "https://www.github.com/DoubleStyx/AutoConfigureNormalMaps";

        private static ModConfiguration config;

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<string> KEY_REGEX = new ModConfigurationKey<string>("Regex", "", () => "normals");

        public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder)
        {
            builder
                .Version(new Version(1, 0, 0))
                .AutoSave(false);
        }

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.DoubleStyx.AutoConfigureNormalMaps");
            harmony.PatchAll();

            config = GetConfiguration();
        }

        [HarmonyPatch(typeof(ImageImporter), nameof(ImageImporter.SetupTextureProxyComponents))]
        static class Patch
        {
            static void Postfix(Slot slot, IAssetProvider<ITexture2D> texture, StereoLayout layout = StereoLayout.None, bool addCollider = true)
            {
                StaticTexture2D textureprovider = (StaticTexture2D)texture;
                if (Regex.IsMatch(slot.Name, config.GetValue(KEY_REGEX)))
                    textureprovider.IsNormalMap.Value = true;
            }
        }
    }
}