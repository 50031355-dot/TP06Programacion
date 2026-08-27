using Microsoft.Data.SqlClient;
using Dapper;

namespace tp06.Models;

public class BD
{
    private string _connectionString = "Server=localhost;Database=tp06;Integrated Security=True;TrustServerCertificate=True;";

    // Autenticar usuario
    public Usuarios AutenticarUsuario(string mail, string password)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT ID, mail, nombre, password, idPartida FROM Usuarios WHERE mail = @mail AND password = @password";
            return connection.QuerySingleOrDefault<Usuarios>(query, new { mail, password });
        }
    }

    // Crear usuario en la base de datos
    public void CrearUsuario(Usuarios usuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "INSERT INTO Usuarios (mail, nombre, password, idPartida) VALUES (@mail, @nombre, @password, @idPartida)";
            connection.Execute(query, usuario);
        }
    }

    // Obtener sala actual de la partida del usuario
    public int ObtenerSalaActual(Usuarios usuario)
    {
        int idPartida = usuario.idPartida;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT salaActual FROM Partidas WHERE ID = @idPartida";
            return connection.QuerySingleOrDefault<int>(query, new { idPartida });
        }
    }
}

