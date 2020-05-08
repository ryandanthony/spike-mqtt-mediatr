namespace Bct.Common.Workflow.Aggregates.Implementation
{
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

   /// <summary>
   /// AppStatus class
   /// </summary>
   [Serializable]
   public class AppStatus : BaseAggregate
   {
      /// Private members
      private static bool initialized = false;
      private static AggregateMetaData tm = new AggregateMetaData();

      /// Public properties
      /// <summary>
      /// 
      /// </summary>
      [JsonConverter(typeof(FieldConverter<BaseField<string>, string>))]
      public BaseField<string>                     Name { get; set; }

      /// <summary>
      /// 
      /// </summary>
      [JsonConverter(typeof(FieldConverter<BaseField<string>, string>))]
      public BaseField<string>                     MessageId { get; set; }

      /// <summary>
      /// 
      /// </summary>
      [JsonConverter(typeof(FieldConverter<BaseField<string>, string>))]
      public BaseField<string>                     When { get; set; }


      /// <summary>
      /// Constructor
      /// </summary>
      public AppStatus()
         : base(BaseAggregate.UseMostRecentVersionStr) => Initialize();

      /// <summary>
      /// Constructor
      /// </summary>
      public AppStatus(string version)
         : base( version) =>Initialize();

      /// <summary>
      /// Constructor
      /// </summary>
      public AppStatus(int fieldId, BaseAggregate parent)
         : base(fieldId, parent) => Initialize();

      /// <summary>
      /// Copy Constructor
      /// </summary>
      /// <param name="other"></param>
      /// <returns>AppStatus</returns>
      public AppStatus(AppStatus other)
        : base(other)
      {
         Name = new BaseField<string>(other.Name, this);
         MessageId = new BaseField<string>(other.MessageId, this);
         When = new BaseField<string>(other.When, this);

         PushFields();
      }

      /// <summary>
      /// Copy Constructor
      /// </summary>
      /// <param name="other"></param>
      /// <param name="parent"></param>
      /// <returns>AppStatus</returns>
      public AppStatus(AppStatus other, BaseAggregate parent)
         : base(other, parent)
      {
         Name = new BaseField<string>(other.Name, this);
         MessageId = new BaseField<string>(other.MessageId, this);
         When = new BaseField<string>(other.When, this);

         PushFields();
      }

      /// <summary>
      /// Overload Comparison operator (==)
      /// </summary>
      /// <param name="a1"></param>
      /// <param name="a2"></param>
      /// <returns>true/false</returns>
      public static bool operator ==(AppStatus a1, AppStatus a2)
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

         if (a1.Name != a2.Name)
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

         return true;
      }

      /// <summary>
      /// Overload Comparison operator (!=)
      /// </summary>
      /// <param name="a1"></param>
      /// <param name="a2"></param>
      /// <returns>true/false</returns>
      public static bool operator !=(AppStatus a1, AppStatus a2) => !( a1 == a2 );

      /// <summary>
      /// Equals
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public bool Equals(AppStatus other)
      {
         if (other is null)
         {
            return false;
         }

         if (Name != other.Name)
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
         return true;
      }

      /// <summary>
      /// Equals
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
      public override bool Equals(object obj)
      {
         if (( null == obj ) || !( obj is AppStatus ))
         {
            return false;
         }

         return this.Equals((AppStatus)obj);      
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
         Name = new BaseField<string> (0, this);
         MessageId = new BaseField<string> (1, this);
         When = new BaseField<string> (2, this);

         PushFields();
         SyncVersion();
      }

      /// <summary>
      /// Add fields into fieldList and aggList
      /// </summary>
      void PushFields()
      {
         FieldList.Add(Name);         
         FieldList.Add(MessageId);         
         FieldList.Add(When);         
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

         tm.AddField(0, "Name", TypeEnum.StringType);
         tm.AddField(1, "MessageId", TypeEnum.StringType);
         tm.AddField(2, "When", TypeEnum.StringType);

         tm.AddFieldMetaToAllVersions(0, FieldStateEnum.NotSet, "notset");
         tm.AddFieldMetaToAllVersions(1, FieldStateEnum.NotSet, "notset");
         tm.AddFieldMetaToAllVersions(2, FieldStateEnum.NotSet, "notset");




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
      static public AppStatus Deserialize(string json)
      {
         return BaseAggregate.Deserialize<AppStatus>(json);
      }
   }

}
