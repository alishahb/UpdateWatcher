using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Alisha.UpdateWatcher.Converter;
using Alisha.UpdateWatcher.Interfaces;
using Newtonsoft.Json;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace Alisha.UpdateWatcher.Managers
{

    public class ConfigurationManager<T, U> where T : IInterface where U : T, INotifyPropertyChanged
    {
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Error,
            DefaultValueHandling = DefaultValueHandling.Include,
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new NullToEmptyStringResolver(),
            Error = SerializationErrorHandler
        };

        private static readonly JsonSerializerSettings _deserializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate,
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            Error = SerializationErrorHandler,
            Converters = new List<JsonConverter>() { new JsonDeserializeHelper() },
            
        };

        private static void SerializationErrorHandler(object sender, ErrorEventArgs e)
        {
            MessageBox.Show($"Settings deserialization error <{e.ErrorContext.Member}>: {e.ErrorContext.Error.Message}", "Read Settings Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public readonly string SettingsDirectoryPath;
        public readonly string SettingsFileName;
        public readonly string SettingsFullPath;
        public readonly DirectoryInfo DirectoryInfo;
        private readonly Type _type;


        public T Settings { get; protected set; }

        internal ConfigurationManager(string directory = "Settings", string fileName = "Settings")
        {
            _type = typeof(U);
            Settings = (T)Activator.CreateInstance(_type);
            SettingsDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);
            SettingsFileName = $"{fileName}.json";
            SettingsFullPath = Path.Combine(SettingsDirectoryPath, SettingsFileName);


            if (!Directory.Exists(SettingsDirectoryPath))
            {
                DirectoryInfo = Directory.CreateDirectory(SettingsDirectoryPath);
                if (!DirectoryInfo.Exists) throw new FileNotFoundException("Can't create settings Directory");
            }

            if (!File.Exists(SettingsFullPath))
                Save();
            else
            {
                try
                {
                    var jsonSettings = JsonConvert.DefaultSettings;
                    JsonConvert.DefaultSettings = () => _deserializerSettings;

                    Settings = (T)JsonConvert.DeserializeObject(File.ReadAllText(SettingsFullPath), _type);

                    JsonConvert.DefaultSettings = jsonSettings;
                }
                catch (JsonSerializationException e)
                {
                    throw new Exception("Can't deserialize Settings from file");
                }
            }
            ((U)Settings).PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e) => Save();

        public static ConfigurationManager<T, U> Create(string fileName = "Settings", string directory = "")
        {
            return new ConfigurationManager<T, U>(directory, fileName);
        }


        public void Save()
        {
            var settingsConcreteType = Convert.ChangeType(Settings, _type);

            File.WriteAllText(SettingsFullPath, JsonConvert.SerializeObject(settingsConcreteType, _serializerSettings));

        }
    }
}
/*

<? xml version="1.0" encoding="utf-8" ?>
<configuration>
  <startup>
    <supportedRuntime version = "v4.0" sku=".NETFramework,Version=v4.6.1" />
  </startup>
  <settings>
    <download url = "http://updates.buddyauth.com/GetNewest?filter=HonorBuddyNightly" folder ="E:\BB\ZIP" />
    <extract folder = "E:\BB\!HB3" delete_existing="true" />
    <rename_files>
      <replace from = "Honorbuddy" to="Nessy" extension=".exe"></replace>
      <replace from = "Honorbuddy" to="Nessy" extension=".config"></replace>
    </rename_files>
    <copy_files>
      <copy pattern = "*.dll" path="E:\BB\!DEV\!Refference" recursive="true"></copy>
      <copy pattern = "Honorbuddy.exe" path="E:\BB\!DEV\!Refference" recursive="false"></copy>
    </copy_files>
  </settings>
</configuration>

    */
