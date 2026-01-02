using System.Text;
using Microsoft.Data.Sqlite;
using PIDTelegramBot.models;

namespace PIDTelegramBot.data;

public sealed class DatabaseService
{
    private static readonly Lazy<DatabaseService> databaseService = new Lazy<DatabaseService>(() => new DatabaseService());
    private Dictionary<char, List<char>> _letterSuggestions = new Dictionary<char, List<char>>();

    public static DatabaseService Instance
    {
        get
        {
            return databaseService.Value;
        }
    }

    private DatabaseService()
    {
        _letterSuggestions.Add('a', new List<char>(){'á'});
        _letterSuggestions.Add('e', new List<char>(){'é','ě'});
        _letterSuggestions.Add('u', new List<char>(){'ú','ů'});
        _letterSuggestions.Add('i', new List<char>(){'í'});
        _letterSuggestions.Add('y', new List<char>(){'ý'});
        _letterSuggestions.Add('c', new List<char>(){'č'});
        _letterSuggestions.Add('s', new List<char>(){'š'});
        _letterSuggestions.Add('r', new List<char>(){'ř'});
        _letterSuggestions.Add('z', new List<char>(){'ž'});
    }

    public List<StopModel> GetStopsByName(string name, string dbPath, string dbName)
    {
        try
        {
            var stopModels = new List<StopModel>();
            var suggestions = GetSuggestions(name);
            using (var connection = new SqliteConnection($"Data Source={dbPath}\\{dbName}"))
            {
                connection.Open();
                using (var command = new SqliteCommand())
                {
                    command.Connection = connection;
                    var sb = new StringBuilder();
                    sb.Append("SELECT Id, Name, Latitude, Longitude FROM Stops WHERE Name ");
                    for (int i = 0; i < suggestions.Count; i++)
                    {
                        sb.Append($"LIKE @name{i}");
                        if (suggestions.Count - i > 1)
                        {
                            sb.Append(" OR Name ");
                        }
                    }
                    command.CommandText = sb.ToString();
                    for(int i = 0; i < suggestions.Count; i++)
                    {
                        command.Parameters.AddWithValue($"@name{i}", '%' + suggestions[i].ToLower() + '%');
                    }
                    
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            stopModels.Add(new StopModel()
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Latitude = reader.GetDecimal(2),
                                Longitude = reader.GetDecimal(3)
                            });
                        }
                    }
                    reader.Close();                  
                }
                connection.Close();
                return stopModels;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public List<PlatformModel> GetPlatformNameByCode(string[] codes, string dbPath, string dbName)
    {
        try
        {
            var result = new List<PlatformModel>();
            using (var connection = new SqliteConnection($"Data Source={dbPath}\\{dbName}"))
            {
                connection.Open();
                using (var command = new SqliteCommand())
                {
                    command.Connection = connection;
                    var res = new StringBuilder();
                    res.Append("SELECT Name, Latitude, Longitude FROM Platforms WHERE GtfsIds LIKE ");
                    for (int i = 0; i < codes.Length; i++)
                    {
                        res.Append($"\'%{codes[i]}%\'");
                        if (codes.Length - i > 1)
                        {
                            res.Append(" OR GtfsIds LIKE ");
                        }
                    }
                    command.CommandText = res.ToString();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new PlatformModel()
                            {
                                Name = reader.GetString(0),
                                Latitude = reader.GetDecimal(1),
                                Longitude = reader.GetDecimal(2)
                            }); 
                        }
                    }
                    reader.Close(); 
                }
            }
            return result;
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    public StopModel GetStopWithPlatforms(string stopName, string dbPath, string dbName)
    {
        try
        {
            var stopModel = new StopModel();
            using (var connection = new SqliteConnection($"Data Source={dbPath}\\{dbName}"))
            {
                connection.Open();
                using (var command = new SqliteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT Id, Name, Latitude, Longitude FROM Stops WHERE Name = @name";
                    command.Parameters.AddWithValue("@name", stopName);
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            stopModel.Id = reader.GetInt32(0);
                            stopModel.Name = reader.GetString(1);
                            stopModel.Latitude = reader.GetDecimal(2);
                            stopModel.Longitude = reader.GetDecimal(3);
                        }
                    }
                    reader.Close(); 
                    stopModel.Stops = new List<PlatformModel>();
                    command.CommandText = "SELECT Id, PlatformCode, Name, GtfsIds, Zone, StopId, TransportType, Latitude, Longitude FROM Platforms WHERE StopId = @stopId";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@stopId", stopModel.Id);
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            stopModel.Stops.Add(new PlatformModel()
                            {
                                Id = reader.GetInt32(0),
                                PlatformCode = reader.GetString(1),
                                Name = reader.GetString(2),
                                GtfsIdsString = reader.GetString(3),
                                Zone = reader.GetString(4),
                                StopId = reader.GetInt32(5),
                                TransportType = reader.GetString(6),
                                Latitude = reader.GetDecimal(7),
                                Longitude = reader.GetDecimal(8)
                            });
                        }
                    }
                    reader.Close();
                                      
                }
                connection.Close();
                return stopModel;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }            
    }

    private List<string> GetSuggestions(string input)
    {
        var result = new List<string>();
        result.Add(input);
        foreach(var suggestion in _letterSuggestions)
        {
            foreach(var val in suggestion.Value)
            {
                var replaced = input.Replace(suggestion.Key, val);
                result.Add(replaced);
            }
        }
        return result.Distinct().ToList();
    }
}
