namespace Bct.Common.Workflow.Aggregates.Implementation
{
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

   /// <summary>
   /// InitializeConnection class
   /// </summary>
   [Serializable]
   public class InitializeConnection : BaseAggregate
   {
      /// Private members
      private static bool initialized = false;
      private static AggregateMetaData tm = new AggregateMetaData();

      /// Public properties
      /// <summary>
      /// 
      /// </summary>
      [JsonConverter(typeof(FieldConverter<BaseField<string>, string>))]
      public BaseField<string>                     DeviceId { get; set; }


      /// <summary>
      /// Constructor
      /// </summary>
      public InitializeConnection()
         : base(BaseAggregate.UseMostRecentVersionStr) => Initialize();

      /// <summary>
      /// Constructor
      /// </summary>
      public InitializeConnection(string version)
         : base( version) =>Initialize();

      /// <summary>
      /// Constructor
      /// </summary>
      public InitializeConnection(int fieldId, BaseAggregate parent)
         : base(fieldId, parent) => Initialize();

      /// <summary>
      /// Copy Constructor
      /// </summary>
      /// <param name="other"></param>
      /// <returns>InitializeConnection</returns>
      public InitializeConnection(InitializeConnection other)
        : base(other)
      {
         DeviceId = new BaseField<string>(other.DeviceId, this);

         PushFields();
      }

      /// <summary>
      /// Copy Constructor
      /// </summary>
      /// <param name="other"></param>
      /// <param name="parent"></param>
      /// <returns>InitializeConnection</returns>
      public InitializeConnection(InitializeConnection other, BaseAggregate parent)
         : base(other, parent)
      {
         DeviceId = new BaseField<string>(other.DeviceId, this);

         PushFields();
      }

      /// <summary>
      /// Overload Comparison operator (==)
      /// </summary>
      /// <param name="a1"></param>
      /// <param name="a2"></param>
      /// <returns>true/false</returns>
      public static bool operator ==(InitializeConnection a1, InitializeConnection a2)
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

         return true;
      }

      /// <summary>
      /// Overload Comparison operator (!=)
      /// </summary>
      /// <param name="a1"></param>
      /// <param name="a2"></param>
      /// <returns>true/false</returns>
      public static bool operator !=(InitializeConnection a1, InitializeConnection a2) => !( a1 == a2 );

      /// <summary>
      /// Equals
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public bool Equals(InitializeConnection other)
      {
         if (other is null)
         {
            return false;
         }

         if (DeviceId != other.DeviceId)
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
         if (( null == obj ) || !( obj is InitializeConnection ))
         {
            return false;
         }

         return this.Equals((InitializeConnection)obj);      
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

         PushFields();
         SyncVersion();
      }

      /// <summary>
      /// Add fields into fieldList and aggList
      /// </summary>
      void PushFields()
      {
         FieldList.Add(DeviceId);         
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

         tm.AddFieldMetaToAllVersions(0, FieldStateEnum.NotSet, "notset");




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
      static public InitializeConnection Deserialize(string json)
      {
         return BaseAggregate.Deserialize<InitializeConnection>(json);
      }
   }

}
