using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Alisha;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Alisha.UpdateWatcher.Converter
{
    public class InterfaceConverter<TInterface, TConcrete> : CustomCreationConverter<TInterface>
        where TConcrete : TInterface, new()
    {
        public override TInterface Create(Type objectType)
        {
            return new TConcrete();
        }
    }
    public class InterfaceListConverter<TInterface, TConcrete> : JsonConverter where TConcrete : TInterface
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var res = serializer.Deserialize<List<TConcrete>>(reader);
            var result = res.ConvertAll(x => (TInterface)x);

            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
            {
                return new ObservableCollection<TInterface>(result);
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

}