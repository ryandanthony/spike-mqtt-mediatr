namespace Bct.Common.Workflow.Aggregates.Implementation
{
using System;
   /// <summary>
   /// StatusEnum enum
   /// </summary>
   [Serializable]       
   public enum StatusEnum : uint
   {
      /// <summary>Unknown</summary>
      Unknown = 0, 
      /// <summary>Connected</summary>
      Connected = 1, 
      /// <summary>In Use</summary>
      InUse = 2, 
      /// <summary>Disconnected</summary>
      Disconnected = 3
   };

}

