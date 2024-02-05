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
        public float Length { get; set; }
        public int ElevationGain { get; set; }
        public int HighestPoint { get; set; }
        public int[] Coords { get; set; }
        public string Difficulty { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }

        // Constructor
        public Hike(string name, float length, int elevationGain, int highestPoint, int[] coords, string difficulty, string description, string link) 
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
