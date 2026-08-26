using Microsoft.Data.SqlClient;
using Dapper;

private string _connectionString = "Server=localhost;Database=tp06;Integrated Security=True;TrustServerCertificate=True;";

//Haceme unmetodo que cree un usuario en la base de datos con los parametros delaclase Usuarios.cs. 
public void CrearUsuario(Usuarios usuario)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = "INSERT INTO Usuarios (mail, nombre, idPartida, idSala) VALUES (@mail, @nombre, @idPartida, @idSala)";
        connection.Execute(query, usuario);
    }
}

//

//Haceme un metodoque devuelva el atributo salaActual de la partida que tenga el idPartida del Usuario.
public int ObtenerSalaActual(Usuarios usuario)
{
    int idPartida = usuario.idPartida;
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = "SELECT salaActual FROM Partidas WHERE ID = @idPartida";
        return connection.QuerySingleOrDefault<int>(query, new { idPartida });
    }
}

