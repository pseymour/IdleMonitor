namespace KioskRebootService
{
    using System;
    using Microsoft.Win32;    
    
    /// <summary>
    /// This class manages application settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The default number of minutes after which the computer will be rebooted.
        /// </summary>
        private const int DefaultNoUserLogonThreshold = 2;

        /// <summary>
        /// The top-level registry key in which the settings will be stored.
        /// </summary>
        private static RegistryKey rootRegistryKey = Registry.LocalMachine;

        /// <summary>
        /// Gets or sets a value indicating the number of minutes that a computer
        /// is allowed to sit with no one logged on before it is rebooted.
        /// </summary>
        public static int NoUserLogonThreshold
        {
            get
            {
                int? regValue = GetDWord("No User Logon Threshold");
                if (!regValue.HasValue)
                {
                    regValue = DefaultNoUserLogonThreshold;
                }
                return regValue.Value;
            }
            set
            {
                SetDWord("No User Logon Threshold", System.Math.Max(value, DefaultNoUserLogonThreshold));
            }
        }

        /// <summary>
        /// Gets the path of the registry key in which all of the settings are stored.
        /// </summary>
        private static string RegistryKeyPath
        {
            get { return string.Format(System.Globalization.CultureInfo.InvariantCulture, @"Software\{0}\{1}", CompanyName, ProductName); }
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
        /// Gets a double-word (DWORD) value from the registry.
        /// </summary>
        /// <param name="valueName">
        /// The name of the DWORD value to be retrieved.
        /// </param>
        /// <returns>
        /// Returns the value of the DWORD registry entry with the specified name,
        /// if that value can be read. Otherwise, returns null.
        /// </returns>
        private static int? GetDWord(string valueName)
        {
            int? returnValue = null;

            RegistryKey settingsKey = rootRegistryKey.OpenSubKey(RegistryKeyPath, false);
            if (settingsKey != null)
            {
                object regValue = settingsKey.GetValue(valueName);
                if (regValue != null)
                {
                    returnValue = (int)regValue;
                }

                settingsKey.Close();
            }

            return returnValue;
        }

        /// <summary>
        /// Sets a double-word (DWORD) value in the registry.
        /// </summary>
        /// <param name="valueName">
        /// The name of the DWORD value to be set.
        /// </param>
        /// <param name="value">
        /// The value to be stored in the registry.
        /// </param>
        /// <remarks>
        /// If the value parameter is null and the specified registry
        /// entry exists, the registry entry is deleted.
        /// </remarks>
        private static void SetDWord(string valueName, int? value)
        {
            RegistryKey settingsKey = rootRegistryKey.CreateSubKey(RegistryKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
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
