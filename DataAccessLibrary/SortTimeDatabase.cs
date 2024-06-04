using Dapper;
using Npgsql;

namespace DataAccessLibrary;

public class SortTimeDatabase
{
    private NpgsqlConnection connection;
    private string[] algorithms = { "Insertion Sort", "Merge Sort", "Bubble Sort" };

    public SortTimeDatabase() {
        connection = new NpgsqlConnection("AZURE_POSTGRESQL_CONNECTIONSTRING");
        connection.Open();
    }

    private bool ContainsAlgorithm(string algorithm) {
        for(int i = 0; i < algorithms.Length; i++) if(algorithms[i].Equals(algorithm)) return true;
        return false;
    }

    public void InsertTime(string algorithm, long nanoSeconds) {
        if(!ContainsAlgorithm(algorithm)) return;
        var cmd = new NpgsqlCommand("INSERT INTO times (algorithm, time_nanoseconds, id) VALUES (@a, @n, @i);", connection);
        List<SortTime> list = (List<SortTime>)connection.Query<SortTime>("SELECT MAX(id) AS id FROM times;");
        SortTime x = list.Last();
        int id = x == null ? 1 : x.id + 1;
        cmd.Parameters.AddWithValue("a", algorithm);
        cmd.Parameters.AddWithValue("n", nanoSeconds);
        cmd.Parameters.AddWithValue("i", id);
        cmd.ExecuteNonQuery();
    }

    public List<SortTime> GetTimes() {
        List<SortTime> list = (List<SortTime>)connection.Query<SortTime>("SELECT DISTINCT algorithm, AVG(time_nanoseconds) AS avgTimeNanoseconds FROM times GROUP BY algorithm ORDER BY avgTimeNanoSeconds ASC;");
        return list;
    }
}