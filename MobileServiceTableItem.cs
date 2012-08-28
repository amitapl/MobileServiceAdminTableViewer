using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Data.Json;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MobileServices
{
    public class MobileServiceTableItem
    {
        public MobileServiceTableItem(JsonObject jsonObject)
        {
            this.JsonObject = jsonObject;
        }

        public JsonObject JsonObject { get; private set; }

        public IEnumerable<Item> Values
        {
            get
            {
                if (this.JsonObject != null)
                {
                    return this.JsonObject.Select(kv => new Item(kv.Key, JsonObject));
                }
                return null;
            }
        }
    }

    public class Item : INotifyPropertyChanged
    {
        public Item(string key, JsonObject jsonObject)
        {
            Key = key;
            JsonObject = jsonObject;
            ValidationError = new SolidColorBrush();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public JsonObject JsonObject { get; private set; }
        public string Key { get; private set; }

        private Brush validationError;
        public Brush ValidationError
        {
            get
            {
                return validationError;
            }
            set
            {
                validationError = value;
                OnPropertyChanged("ValidationError");
            }
        }

        public string Value
        {
            get
            {
                if (JsonObject[Key].ValueType == JsonValueType.String)
                {
                    return JsonObject[Key].GetString();
                }
                return JsonObject[Key].Stringify();
            }
            set
            {
                try
                {
                    switch (JsonObject[Key].ValueType)
                    {
                        case JsonValueType.Boolean:
                            JsonObject[Key] = JsonValue.CreateBooleanValue(bool.Parse(value));
                            break;

                        case JsonValueType.Number:
                            JsonObject[Key] = JsonValue.CreateNumberValue(double.Parse(value));
                            break;

                        case JsonValueType.String:
                            JsonObject[Key] = JsonValue.CreateStringValue(value);
                            break;

                        case JsonValueType.Null:
                        case JsonValueType.Object:
                        case JsonValueType.Array:
                        default:
                            throw new NotSupportedException("The value type is not supported: " + JsonObject[Key].ValueType);
                    }

                    ValidationError = new SolidColorBrush();
                }
                catch (FormatException)
                {
                    ValidationError = new SolidColorBrush(Colors.Red);
                }

                OnPropertyChanged("Value");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
