namespace Bct.Common.Workflow.Aggregates.Implementation
{
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Bct.Common.Workflow.Aggregates.Implementation;

   /// <summary>
   /// DeviceStatus class
   /// </summary>
   [Serializable]
   public class DeviceStatus : BaseAggregate
   {
      /// Private members
      private static bool initialized = false;
      private static AggregateMetaData tm = new AggregateMetaData();

      /// Public properties
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
         Condition = new EnumField<StatusEnum> (0, "StatusEnum",
            "",
            "",
            this);

         PushFields();
         SyncVersion();
      }

      /// <summary>
      /// MetaData()
      /// </summary>
      void PushFields()
      {
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

         tm.AddField(0, "Condition", TypeEnum.Int32Type);

         tm.AddFieldMetaToAllVersions(0, FieldStateEnum.Default, " '2' ");




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
