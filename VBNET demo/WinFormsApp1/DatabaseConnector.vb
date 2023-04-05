Imports MySql.Data.MySqlClient

Public Class DatabaseConnector
    Private connectionString As String

    Public Sub New()
        connectionString = "server=104.210.70.61;userid=TestDev;password=TestDev;database=testdb;"
    End Sub

    Public Function ValidateUser(username As String, password As String) As Integer
        Using connection As New MySqlConnection(connectionString)
            connection.Open()
            Dim command As New MySqlCommand("SELECT user_id, user_password FROM user_info WHERE user_name = @username", connection)
            command.Parameters.AddWithValue("@username", username)

            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    If reader("user_password") = password Then
                        Return 1 ' Successful login
                    Else
                        Return 2 ' Failed login
                    End If
                Else
                    Return 0 ' User not found
                End If
            End Using
        End Using
    End Function

    Public Sub AddSuccessfulLogin(username As String)
        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim getUserIdCommand As New MySqlCommand("SELECT user_id FROM user_info WHERE user_name = @username", connection)
            getUserIdCommand.Parameters.AddWithValue("@username", username)
            Dim userId As Integer = CInt(getUserIdCommand.ExecuteScalar())

            Dim command As New MySqlCommand("INSERT INTO login_attempts (login_user_id, success, login_date_time) VALUES (@user_id, 1, CURRENT_TIMESTAMP)", connection)
            command.Parameters.AddWithValue("@user_id", userId)
            command.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub AddFailedLogin(username As String, userId As Integer?)
        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim command As New MySqlCommand("INSERT INTO login_attempts (login_user_id, success, login_date_time) VALUES (@user_id, 0, CURRENT_TIMESTAMP)", connection)
            If userId.HasValue Then
                command.Parameters.AddWithValue("@user_id", userId.Value)
            Else
                command.Parameters.AddWithValue("@user_id", DBNull.Value)
            End If

            command.ExecuteNonQuery()
        End Using
    End Sub

    Public Function GetSuccessfulLogins(username As String) As String
        Using connection As New MySqlConnection(connectionString)
            connection.Open()
            Dim command As New MySqlCommand("SELECT user_name, MAX(login_date_time) as last_login_time, SUM(success) as success FROM login_attempts INNER JOIN user_info ON login_attempts.login_user_id = user_info.user_id WHERE user_name = @username AND success = 1", connection)
            command.Parameters.AddWithValue("@username", username)

            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    Return $"Past successful logins: Username: {reader("user_name")} - Last Login Time: {reader("last_login_time")} - Success: {reader("success")}"
                Else
                    Return "No successful logins found."
                End If
            End Using
        End Using
    End Function




End Class
