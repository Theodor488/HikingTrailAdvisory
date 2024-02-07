using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikingTrailAdvisory
{
    public class Hike
    {
        // Properties
        public string Name { get; set; }
        public string Length { get; set; }
        public string ElevationGain { get; set; }
        public string HighestPoint { get; set; }
        public string Coords { get; set; }
        public string Difficulty { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }

        // Constructor
        public Hike() 
        {
            
        }

        public Hike(string name, string length, string elevationGain, string highestPoint, string coords, string difficulty, string description, string link) 
        {
            Name = name;
            Length = length;
            ElevationGain = elevationGain;
            HighestPoint = highestPoint;
            Coords = coords;
            Difficulty = difficulty;
            Description = description;
            Link = link;
        }
    }
}
