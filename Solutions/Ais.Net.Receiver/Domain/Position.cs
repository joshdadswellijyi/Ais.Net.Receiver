﻿// <copyright file="Position.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Ais.Net.Receiver.Domain
{
    public record Position(double Latitude, double Longitude)
    {
        public static Position From10000thMins(int latitude, int longitude) =>
            new(latitude.From10000thMinsToDegrees(), longitude.From10000thMinsToDegrees());
    }
}