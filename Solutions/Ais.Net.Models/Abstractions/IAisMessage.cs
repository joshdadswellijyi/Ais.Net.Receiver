﻿// <copyright file="IAisMessage.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Ais.Net.Models.Abstractions
{
    public interface IAisMessage : IVesselIdentity, IAisMessageType
    {
    }
}