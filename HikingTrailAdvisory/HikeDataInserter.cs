using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace HikingTrailAdvisory
{
    public class HikeDataInserter
    {
        private readonly string _connectionString;

        public HikeDataInserter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertTrailData(Dictionary<string, Hike> trailData)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (var trail in trailData.Values)
                {
                    string insertTrailQuery = "INSERT INTO Trails (Name, Length, ElevationGain, HighestPoint, Coords, Difficulty, Description, Link) " +
                                                          "VALUES (@Name, @Length, @ElevationGain, @HighestPoint, @Coords, @Difficulty, @Description, @Link)";

                    using (SqlCommand command = new SqlCommand(insertTrailQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", trail.Name);
                        command.Parameters.AddWithValue("@Length", trail.Length);
                        command.Parameters.AddWithValue("@ElevationGain", trail.ElevationGain);
                        command.Parameters.AddWithValue("@HighestPoint", trail.HighestPoint);
                        command.Parameters.AddWithValue("@Coords", trail.Coords);
                        command.Parameters.AddWithValue("@Difficulty", trail.Difficulty);
                        command.Parameters.AddWithValue("@Description", trail.Description);
                        command.Parameters.AddWithValue("@Link", trail.Link);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
