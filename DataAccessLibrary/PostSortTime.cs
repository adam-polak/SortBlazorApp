using System.Data;
using Dapper;
using Npgsql;

namespace DataAccessLibrary;

public class PostSortTime
{
    private NpgsqlConnection connection;
    private string[] algorithms = { "Insertion Sort", "Merge Sort", "Bubble Sort" };

    public PostSortTime() {
        connection = new NpgsqlConnection("Host=localhost:5432;Username=postgres;Password=password;Database=sortingdb");
        connection.Open();
    }

    private bool ContainsAlgorithm(string algorithm) {
        for(int i = 0; i < algorithms.Length; i++) if(algorithms[i].Equals(algorithm)) return true;
        return false;
    }

    public void InsertTime(string algorithm, long nanoSeconds) {
        if(!ContainsAlgorithm(algorithm)) return;
        var cmd = new NpgsqlCommand("INSERT INTO times (algorithm, nanoseconds) VALUES (" + algorithm + ", " + nanoSeconds + ");", connection);
        cmd.ExecuteNonQuery();
    }
}