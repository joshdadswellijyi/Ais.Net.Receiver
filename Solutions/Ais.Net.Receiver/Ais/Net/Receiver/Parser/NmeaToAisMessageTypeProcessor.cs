﻿// <copyright file="NmeaToAisMessageTypeProcessor.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Ais.Net.Receiver.Parser
{
    using System;
    using System.Reactive.Subjects;

    using Ais.Net.Models;
    using Ais.Net.Models.Abstractions;

    /// <summary>
    /// Receives AIS messages parsed from an NMEA sentence and converts it into an
    /// <see cref="System.IObservable{T}"/> stream of <see cref="IAisMessage"/> based types.
    /// </summary>
    public class NmeaToAisMessageTypeProcessor : INmeaAisMessageStreamProcessor
    {
        private readonly Subject<IAisMessage> messages = new();

        public IObservable<IAisMessage> Messages => this.messages;

        public void OnNext(in NmeaLineParser parsedLine, in ReadOnlySpan<byte> asciiPayload, uint padding)
        {
            int messageType = NmeaPayloadParser.PeekMessageType(asciiPayload, padding);

            try
            {
                switch (messageType)
                {
                    case >= 1 and <= 3:
                        {
                            this.ParseMessageTypes1Through3(parsedLine, asciiPayload, padding, messageType);
                            return;
                        }

                    case 5:
                        {
                            this.ParseMessageType5(parsedLine, asciiPayload, padding);
                            return;
                        }

                    case 18:
                        {
                            this.ParseMessageType18(parsedLine, asciiPayload, padding);
                            return;
                        }

                    case 19:
                        {
                            this.ParseMessageType19(parsedLine, asciiPayload, padding);
                            return;
                        }

                    case 24:
                        {
                            this.ParseMessageType24(parsedLine, asciiPayload, padding);
                            return;
                        }

                    case 27:
                        {
                            this.ParseMessageType27(parsedLine, asciiPayload, padding);
                            return;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{messageType}] {e.Message}");
            }
        }

        public void OnError(in ReadOnlySpan<byte> line, Exception error, int lineNumber)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void Progress(
            bool done,
            int totalNmeaLines,
            int totalAisMessages,
            int totalTicks,
            int nmeaLinesSinceLastUpdate,
            int aisMessagesSinceLastUpdate,
            int ticksSinceLastUpdate)
        {
            throw new NotImplementedException();
        }

        private void ParseMessageTypes1Through3(NmeaLineParser nmeaLineParser, ReadOnlySpan<byte> asciiPayload, uint padding, int messageType)
        {
            var parser = new NmeaAisPositionReportClassAParser(asciiPayload, padding);

            var message = new AisMessageType1Through3(
                CourseOverGroundDegrees: parser.CourseOverGround10thDegrees.FromTenthsToDegrees(),
                ManoeuvreIndicator: parser.ManoeuvreIndicator,
                MessageType: messageType,
                Mmsi: parser.Mmsi,
                NavigationStatus: parser.NavigationStatus,
                Position: Position.From10000thMins(parser.Latitude10000thMins, parser.Longitude10000thMins),
                PositionAccuracy: parser.PositionAccuracy,
                RadioSlotTimeout: parser.RadioSlotTimeout,
                RadioSubMessage: parser.RadioSubMessage,
                RadioSyncState: parser.RadioSyncState,
                RaimFlag: parser.RaimFlag,
                RateOfTurn: parser.RateOfTurn,
                RepeatIndicator: parser.RepeatIndicator,
                SpareBits145: parser.SpareBits145,
                SpeedOverGround: parser.SpeedOverGroundTenths.FromTenths(),
                TimeStampSecond: parser.TimeStampSecond,
                TrueHeadingDegrees: parser.TrueHeadingDegrees,
                UnixTimestamp: nmeaLineParser.TagBlock.UnixTimestamp);

            this.messages.OnNext(message);
        }

        private void ParseMessageType5(NmeaLineParser nmeaLineParser, ReadOnlySpan<byte> asciiPayload, uint padding)
        {
            var parser = new NmeaAisStaticAndVoyageRelatedDataParser(asciiPayload, padding);

            var message = new AisMessageType5(
                AisVersion: parser.AisVersion,
                EtaMonth: parser.EtaMonth,
                EtaDay: parser.EtaDay,
                EtaHour: parser.EtaHour,
                EtaMinute: parser.EtaMinute,
                Mmsi: parser.Mmsi,
                IsDteNotReady: parser.IsDteNotReady,
                ImoNumber: parser.ImoNumber,
                CallSign: parser.CallSign.TextFieldToString(),
                Destination: parser.Destination.TextFieldToString(),
                VesselName: parser.VesselName.TextFieldToString(),
                ShipType: parser.ShipType,
                RepeatIndicator: parser.RepeatIndicator,
                DimensionToBow: parser.DimensionToBow,
                DimensionToPort: parser.DimensionToPort,
                DimensionToStarboard: parser.DimensionToStarboard,
                DimensionToStern: parser.DimensionToStern,
                Draught10thMetres: parser.Draught10thMetres,
                Spare423: parser.Spare423,
                PositionFixType: parser.PositionFixType,
                UnixTimestamp: nmeaLineParser.TagBlock.UnixTimestamp);

            this.messages.OnNext(message);
        }

        private void ParseMessageType18(NmeaLineParser nmeaLineParser, ReadOnlySpan<byte> asciiPayload, uint padding)
        {
            var parser = new NmeaAisPositionReportClassBParser(asciiPayload, padding);

            var message = new AisMessageType18(
                Mmsi: parser.Mmsi,
                Position: Position.From10000thMins(parser.Latitude10000thMins, parser.Longitude10000thMins),
                CanAcceptMessage22ChannelAssignment: parser.CanAcceptMessage22ChannelAssignment,
                CanSwitchBands: parser.CanSwitchBands,
                CsUnit: parser.CsUnit,
                HasDisplay: parser.HasDisplay,
                IsDscAttached: parser.IsDscAttached,
                RadioStatusType: parser.RadioStatusType,
                RegionalReserved139: parser.RegionalReserved139,
                RegionalReserved38: parser.RegionalReserved38,
                CourseOverGroundDegrees: parser.CourseOverGround10thDegrees.FromTenthsToDegrees(),
                PositionAccuracy: parser.PositionAccuracy,
                SpeedOverGround: parser.SpeedOverGroundTenths.FromTenths(),
                TimeStampSecond: parser.TimeStampSecond,
                TrueHeadingDegrees: parser.TrueHeadingDegrees,
                IsAssigned: parser.IsAssigned,
                RaimFlag: parser.RaimFlag,
                RepeatIndicator: parser.RepeatIndicator,
                UnixTimestamp: nmeaLineParser.TagBlock.UnixTimestamp);

            this.messages.OnNext(message);
        }

        private void ParseMessageType19(NmeaLineParser nmeaLineParser, ReadOnlySpan<byte> asciiPayload, uint padding)
        {
            var parser = new NmeaAisPositionReportExtendedClassBParser(asciiPayload, padding);

            Span<byte> shipNameAscii = stackalloc byte[(int)parser.ShipName.CharacterCount];
            parser.ShipName.WriteAsAscii(shipNameAscii);

            var message = new AisMessageType19(
                Mmsi: parser.Mmsi,
                ShipName: shipNameAscii.GetString(),
                CourseOverGroundDegrees: parser.CourseOverGround10thDegrees.FromTenthsToDegrees(),
                DimensionToBow: parser.DimensionToBow,
                DimensionToPort: parser.DimensionToPort,
                DimensionToStarboard: parser.DimensionToStarboard,
                DimensionToStern: parser.DimensionToStern,
                IsAssigned: parser.IsAssigned,
                IsDteNotReady: parser.IsDteNotReady,
                PositionAccuracy: parser.PositionAccuracy,
                PositionFixType: parser.PositionFixType,
                RaimFlag: parser.RaimFlag,
                RegionalReserved139: parser.RegionalReserved139,
                RegionalReserved38: parser.RegionalReserved38,
                RepeatIndicator: parser.RepeatIndicator,
                ShipType: parser.ShipType,
                Spare308: parser.Spare308,
                SpeedOverGround: parser.SpeedOverGroundTenths.FromTenths(),
                TimeStampSecond: parser.TimeStampSecond,
                TrueHeadingDegrees: parser.TrueHeadingDegrees,
                Position: Position.From10000thMins(parser.Latitude10000thMins, parser.Longitude10000thMins),
                UnixTimestamp: nmeaLineParser.TagBlock.UnixTimestamp);

            this.messages.OnNext(message);
        }

        private void ParseMessageType24(NmeaLineParser nmeaLineParser, ReadOnlySpan<byte> asciiPayload, uint padding)
        {
            uint part = NmeaAisStaticDataReportParser.GetPartNumber(asciiPayload, padding);

            switch (part)
            {
                case 0:
                    {
                        var parser = new NmeaAisStaticDataReportParserPartA(asciiPayload, padding);

                        Span<byte> vesselNameAscii = stackalloc byte[(int)parser.VesselName.CharacterCount];
                        parser.VesselName.WriteAsAscii(vesselNameAscii);

                        var message = new AisMessageType24Part0(
                            Mmsi: parser.Mmsi,
                            PartNumber: parser.PartNumber,
                            RepeatIndicator: parser.RepeatIndicator,
                            Spare160: parser.Spare160,
                            UnixTimestamp: nmeaLineParser.TagBlock.UnixTimestamp);

                        this.messages.OnNext(message);
                        break;
                    }

                case 1:
                    {
                        var parser = new NmeaAisStaticDataReportParserPartB(asciiPayload, padding);

                        Span<byte> callSignAscii = stackalloc byte[(int)parser.CallSign.CharacterCount];
                        parser.CallSign.WriteAsAscii(callSignAscii);

                        Span<byte> vendorIdRev3Ascii = stackalloc byte[(int)parser.VendorIdRev3.CharacterCount];
                        parser.VendorIdRev3.WriteAsAscii(vendorIdRev3Ascii);

                        Span<byte> vendorIdRev4Ascii = stackalloc byte[(int)parser.VendorIdRev4.CharacterCount];
                        parser.VendorIdRev3.WriteAsAscii(vendorIdRev4Ascii);

                        var message = new AisMessageType24Part1(
                            Mmsi: parser.Mmsi,
                            CallSign: callSignAscii.GetString(),
                            DimensionToBow: parser.DimensionToBow,
                            DimensionToPort: parser.DimensionToPort,
                            DimensionToStarboard: parser.DimensionToStarboard,
                            DimensionToStern: parser.DimensionToStern,
                            MothershipMmsi: parser.MothershipMmsi,
                            PartNumber: parser.PartNumber,
                            RepeatIndicator: parser.RepeatIndicator,
                            SerialNumber: parser.SerialNumber,
                            ShipType: parser.ShipType,
                            Spare162: parser.Spare162,
                            UnitModelCode: parser.UnitModelCode,
                            VendorIdRev3: vendorIdRev3Ascii.GetString(),
                            VendorIdRev4: vendorIdRev4Ascii.GetString(),
                            UnixTimestamp: nmeaLineParser.TagBlock.UnixTimestamp);

                        this.messages.OnNext(message);
                        break;
                    }
            }
        }

        private void ParseMessageType27(NmeaLineParser nmeaLineParser, ReadOnlySpan<byte> asciiPayload, uint padding)
        {
            var parser = new NmeaAisLongRangeAisBroadcastParser(asciiPayload, padding);

            var message = new AisMessageType27(
                Mmsi: parser.Mmsi,
                Position: Position.From10thMins(parser.Latitude10thMins, parser.Longitude10thMins),
                CourseOverGroundDegrees: parser.CourseOverGroundDegrees,
                PositionAccuracy: parser.PositionAccuracy,
                SpeedOverGround: parser.SpeedOverGroundTenths.FromTenths(),
                RaimFlag: parser.RaimFlag,
                RepeatIndicator: parser.RepeatIndicator,
                NotGnssPosition: parser.NotGnssPosition,
                NavigationStatus: parser.NavigationStatus,
                UnixTimestamp: nmeaLineParser.TagBlock.UnixTimestamp);

            this.messages.OnNext(message);
        }
    }
}