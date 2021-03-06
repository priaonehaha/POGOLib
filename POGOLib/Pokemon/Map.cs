﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using Google.Protobuf.Collections;
using POGOLib.Net;
using POGOProtos.Map;
using POGOProtos.Map.Fort;

namespace POGOLib.Pokemon
{
    /// <summary>
    /// A wrapper class for <see cref="RepeatedField{MapCell}"/> with helper methods.
    /// </summary>
    public class Map
    {
        // The current authenticated session.
        private readonly Session _session;

        // The last received map cells.
        private RepeatedField<MapCell> _cells;

        internal Map(Session session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets the last received <see cref="RepeatedField{MapCell}"/> from PokémonGo.<br/>
        /// Only use this if you know what you are doing.
        /// </summary>
        public RepeatedField<MapCell> Cells
        {
            get { return _cells; }
            internal set
            {
                _cells = value;
                OnUpdate();
            }
        }
        
        public List<FortData> GetFortsSortedByDistance(Func<FortData, bool> filter = null)
        {
            var forts = Cells.SelectMany(f => f.Forts);

            if (filter != null)
                forts = forts.Where(filter);

            var sorted = forts.ToList();
            sorted.Sort((f1, f2) =>
            {
                var f1Coordinate = new GeoCoordinate(f1.Latitude, f1.Longitude);
                var f2Coordinate = new GeoCoordinate(f2.Latitude, f2.Longitude);

                var distance1 = f1Coordinate.GetDistanceTo(_session.Player.Coordinate);
                var distance2 = f2Coordinate.GetDistanceTo(_session.Player.Coordinate);

                return distance1.CompareTo(distance2);
            });

            return sorted;
        }

        internal void OnUpdate()
        {
            Update?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> Update;
    }
}
