﻿// <copyright file="IVesselPosition.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Ais.Net.Receiver.Domain
{
    public interface IVesselPosition
    {
        Position? Position { get; }
    }
}