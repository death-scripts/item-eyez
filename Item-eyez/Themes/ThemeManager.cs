// ----------------------------------------------------------------------------
// <copyright company="death-scripts">
// Copyright (c) death-scripts. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Item_eyez.Themes
{
    internal static class ThemeManager
    {
        internal const string Theme1Source = "Themes/Theme1.xaml";
        internal const string Theme2Source = "Themes/Theme2.xaml";
        internal const string Theme3Source = "Themes/Theme3.xaml";

        private const string ConfigFileName = "theme-config.json";
        private const string AppFolderName = "Item-eyez";

        private sealed class ThemeConfigModel
        {
            public string ThemeSource { get; set; } = Theme1Source;
        }

        internal static void ApplyThemeFromConfig()
        {
            var themeSource = LoadThemeSource() ?? Theme1Source;
            ApplyTheme(themeSource);
        }

        internal static void ApplyTheme(string source)
        {
            if (Application.Current == null)
            {
                return;
            }

            var dictionaries = Application.Current.Resources.MergedDictionaries;
            if (dictionaries == null)
            {
                return;
            }

            for (var index = dictionaries.Count - 1; index >= 0; index--)
            {
                var uri = dictionaries[index].Source;
                if (uri != null && uri.OriginalString.StartsWith("Themes/", StringComparison.OrdinalIgnoreCase))
                {
                    dictionaries.RemoveAt(index);
                }
            }

            try
            {
                dictionaries.Add(new ResourceDictionary { Source = new Uri(source, UriKind.Relative) });
            }
            catch
            {
                // If the requested theme cannot be loaded, fall back to Theme1.
                dictionaries.Add(new ResourceDictionary { Source = new Uri(Theme1Source, UriKind.Relative) });
            }
        }

        internal static void SaveThemeSource(string source)
        {
            try
            {
                var path = GetConfigPath();
                var configDirectory = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(configDirectory))
                {
                    Directory.CreateDirectory(configDirectory);
                }

                var model = new ThemeConfigModel { ThemeSource = source };
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(model, options);
                File.WriteAllText(path, json);
            }
            catch
            {
                // Ignore persistence failures; do not crash the app.
            }
        }

        private static string? LoadThemeSource()
        {
            try
            {
                var path = GetConfigPath();
                if (!File.Exists(path))
                {
                    return null;
                }

                var json = File.ReadAllText(path);
                var model = JsonSerializer.Deserialize<ThemeConfigModel>(json);
                if (model == null || string.IsNullOrWhiteSpace(model.ThemeSource))
                {
                    return null;
                }

                return model.ThemeSource;
            }
            catch
            {
                return null;
            }
        }

        private static string GetConfigPath()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataFolder, AppFolderName);
            return Path.Combine(appFolder, ConfigFileName);
        }
    }
}
