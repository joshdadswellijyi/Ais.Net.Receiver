﻿// <copyright file="AisMessageType18.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net.Models
{
    using Ais.Net.Models.Abstractions;

    public record AisMessageType18(
        bool CanAcceptMessage22ChannelAssignment,
        bool CanSwitchBands,
        float? CourseOverGroundDegrees,
        ClassBUnit CsUnit,
        bool HasDisplay,
        bool IsAssigned,
        bool IsDscAttached,
        uint Mmsi,
        Position? Position,
        bool PositionAccuracy,
        ClassBRadioStatusType RadioStatusType,
        bool RaimFlag,
        int RegionalReserved139,
        int RegionalReserved38,
        uint RepeatIndicator,
        float? SpeedOverGround,
        uint TimeStampSecond,
        uint TrueHeadingDegrees,
        long? UnixTimestamp) :
            AisMessageBase(MessageType: 18, Mmsi, UnixTimestamp),
            IAisMessageType18,
            IAisIsAssigned,
            IRaimFlag,
            IRepeatIndicator,
            IVesselNavigation;
}