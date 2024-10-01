using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

/**
 * Description: Retrieves and alters data in the database
 * Name: Dominick Hagedorn - Luke Kastern
 * Date:9/30/2024
 * Bugs: None Known.
 * Reflection: This was a pretty easy class to implement
 */
namespace Lab3
{
    public class Database : IDatabase
       {
        private string connectionString = GetConnectionString();

        /*
         * initializes the connection to the SQL database
         */
        public Database()
        {
            // Optional: Test connection on initialization
            try
            {
                using var connection = new NpgsqlConnection(connectionString); // format connection
                connection.Open(); // open connection
                Console.WriteLine("Connection successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }
        /*
         * Builds a ConnectionString, which is used to connect to the database
         */ 
        static String GetConnectionString() { 
            var connStringBuilder = new NpgsqlConnectionStringBuilder(); 
            connStringBuilder.Host = "hollow-centaur-1973.jxf.cockroachlabs.cloud"; 
            connStringBuilder.Port = 26257; 
            connStringBuilder.SslMode = SslMode.VerifyFull; 
            connStringBuilder.Username = "lower"; // won't hardcode this in your app 
            IConfiguration config = new ConfigurationBuilder().AddUserSecrets<Database>().Build();
            connStringBuilder.Password = config["CockroachDBPassword"] ?? "";
            connStringBuilder.Database = "defaultdb"; 
            connStringBuilder.ApplicationName = "whatever"; // ignored, apparently connStringBuilder.IncludeErrorDetail = true;
            return connStringBuilder.ConnectionString; 
        }

        /**
         * deletes an airport with a given id
         */
        public AirportDeletionError DeleteAirport(string id)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var sql = "DELETE FROM public.airports WHERE id = @id"; //sql
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("id", id); // insert id to be deleted into command

                int rowsAffected = command.ExecuteNonQuery(); // deletes airport
                return rowsAffected > 0 ? AirportDeletionError.NoError : AirportDeletionError.FailedToDeleteError; // if rowsAffected is -1, then no airports were deleted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting airport: {ex.Message}");
                return AirportDeletionError.FailedToDeleteError;
            }
        }

        /**
         * adds an airport to the collection
         */
        public AirportAddError InsertAirport(Airport airport)
        {
            if (airport == null)
            {
                return AirportAddError.NoAirportPassed;
            }

            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var sql = "INSERT INTO public.airports (id, city, datevisited, rating) VALUES (@id, @city, @dateVisited, @rating)"; // sql command
                using var command = new NpgsqlCommand(sql, connection);
                //parameters
                command.Parameters.AddWithValue("id", airport.Id);
                command.Parameters.AddWithValue("city", airport.City);
                command.Parameters.AddWithValue("dateVisited", airport.DateVisited);
                command.Parameters.AddWithValue("rating", airport.Rating);

                command.ExecuteNonQuery();
                return AirportAddError.NoError;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting airport: {ex.Message}");
                return AirportAddError.DatabaseError;
            }
        }

        /**
         * finds and returns an airport based on its id
         */
        public Airport SelectAirport(string id)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var sql = "SELECT id, city, datevisited, rating FROM public.airports WHERE id = @id"; //sql command
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("id", id); // add id to command

                using var reader = command.ExecuteReader();
                // read in the airport retrieved from the database
                if (reader.Read())  
                {
                    return new Airport(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetDateTime(2),
                        reader.GetInt16(3)
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting airport: {ex.Message}");
            }
            return null;
        }

        public List<Airport> SelectAllAirports()
        {
            var airports = new List<Airport>();

            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var sql = "SELECT id, city, datevisited, rating FROM public.airports"; // SQL command
                using var command = new NpgsqlCommand(sql, connection);

                using var reader = command.ExecuteReader();
                // read in the query of airports from the database
                while (reader.Read())
                {
                    airports.Add(new Airport(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetDateTime(2),
                        reader.GetInt16(3)
                    ));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting all airports: {ex.Message}");
            }

            return airports;
        }

        /**
         * update the properties inside of a given airport
         * 
         */
        public AirportEditError UpdateAirport(Airport airport)
        {
            if (airport == null || SelectAirport(airport.Id) == null)
            {
                return AirportEditError.IdNotPresent;
            }

            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var sql = "UPDATE public.airports SET city = @city, datevisited = @dateVisited, rating = @rating WHERE id = @id"; // SQL Command
                using var command = new NpgsqlCommand(sql, connection);
                // add the parameters to the SQL command
                command.Parameters.AddWithValue("id", airport.Id);
                command.Parameters.AddWithValue("city", airport.City);
                command.Parameters.AddWithValue("dateVisited", airport.DateVisited);
                command.Parameters.AddWithValue("rating", airport.Rating);

                command.ExecuteNonQuery();
                connection.Close();
                return AirportEditError.NoError;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating airport: {ex.Message}");
                return AirportEditError.DatabaseError;
            }
        }
    }
}
