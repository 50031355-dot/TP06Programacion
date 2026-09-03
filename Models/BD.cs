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
            string query = "SELECT ID, mail, nombre, contrasena, idPartida FROM Usuarios WHERE mail = @mail AND contrasena = @password";
            return connection.QuerySingleOrDefault<Usuarios>(query, new { mail, password });
        }
    }

    //Actualiza la sala actual de la partida del usuario
    public void ActualizarSalaActual(int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "UPDATE Partidas SET salaActual = salaActual+1 WHERE ID = @idUsuario";
            connection.Execute(query, new { idUsuario });
        }
    }

    //Una función que devuelve la respuesta correcta para una sala específica
    public int ObtenerRespuestaSala(int idSala)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT clave FROM Salas WHERE ID = @idSala";
            return connection.QuerySingleOrDefault<int>(query, new { idSala });
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

    // Obtener usuario por email. El método debe primero buscar si existe el usuario con ese mail (query con dapper que devuelve un int): //si existe, buscar el usuario con ese mail y devolverlo, si no existe, devolver null. El método debe ser público.
    public Usuarios ObtenerUsuarioPorEmail(string email)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT ID, mail, nombre, password, idPartida FROM Usuarios WHERE mail = @email";
            int count = connection.QuerySingleOrDefault<int>(query, new { email });
            if (count != 0)
            {
                Usuarios u= connection.QuerySingleOrDefault<Usuarios>(query, new { email });
                return u;
            }
            else
            {
                return null;
            }
        }
    }

    /*public string ObtenerPistaSala(int salaActual, int num)
    {
        using (SqlConnection = new SqlConnection(_connectionString))
        {
            string query = "SELECT contenido FROM Pistas WHERE idSala=@salaActual AND num=@num";
            string contenido = connection.QuerySingleOrDefault<string>(query, new {salaActual, num});
            return contenido;
        }   
    }
*/
}
