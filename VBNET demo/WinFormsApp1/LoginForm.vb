Imports WinFormsApp1.DatabaseConnector
Imports MySql.Data.MySqlClient


Public Class LoginForm
    Private ReadOnly dbConnector As New DatabaseConnector()

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim dbConnector As New DatabaseConnector()
        Dim validationResult As Integer = dbConnector.ValidateUser(txtUsername.Text, txtPassword.Text)

        If validationResult = 1 Then
            dbConnector.AddSuccessfulLogin(txtUsername.Text)
            MessageBox.Show(dbConnector.GetSuccessfulLogins(txtUsername.Text))
        ElseIf validationResult = 2 Then
            Dim userId As Integer = GetUserid(txtUsername.Text)
            dbConnector.AddFailedLogin(txtUsername.Text, userId)
            MessageBox.Show("Invalid password.")
        Else
            dbConnector.AddFailedLogin(txtUsername.Text, Nothing)
            MessageBox.Show("Invalid username.")
        End If
    End Sub

    Private Function GetUserid(username As String) As Integer?
        Using connection As New MySqlConnection("server=104.210.70.61;userid=TestDev;password=TestDev;database=testdb;")
            connection.Open()
            Dim command As New MySqlCommand("SELECT user_id FROM user_info WHERE user_name = @username", connection)
            command.Parameters.AddWithValue("@username", username)

            Using reader As MySqlDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    Return reader("user_id")
                Else
                    Return Nothing
                End If
            End Using
        End Using
    End Function


    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub


End Class

