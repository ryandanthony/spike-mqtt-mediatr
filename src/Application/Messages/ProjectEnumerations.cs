namespace Bct.Common.Workflow.Aggregates.Implementation
{
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

   /// <summary>
   /// ProjectEnumerations class
   /// </summary>
   [Serializable]
   public class ProjectEnumerations : BaseAggregate
   {
      /// Private members
      private static bool initialized = false;
      private static AggregateMetaData tm = new AggregateMetaData();

      /// Public properties
      /// <summary>
      /// 
      /// </summary>
      [JsonConverter(typeof(FieldConverter<EnumField<StatusEnum>, StatusEnum>))]
      public EnumField<StatusEnum>                     StatusGenerator { get; set; }


      /// <summary>
      /// Constructor
      /// </summary>
      public ProjectEnumerations()
         : base(BaseAggregate.UseMostRecentVersionStr) => Initialize();

      /// <summary>
      /// Constructor
      /// </summary>
      public ProjectEnumerations(string version)
         : base( version) =>Initialize();

      /// <summary>
      /// Constructor
      /// </summary>
      public ProjectEnumerations(int fieldId, BaseAggregate parent)
         : base(fieldId, parent) => Initialize();

      /// <summary>
      /// Copy Constructor
      /// </summary>
      /// <param name="other"></param>
      /// <returns>ProjectEnumerations</returns>
      public ProjectEnumerations(ProjectEnumerations other)
        : base(other)
      {
         StatusGenerator = new EnumField<StatusEnum>(other.StatusGenerator, this);

         PushFields();
      }

      /// <summary>
      /// Copy Constructor
      /// </summary>
      /// <param name="other"></param>
      /// <param name="parent"></param>
      /// <returns>ProjectEnumerations</returns>
      public ProjectEnumerations(ProjectEnumerations other, BaseAggregate parent)
         : base(other, parent)
      {
         StatusGenerator = new EnumField<StatusEnum>(other.StatusGenerator, this);

         PushFields();
      }

      /// <summary>
      /// Overload Comparison operator (==)
      /// </summary>
      /// <param name="a1"></param>
      /// <param name="a2"></param>
      /// <returns>true/false</returns>
      public static bool operator ==(ProjectEnumerations a1, ProjectEnumerations a2)
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

         if (a1.StatusGenerator != a2.StatusGenerator)
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
      public static bool operator !=(ProjectEnumerations a1, ProjectEnumerations a2) => !( a1 == a2 );

      /// <summary>
      /// Equals
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public bool Equals(ProjectEnumerations other)
      {
         if (other is null)
         {
            return false;
         }

         if (StatusGenerator != other.StatusGenerator)
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
         if (( null == obj ) || !( obj is ProjectEnumerations ))
         {
            return false;
         }

         return this.Equals((ProjectEnumerations)obj);      
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
         StatusGenerator = new EnumField<StatusEnum> (0, "StatusEnum",
            "0 1 2 3 ",
            "StatusEnum.Unknown StatusEnum.Connected StatusEnum.InUse StatusEnum.Disconnected ",
            this);

         PushFields();
         SyncVersion();
      }

      /// <summary>
      /// MetaData()
      /// </summary>
      void PushFields()
      {
         FieldList.Add(StatusGenerator);         
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

         tm.AddField(0, "StatusGenerator", TypeEnum.Int32Type);

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
      static public ProjectEnumerations Deserialize(string json)
      {
         return BaseAggregate.Deserialize<ProjectEnumerations>(json);
      }
   }

}
