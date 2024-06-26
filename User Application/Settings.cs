﻿namespace UserIdleMonitor
{
    using Microsoft.Win32;    
    
    /// <summary>
    /// This class manages user-specific application settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The top-level registry key in which the settings will be stored.
        /// </summary>
        private static RegistryKey rootRegistryKey = Registry.CurrentUser;

        /// <summary>
        /// Gets or sets a value specifying a number of minutes, after which an idle user session will be logged off.
        /// </summary>
        public static int IdleTimeoutMinutes
        {
            get
            {
                int? policyTimeoutSetting = GetDWord(PolicyRegistryKeyPath, null, "Idle Timeout");
                int? preferenceTimeoutSetting = GetDWord(PreferenceRegistryKeyPath, null, "Idle Timeout");
                if (policyTimeoutSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return policyTimeoutSetting.Value;
                }
                else if (preferenceTimeoutSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceTimeoutSetting.Value;
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value.
#if DEBUG
                    return 5;
#else
                    return 60;
#endif
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Idle Timeout", value);
            }
        }


        /// <summary>
        /// Gets or sets a value specifying a number of minutes, after which an idle user is warned that they will be logged off.
        /// </summary>
        public static int WarningMinutes
        {
            get
            {
                int? policyTimeoutSetting = GetDWord(PolicyRegistryKeyPath, null, "Initial Warning Minutes");
                int? preferenceTimeoutSetting = GetDWord(PreferenceRegistryKeyPath, null, "Initial Warning Minutes");
                if (policyTimeoutSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return policyTimeoutSetting.Value;
                }
                else if (preferenceTimeoutSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceTimeoutSetting.Value;
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value.
#if DEBUG
                    return 3;
#else
                    return 15;
#endif
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Initial Warning Minutes", value);
            }
        }

        /// <summary>
        /// Gets or sets a value specifying a number of minutes, after which a log off warning message is repeatedly displayed to the user.
        /// </summary>
        public static int WarningRepeatMinutes
        {
            get
            {
                int? policyTimeoutSetting = GetDWord(PolicyRegistryKeyPath, null, "Warning Repeat Minutes");
                int? preferenceTimeoutSetting = GetDWord(PreferenceRegistryKeyPath, null, "Warning Repeat Minutes");
                if (policyTimeoutSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return policyTimeoutSetting.Value;
                }
                else if (preferenceTimeoutSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceTimeoutSetting.Value;
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value.
#if DEBUG
                    return 1;
#else
                    return 5;
#endif
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Warning Repeat Minutes", value);
            }
        }

        /// <summary>
        /// Gets the path of the registry key in which all of the settings are stored.
        /// </summary>
        private static string PreferenceRegistryKeyPath
        {
            get { return string.Format(System.Globalization.CultureInfo.InvariantCulture, @"Software\{0}", ProductName); }
        }

        private static string PolicyRegistryKeyPath
        {
            get { return string.Format(System.Globalization.CultureInfo.InvariantCulture, @"Software\Policies\{0}", ProductName); }
        }


        /// <summary>
        /// Gets the company name from this class's assembly.
        /// </summary>
        /// <remarks>
        /// If a company name is not specified in the assembly, "My Company" is returned.
        /// </remarks>
        private static string CompanyName
        {
            get
            {
                string returnValue = "My Company";
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                object[] attributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCompanyAttribute), false);
                foreach (object o in attributes)
                {
                    if (o != null)
                    {
                        returnValue = ((System.Reflection.AssemblyCompanyAttribute)o).Company;
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the product name from this class's assembly.
        /// </summary>
        /// <remarks>
        /// If a product name is not specified in the assembly, "My Product" is returned.
        /// </remarks>
        private static string ProductName
        {
            get
            {
                string returnValue = "My Product";
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                object[] attributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), false);
                foreach (object o in attributes)
                {
                    if (o != null)
                    {
                        returnValue = ((System.Reflection.AssemblyProductAttribute)o).Product;
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Retrieves an integer value from the registry.
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key in which the value is stored.
        /// </param>
        /// <param name="subkeyName">
        /// The optional path of a subkey within the specified key.
        /// </param>
        /// <param name="valueName">
        /// The name of the registry value in which the integer is be stored.
        /// </param>
        /// <returns>
        /// </returns>
        private static int? GetDWord(string keyPath, string subkeyName, string valueName)
        {
            int? returnValue = null;

            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }

            RegistryKey settingsKey = rootRegistryKey.OpenSubKey(keyPath, false);
            if (settingsKey != null)
            {
                object regValue = settingsKey.GetValue(valueName);
                if (regValue != null)
                {
                    returnValue = System.Convert.ToInt32(regValue);
                }

                settingsKey.Close();
            }

            return returnValue;
        }

        /// <summary>
        /// Stores an integer in the registry.
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key in which the value is stored.
        /// </param>
        /// <param name="subkeyName">
        /// The optional path of a subkey within the specified key.
        /// </param>
        /// <param name="valueName">
        /// The name of the registry value in which the integer will be stored.
        /// </param>
        /// <param name="value">
        /// The integer to stored in the registry.
        /// </param>
        /// <remarks>
        /// The integer value is stored in the registry as a DWORD.
        /// </remarks>
        private static void SetDWord(string keyPath, string subkeyName, string valueName, int? value)
        {
            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }
            RegistryKey settingsKey = rootRegistryKey.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (settingsKey != null)
            {
                if (value.HasValue)
                {
                    settingsKey.SetValue(valueName, System.Convert.ToInt32(value.Value), RegistryValueKind.DWord);
                }
                else
                {
                    string[] valueNames = settingsKey.GetValueNames();
                    for (int i = 0; i < valueNames.Length; i++)
                    {
                        if (string.Compare(valueNames[i], valueName, true) == 0)
                        {
                            settingsKey.DeleteValue(valueName);
                            break;
                        }
                    }
                }

                settingsKey.Flush();
                settingsKey.Close();
            }
        }
    }
}
