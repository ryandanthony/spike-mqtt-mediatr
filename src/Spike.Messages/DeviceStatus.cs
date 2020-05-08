namespace Bct.Common.Workflow.Aggregates.Implementation
{
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

    /// <summary>
    /// DeviceStatus class
    /// </summary>
    [Serializable]
    public class DeviceStatus : BaseAggregate
    {
        static DeviceStatus()
        {
            initialized = false;
            tm = new AggregateMetaData();
            s_MetaData();
        }
        /// Private members
        private static bool initialized;
        private static AggregateMetaData tm;

      /// Public properties
      /// <summary>
      /// 
      /// </summary>
      [JsonConverter(typeof(FieldConverter<BaseField<string>, string>))]
      public BaseField<string>                     DeviceId { get; set; }

      /// <summary>
      /// 
      /// </summary>
      [JsonConverter(typeof(FieldConverter<BaseField<string>, string>))]
      public BaseField<string>                     MessageId { get; set; }

      /// <summary>
      /// 
      /// </summary>
      [JsonConverter(typeof(FieldConverter<BaseField<double>, double>))]
      public BaseField<double>                     When { get; set; }

      /// <summary>
      /// 
      /// </summary>
      [JsonConverter(typeof(FieldConverter<EnumField<StatusEnum>, StatusEnum>))]
      public EnumField<StatusEnum>                     Condition { get; set; }


      /// <summary>
      /// Constructor
      /// </summary>
      public DeviceStatus()
         : base(BaseAggregate.UseMostRecentVersionStr) => Initialize();

      /// <summary>
      /// Constructor
      /// </summary>
      public DeviceStatus(string version)
         : base( version) =>Initialize();

      /// <summary>
      /// Constructor
      /// </summary>
      public DeviceStatus(int fieldId, BaseAggregate parent)
         : base(fieldId, parent) => Initialize();

      /// <summary>
      /// Copy Constructor
      /// </summary>
      /// <param name="other"></param>
      /// <returns>DeviceStatus</returns>
      public DeviceStatus(DeviceStatus other)
        : base(other)
      {
         DeviceId = new BaseField<string>(other.DeviceId, this);
         MessageId = new BaseField<string>(other.MessageId, this);
         When = new BaseField<double>(other.When, this);
         Condition = new EnumField<StatusEnum>(other.Condition, this);

         PushFields();
      }

      /// <summary>
      /// Copy Constructor
      /// </summary>
      /// <param name="other"></param>
      /// <param name="parent"></param>
      /// <returns>DeviceStatus</returns>
      public DeviceStatus(DeviceStatus other, BaseAggregate parent)
         : base(other, parent)
      {
         DeviceId = new BaseField<string>(other.DeviceId, this);
         MessageId = new BaseField<string>(other.MessageId, this);
         When = new BaseField<double>(other.When, this);
         Condition = new EnumField<StatusEnum>(other.Condition, this);

         PushFields();
      }

      /// <summary>
      /// Overload Comparison operator (==)
      /// </summary>
      /// <param name="a1"></param>
      /// <param name="a2"></param>
      /// <returns>true/false</returns>
      public static bool operator ==(DeviceStatus a1, DeviceStatus a2)
      {
         if (ReferenceEquals(a1, a2))
         {
            return true;
         }
         if (a1 is null)
         {
            return false;
         }
         if (a2 is null)
         {
            return false;
         }

         if (a1.DeviceId != a2.DeviceId)
         {
            return false;
         }
         if (a1.MessageId != a2.MessageId)
         {
            return false;
         }
         if (a1.When != a2.When)
         {
            return false;
         }
         if (a1.Condition != a2.Condition)
         {
            return false;
         }

         return true;
      }

      /// <summary>
      /// Overload Comparison operator (!=)
      /// </summary>
      /// <param name="a1"></param>
      /// <param name="a2"></param>
      /// <returns>true/false</returns>
      public static bool operator !=(DeviceStatus a1, DeviceStatus a2) => !( a1 == a2 );

      /// <summary>
      /// Equals
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public bool Equals(DeviceStatus other)
      {
         if (other is null)
         {
            return false;
         }

         if (DeviceId != other.DeviceId)
         {
            return false;
         }
         if (MessageId != other.MessageId)
         {
            return false;
         }
         if (When != other.When)
         {
            return false;
         }
         if (Condition != other.Condition)
         {
            return false;
         }
         return true;
      }

      /// <summary>
      /// Equals
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
      public override bool Equals(object obj)
      {
         if (( null == obj ) || !( obj is DeviceStatus ))
         {
            return false;
         }

         return this.Equals((DeviceStatus)obj);      
      }

      public override int GetHashCode()
      {
         var hashCode = Tuple.Create(this).GetHashCode();
         return hashCode;
      }

      /// <summary>
      /// MetaData()
      /// </summary>
      public override AggregateMetaData MetaData() => s_MetaData();

      /// <summary>
      /// Initialize()
      /// </summary>
      void Initialize()
      {
         DeviceId = new BaseField<string> (0, this);
         MessageId = new BaseField<string> (1, this);
         When = new BaseField<double> (2, this);
         Condition = new EnumField<StatusEnum> (3, "StatusEnum",
            "",
            "",
            this);

         PushFields();
         SyncVersion();
      }

      /// <summary>
      /// Add fields into fieldList and aggList
      /// </summary>
      void PushFields()
      {
         FieldList.Add(DeviceId);         
         FieldList.Add(MessageId);         
         FieldList.Add(When);         
         FieldList.Add(Condition);         
      }

      /// <summary>
      /// s_MetaData: static metadata
      /// </summary>
      /// <returns>AggregateMetaData</returns>
      private static AggregateMetaData s_MetaData()
      {
         if (initialized)
         {
            return tm;
         }

         tm.AddVersion("v1.0.0");

         tm.AddField(0, "DeviceId", TypeEnum.StringType);
         tm.AddField(1, "MessageId", TypeEnum.StringType);
         tm.AddField(2, "When", TypeEnum.DoubleType);
         tm.AddField(3, "Condition", TypeEnum.Int32Type);

         tm.AddFieldMetaToAllVersions(0, FieldStateEnum.NotSet, "notset");
         tm.AddFieldMetaToAllVersions(1, FieldStateEnum.NotSet, "notset");
         tm.AddFieldMetaToAllVersions(2, FieldStateEnum.NotSet, "notset");
         tm.AddFieldMetaToAllVersions(3, FieldStateEnum.Default, " '2' ");




         initialized = true;

         return tm;
      }

      /// <summary>
      /// Serializes this aggregate to JSON string.
      /// </summary>
      /// <returns></returns>
      public string Serialize()
      {
         return BaseAggregate.Serialize(this);
      }
      /// <summary>
      /// Deserializes the given JSON string into an instance of this aggregate.
      /// </summary>
      /// <param name="json"></param>
      /// <returns></returns>
      static public DeviceStatus Deserialize(string json)
      {
         return BaseAggregate.Deserialize<DeviceStatus>(json);
      }
   }

}
