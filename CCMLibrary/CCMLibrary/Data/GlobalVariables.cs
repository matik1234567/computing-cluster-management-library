using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class allow to define global variables,
    /// Implements necessery variables if user didn't define it
    /// </summary>
    public class GlobalVariables
    {
        private static Dictionary<string, dynamic> RequiredAttrDefaults = new Dictionary<string, dynamic> { { "heartBeatFrequency", 10 } };

        [JsonProperty]
        private Dictionary<string, dynamic>? _values;

        public GlobalVariables()
        {
            _values = new Dictionary<string, dynamic>();
            Validate();
        }

        public GlobalVariables(Dictionary<string, dynamic> values)
        {
            _values = values;
            Validate();
        }

        public GlobalVariables(string jsonValues)
        {
            if(jsonValues == null)
            {
                throw new ArgumentNullException();
            }
            _values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonValues);
            Validate();
        }

        public dynamic? GetValue(string key)
        {
            if (_values?.ContainsKey(key)==true)
            {
                return _values[key];
            }
            else
            {
                return null;
            }
        }

        private void Validate()
        {
            foreach(var required in RequiredAttrDefaults)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (!_values.ContainsKey(required.Key))
                {
                    _values[required.Key] = required.Value;
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }

        public int GetHeartBeatFreq()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return Convert.ToInt32(_values["heartBeatFrequency"]);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public Dictionary<string, dynamic>? GetValues()
        {
            return _values;
        }
    }
}
