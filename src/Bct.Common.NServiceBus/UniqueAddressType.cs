// <copyright file="UniqueAddressType.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

namespace Bct.Common.NServiceBus
{
   /// <summary>
   /// Unique Address types.
   /// </summary>
   public enum UniqueAddressType
   {
      /// <summary>
      /// Represents a HostnameBased address.
      /// </summary>
      HostnameBased = 0,

      /// <summary>
      /// Represents a RandomBased address.
      /// </summary>
      RandomBased = 1,

      /// <summary>
      /// Represents a UserSpecified address.
      /// </summary>
      UserSpecified = 2,
   }
}