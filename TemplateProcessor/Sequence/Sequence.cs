using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TemplateProcessor.Sequence
{
    public class Sequence
    {
        private string connectionString;
        private string server;

        public Sequence(string server, string database, string username, string password)
        {
            // Initialize server and connection string
            this.server = server;
            connectionString = $"Server={server},1433;Database={database};User Id={username};Password={password};";
        }

        // Method to fetch data from the Sequence table
        public DataTable GetSequenceData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    HelpFunc.LogMessage($"Connection to {server} successful!");
                    string query = "SELECT * FROM Sequence";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL Error: {sqlEx.Message}\nError Code: {sqlEx.ErrorCode}\nState: {sqlEx.State}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        // Method to fetch data for Steps related to a Sequence
        public DataTable GetStepData(int sequenceId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Step WHERE SequenceId = @SequenceId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SequenceId", sequenceId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable stepData = new DataTable();
                    adapter.Fill(stepData);
                    return stepData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Data Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Method to fetch data from the MultilanguageDescription table
        public DataTable LoadSequenceDescriptionData(int descriptionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM MultilanguageDescription WHERE DescriptionId = @DescriptionId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DescriptionId", descriptionId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable descriptionData = new DataTable();
                    adapter.Fill(descriptionData);
                    return descriptionData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Data Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        // Method to fetch Step descriptions
        public DataTable LoadStepDescriptionData(int stepId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM MultilanguageDescription WHERE EntityId = @StepId AND EntityType = 'Step'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StepId", stepId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable stepDescriptionData = new DataTable();
                    adapter.Fill(stepDescriptionData);
                    return stepDescriptionData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Data Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        // Method to add a new sequence
        public void AddSequence()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Define the SQL command to call the stored procedure
                    SqlCommand command = new SqlCommand("AddSequence", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Execute the command and handle the result
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        int newSequenceId = Convert.ToInt32(result);
                        MessageBox.Show($"Sequence added successfully! New SequenceId: {newSequenceId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to add sequence or retrieve the new SequenceId.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void DeleteSequence(int sequenceId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Define the SQL command to call the stored procedure
                    string storedProc = "DeleteSequence";

                    using (SqlCommand command = new SqlCommand(storedProc, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the SequenceId parameter to the command
                        command.Parameters.AddWithValue("@SequenceId", sequenceId);

                        // Execute the stored procedure
                        int rowsAffected = command.ExecuteNonQuery();

                        // Optionally, show a success message
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sequence deleted successfully using the stored procedure!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete sequence.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public int AddStep(int sequenceId, int stepNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Define the SQL command to call the stored procedure
                    SqlCommand command = new SqlCommand("AddStep", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@SequenceId", sequenceId);
                    command.Parameters.AddWithValue("@StepNumber", stepNumber);

                    // Execute the command and handle the result
                    int result = Convert.ToInt32(command.ExecuteScalar());
                    return result; // Return the result indicating success or failure
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -3; // You can define custom error codes as needed
                }
            }
        }

        public void DeleteStep(int stepId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Define the SQL command to call the stored procedure
                    SqlCommand command = new SqlCommand("DeleteStep", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Add the StepId parameter
                    command.Parameters.AddWithValue("@StepId", stepId);

                    // Add a return value parameter
                    SqlParameter returnValue = new SqlParameter("@ReturnVal", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(returnValue);

                    // Execute the command
                    command.ExecuteNonQuery();

                    // Retrieve the return value
                    int result = (int)returnValue.Value;

                    // Handle the return value
                    if (result == 0)
                    {
                        MessageBox.Show("Step deleted successfully using the stored procedure!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == -1)
                    {
                        MessageBox.Show("Failed to delete step: StepId does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("An unknown error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // Add this method to your Sequence class
        public DataTable GetTransitionData(int sequenceId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Transition WHERE StepId = @StepId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StepId", sequenceId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable transitionData = new DataTable();
                    adapter.Fill(transitionData);
                    return transitionData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Data Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Add this method for adding a transition
        public int AddTransition(int stepId, int transNumber, int jumpN = 0)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("AddTransition", connection); // Ensure you have this stored procedure
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@StepId", stepId);
                    command.Parameters.AddWithValue("@TransN", transNumber);
                    command.Parameters.AddWithValue("@JumpN", jumpN); // Set JumpN to 0 by default

                    int result = Convert.ToInt32(command.ExecuteScalar());
                    return result; // Return the result indicating success or failure
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -3; // Define custom error codes as needed
                }
            }
        }

        // Add this method for deleting a transition
        public void DeleteTransition(int transitionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Define the SQL command to call the stored procedure
                    SqlCommand command = new SqlCommand("DeleteTransition", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Add the TransitionId parameter
                    command.Parameters.AddWithValue("@TransitionId", transitionId);

                    // Add a return value parameter
                    SqlParameter returnValue = new SqlParameter("@ReturnVal", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(returnValue);

                    // Execute the command
                    command.ExecuteNonQuery();

                    // Retrieve the return value
                    int result = (int)returnValue.Value;

                    // Handle the return value
                    if (result == 0)
                    {
                        MessageBox.Show("Transition deleted successfully using the stored procedure!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == -1)
                    {
                        MessageBox.Show("Failed to delete transition: TransitionId does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("An unknown error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Add this method to load Transition descriptions
        public DataTable LoadTransitionDescriptionData(int transitionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM MultilanguageDescription WHERE EntityId = @TransitionId AND EntityType = 'Transition'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TransitionId", transitionId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable transitionDescriptionData = new DataTable();
                    adapter.Fill(transitionDescriptionData);
                    return transitionDescriptionData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Data Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Method to fetch Condition data related to a Transition
        public DataTable GetConditionData(int transitionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Condition WHERE TransitionId = @TransitionId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TransitionId", transitionId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable conditionData = new DataTable();
                    adapter.Fill(conditionData);
                    return conditionData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Data Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Method to fetch Condition descriptions
        public DataTable LoadConditionDescriptionData(int conditionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM MultilanguageDescription WHERE EntityId = @ConditionId AND EntityType = 'Condition'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ConditionId", conditionId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable conditionDescriptionData = new DataTable();
                    adapter.Fill(conditionDescriptionData);
                    return conditionDescriptionData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Data Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Method to add a new condition
        public int AddCondition(int transitionId, int conditionNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("AddCondition", connection); // Ensure you have this stored procedure
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TransitionId", transitionId);
                    command.Parameters.AddWithValue("@CondN", conditionNumber);

                    int result = Convert.ToInt32(command.ExecuteScalar());
                    return result; // Return the result indicating success or failure
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -3; // Define custom error codes as needed
                }
            }
        }

        // Method to delete a condition
        public void DeleteCondition(int conditionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Define the SQL command to call the stored procedure
                    SqlCommand command = new SqlCommand("DeleteCondition", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Add the ConditionId parameter
                    command.Parameters.AddWithValue("@ConditionId", conditionId);

                    // Add a return value parameter
                    SqlParameter returnValue = new SqlParameter("@ReturnVal", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(returnValue);

                    // Execute the command
                    command.ExecuteNonQuery();

                    // Retrieve the return value
                    int result = (int)returnValue.Value;

                    // Handle the return value
                    if (result == 0)
                    {
                        MessageBox.Show("Condition deleted successfully using the stored procedure!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == -1)
                    {
                        MessageBox.Show("Failed to delete condition: ConditionId does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("An unknown error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    } //
}
