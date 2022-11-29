using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CCMLibrary
{
    internal class Package
    {
#pragma warning disable CS8618 // Non-nullable property 'MessageType' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public Type MessageType { set; get; }
#pragma warning restore CS8618 // Non-nullable property 'MessageType' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'ClassType' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public Type ClassType { set; get; }
#pragma warning restore CS8618 // Non-nullable property 'ClassType' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
  
#pragma warning disable CS8618 // Non-nullable property 'MessageBytes' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public dynamic MessageBytes { set; get; }
#pragma warning restore CS8618 // Non-nullable property 'MessageBytes' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'ClassBytes' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public dynamic ClassBytes { set; get; }
#pragma warning restore CS8618 // Non-nullable property 'ClassBytes' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
    }
}
