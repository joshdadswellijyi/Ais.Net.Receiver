﻿// <copyright file="IAisMultipartMessage.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Ais.Net.Models.Abstractions
{
    public interface IAisMultipartMessage
    {
        uint PartNumber { get; }
    }
}