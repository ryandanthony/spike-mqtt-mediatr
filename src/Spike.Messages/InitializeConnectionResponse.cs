namespace Bct.Common.Workflow.Aggregates.Implementation
{
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

   /// <summary>
   /// InitializeConnectionResponse class
   /// </summary>
   [Serializable]
   public class InitializeConnectionResponse : BaseAggregate
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
      /// 
      /// </summary>
      [JsonConverter(typeof(FieldConverter<BaseField<bool>, bool>))]
      public BaseField<bool>                     RequestApproved { get; set; }


      /// <summary>
      /// Constructor
      /// </summary>
      public InitializeConnectionResponse()
         : base(BaseAggregate.UseMostRecentVersionStr) => Initialize();

      /// <summary>
      /// Constructor
      /// </summary>
      public InitializeConnectionResponse(string version)
         : base( version) =>Initialize();

      /// <summary>
      /// Constructor
      /// </summary>
      public InitializeConnectionResponse(int fieldId, BaseAggregate parent)
         : base(fieldId, parent) => Initialize();

      /// <summary>
      /// Copy Constructor
      /// </summary>
      /// <param name="other"></param>
      /// <returns>InitializeConnectionResponse</returns>
      public InitializeConnectionResponse(InitializeConnectionResponse other)
        : base(other)
      {
         DeviceId = new BaseField<string>(other.DeviceId, this);
         RequestApproved = new BaseField<bool>(other.RequestApproved, this);

         PushFields();
      }

      /// <summary>
      /// Copy Constructor
      /// </summary>
      /// <param name="other"></param>
      /// <param name="parent"></param>
      /// <returns>InitializeConnectionResponse</returns>
      public InitializeConnectionResponse(InitializeConnectionResponse other, BaseAggregate parent)
         : base(other, parent)
      {
         DeviceId = new BaseField<string>(other.DeviceId, this);
         RequestApproved = new BaseField<bool>(other.RequestApproved, this);

         PushFields();
      }

      /// <summary>
      /// Overload Comparison operator (==)
      /// </summary>
      /// <param name="a1"></param>
      /// <param name="a2"></param>
      /// <returns>true/false</returns>
      public static bool operator ==(InitializeConnectionResponse a1, InitializeConnectionResponse a2)
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
         if (a1.RequestApproved != a2.RequestApproved)
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
      public static bool operator !=(InitializeConnectionResponse a1, InitializeConnectionResponse a2) => !( a1 == a2 );

      /// <summary>
      /// Equals
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public bool Equals(InitializeConnectionResponse other)
      {
         if (other is null)
         {
            return false;
         }

         if (DeviceId != other.DeviceId)
         {
            return false;
         }
         if (RequestApproved != other.RequestApproved)
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
         if (( null == obj ) || !( obj is InitializeConnectionResponse ))
         {
            return false;
         }

         return this.Equals((InitializeConnectionResponse)obj);      
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
         RequestApproved = new BaseField<bool> (1, this);

         PushFields();
         SyncVersion();
      }

      /// <summary>
      /// MetaData()
      /// </summary>
      void PushFields()
      {
         FieldList.Add(DeviceId);         
         FieldList.Add(RequestApproved);         
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
         tm.AddField(1, "RequestApproved", TypeEnum.BoolType);

         tm.AddFieldMetaToAllVersions(0, FieldStateEnum.NotSet, "notset");
         tm.AddFieldMetaToAllVersions(1, FieldStateEnum.NotSet, "notset");




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
      static public InitializeConnectionResponse Deserialize(string json)
      {
         return BaseAggregate.Deserialize<InitializeConnectionResponse>(json);
      }
   }

}
