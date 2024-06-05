using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DataAccessLibrary;

public class SortTimeDatabase
{
    private static IConfiguration? _config;
    private SqlConnection? connection;
    private string[] algorithms = { "Insertion Sort", "Merge Sort", "Bubble Sort" };

    public SortTimeDatabase(IConfiguration config) {
        _config = config;
        if(_config == null) connection = null;
        else {
            connection = new SqlConnection(_config.GetConnectionString("Default"));
            connection.Open();
        }
    }

    private bool ContainsAlgorithm(string algorithm) {
        for(int i = 0; i < algorithms.Length; i++) if(algorithms[i].Equals(algorithm)) return true;
        return false;
    }

    public void InsertTime(string algorithm, long nanoSeconds) {
        if(connection == null || !ContainsAlgorithm(algorithm)) return;
        var cmd = new SqlCommand("INSERT INTO times (algorithm, time_nanoseconds, id) VALUES (@a, @n, @i);", connection);
        List<SortTime> list = (List<SortTime>)connection.Query<SortTime>("SELECT MAX(id) AS id FROM times;");
        SortTime x = list.Last();
        int id = x == null ? 1 : x.id + 1;
        cmd.Parameters.AddWithValue("a", algorithm);
        cmd.Parameters.AddWithValue("n", nanoSeconds);
        cmd.Parameters.AddWithValue("i", id);
        cmd.ExecuteNonQuery();
    }

    public List<SortTime>? GetTimes() {
        if(connection == null) return null;
        List<SortTime> list = (List<SortTime>)connection.Query<SortTime>("SELECT algorithm, time_nanoseconds FROM times;");
        HashSet<string> algs = new HashSet<string>();
        List<SortTime> ans = new List<SortTime>();
        foreach(SortTime x in list) {
            if(x.algorithm != null) {
                if(algs.Contains(x.algorithm)) {
                    foreach(SortTime y in ans) {
                        if(y.algorithm != null && y.algorithm.Equals(x.algorithm)) {
                            y.time_nanoseconds = (y.time_nanoseconds + x.time_nanoseconds) / 2;
                            break;
                        }
                    }
                } else {
                    algs.Add(x.algorithm);
                    ans.Add(x);
                }
            }
        }
        return ans;
    }
}