namespace Server;

public class Users
{

    public record SingleDTO(string Firstname, string Lastname, string Email, string Role);
    public record AllDTO(int Id, string Firstname, string Lastname, string Email, string Role);
    public record PostDTO(string Firstname, string Lastname, string Email, string Password);

    public static void Post(PostDTO user, State state)
    {
        var cmd = state.DB.CreateCommand("insert into users(firstname, lastname, email, password) values($1, $2, $3, $4)");
        cmd.Parameters.AddWithValue(user.Firstname);
        cmd.Parameters.AddWithValue(user.Lastname);
        cmd.Parameters.AddWithValue(user.Email);
        cmd.Parameters.AddWithValue(user.Password);
        cmd.ExecuteNonQuery();
    }

    public static List<AllDTO> All(State state)
    {

        List<AllDTO> result = new();

        var cmd = state.DB.CreateCommand("select user_id as id, firstname, lastname, email, role from users");
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
        }
        return result;
    }

    public static SingleDTO? Single(int id, State state)
    {
        var cmd = state.DB.CreateCommand("select firstname, lastname, email, role from users where user_id = $1");
        cmd.Parameters.AddWithValue(id);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
        }
        else
        {
            return null;
        }
    }


}