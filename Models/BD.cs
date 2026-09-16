using Microsoft.Data.SqlClient;
using Dapper;

namespace tp06.Models;

public class BD
{
    private string _connectionString = "Server=localhost;Database=tp06;Integrated Security=True;TrustServerCertificate=True;";

    // Autenticar usuario
    public Usuarios AutenticarUsuario(string mail)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT ID, mail, nombre,idPartida FROM Usuarios WHERE mail = @mail";
            return connection.QuerySingleOrDefault<Usuarios>(query, new { mail});
        }
    }


    //Funcion que recibe un mail y crea un usuario(solo pone el mail) y crea una partida nueva (pone salaActual=1 y tiempo =date actual) y los relaciona
    //Hace que devuelva el nuevo usuario creado (con el idPartida que le corresponde)
    public Usuarios CrearUsuarioYPartida(string mail)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            // Crear partida nueva
            string queryPartida = @"INSERT INTO Partidas (nombre, salaActual, tiempo, progreso)
                                    OUTPUT INSERTED.ID
                                    VALUES (@nombre, 1, GETDATE(), @progreso)";
            int idPartida = connection.QuerySingle<int>(queryPartida, new { nombre = mail, progreso = "" });

            // Crear usuario y relacionarlo con la partida
            string queryUsuario = "INSERT INTO Usuarios (mail, nombre, idPartida) VALUES (@mail, @nombre, @idPartida)";
            connection.Execute(queryUsuario, new { mail, nombre = mail, idPartida });
        }
        return AutenticarUsuario(mail);
    }
    
    //Actualiza la sala actual de la partida del usuario
    public void ActualizarSalaActual(int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "UPDATE Partidas SET salaActual = salaActual+1 WHERE ID IN (SELECT idPartida FROM Usuarios WHERE ID = @idUsuario)";
            connection.Execute(query, new { idUsuario });
        }
    }
    //Reinicia SalaActual=1 de la partida del usuario
    public void ReiniciarSalaActual(int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "UPDATE Partidas SET salaActual = 1 WHERE ID IN (SELECT idPartida FROM Usuarios WHERE ID = @idUsuario)";
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

    public string ObtenerPistaSala(int salaActual, int numeroPista) //////
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            int offset = numeroPista - 1;
            string query = "SELECT contenido FROM Pistas WHERE idSala = @salaActual ORDER BY ID OFFSET @offset ROWS FETCH NEXT 1 ROWS ONLY";
            return connection.QuerySingleOrDefault<string>(query, new { salaActual, offset });
        }
    }

    // Crear usuario en la base de datos
    public void CrearUsuario(Usuarios usuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "INSERT INTO Usuarios (mail, nombre, idPartida) VALUES (@mail, @nombre,@idPartida)";
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

    // Obtener sala actual de la partida del usuario con el id de la partida
    public int ObtenerSalaActual(int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT salaActual FROM Partidas INNER JOIN Usuarios ON Partidas.ID = Usuarios.idPartida WHERE Usuarios.ID = @idUsuario";
            return connection.QuerySingleOrDefault<int>(query, new { idUsuario });
        }
    }


    // Obtener usuario por email. El método debe primero buscar si existe el usuario con ese mail (query con dapper que devuelve un int): //si existe, buscar el usuario con ese mail y devolverlo, si no existe, devolver null. El método debe ser público.
    public Usuarios ObtenerUsuarioPorEmail(string email)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT ID, mail, nombre,idPartida FROM Usuarios WHERE mail = @email";
            return connection.QuerySingleOrDefault<Usuarios>(query, new { email });
        }
    }
}
