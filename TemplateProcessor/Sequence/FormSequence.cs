using System;
using System.Data;
using System.Windows.Forms;

namespace TemplateProcessor.Sequence
{
    public partial class FormSequence : Form
    {
        // Fields for the connection
        private string server = GlobalData.Instance.Server2;
        private string database = GlobalData.Instance.Database2;
        private string username = GlobalData.Instance.Username2;
        private string password = GlobalData.Instance.Password2;

        public FormSequence()
        {
            InitializeComponent();

            // Initialize and load the Sequence data into the first DataGridView
            LoadSequenceData();
        }

        // Method to load sequence data into the DataGridView
        private void LoadSequenceData()
        {
            Sequence sequence = new Sequence(server, database, username, password);
            DataTable sequenceData = sequence.GetSequenceData();
            if (sequenceData != null)
            {
                dataGridViewSequence.DataSource = sequenceData;
            }
        }

        // Event handler for when a row is selected in the first DataGridView
        private void dataGridViewSequence_SelectionChanged(object sender, EventArgs e)
        {
            {
                if (dataGridViewSequence.SelectedRows.Count > 0)
                {
                    // Get the selected row from the first grid
                    DataGridViewRow selectedRow = dataGridViewSequence.SelectedRows[0];

                    // Extract the DescriptionId from the selected row
                    int descriptionId = Convert.ToInt32(selectedRow.Cells["DescriptionId"].Value);

                    // Fetch and display data for the second DataGridView (dataGridViewSequenceDesc)
                    Sequence sequence = new Sequence(server, database, username, password);
                    DataTable descriptionData = sequence.LoadSequenceDescriptionData(descriptionId);

                    // Bind the data to the second DataGridView (dataGridViewSequenceDesc)
                    if (descriptionData != null)
                    {
                        dataGridViewSequenceDesc.DataSource = descriptionData;
                    }

                    // Extract the SequenceId from the selected row
                    int sequenceId = Convert.ToInt32(selectedRow.Cells["SequenceId"].Value);

                    // Load Step data for the selected SequenceId
                    LoadStepData(sequenceId);

                    // After loading step data, select the first row in dataGridViewStep
                    if (dataGridViewStep.Rows.Count > 0)
                    {
                        dataGridViewStep.Rows[0].Selected = true;

                        // Extract the StepId from the first row
                        int stepId = Convert.ToInt32(dataGridViewStep.Rows[0].Cells["StepId"].Value);

                        // Load Step description data
                        LoadStepDescriptionData(stepId);

                        // Load Transition data for the selected StepId
                        LoadTransitionData(stepId);

                        // After loading transition data, select the first row in dataGridViewTransition
                        if (dataGridViewTransition.Rows.Count > 0)
                        {
                            dataGridViewTransition.Rows[0].Selected = true;

                            // Extract the TransitionId from the first row
                            int transitionId = Convert.ToInt32(dataGridViewTransition.Rows[0].Cells["TransitionId"].Value);

                            // Load Transition description data
                            LoadTransitionDescriptionData(transitionId);

                            // Load Condition data for the selected TransitionId
                            LoadConditionData(transitionId);

                            // After loading condition data, select the first row in dataGridViewCondition
                            if (dataGridViewCondition.Rows.Count > 0)
                            {
                                dataGridViewCondition.Rows[0].Selected = true;

                                // Extract the ConditionId from the first row
                                int conditionId = Convert.ToInt32(dataGridViewCondition.Rows[0].Cells["ConditionId"].Value);

                                // Load Condition description data
                                LoadConditionDescriptionData(conditionId);
                            }
                            else
                            {
                                dataGridViewCondition.DataSource = null;
                                dataGridViewConditionDesc.DataSource = null;
                            }
                        }
                        else
                        {
                            dataGridViewTransition.DataSource = null;
                            dataGridViewTransitionDesc.DataSource = null;
                            dataGridViewCondition.DataSource = null;
                            dataGridViewConditionDesc.DataSource = null;
                        }
                    }
                    else
                    {
                        dataGridViewStep.DataSource = null;
                        dataGridViewStepDesc.DataSource = null;
                        dataGridViewTransition.DataSource = null;
                        dataGridViewTransitionDesc.DataSource = null;
                        dataGridViewCondition.DataSource = null;
                        dataGridViewConditionDesc.DataSource = null;
                    }
                }
            }
        }

        private void btnAddSequence_Click(object sender, EventArgs e)
        {
            Sequence sequence = new Sequence(server, database, username, password);
            sequence.AddSequence();

            // Refresh the DataGridView after adding a new sequence
            LoadSequenceData();
        }

        private void btnDeleteSequence_Click(object sender, EventArgs e)
        {
            {
                if (dataGridViewSequence.SelectedRows.Count > 0)
                {
                    // Get the selected row from the first grid
                    DataGridViewRow selectedRow = dataGridViewSequence.SelectedRows[0];

                    // Extract the SequenceId from the selected row
                    int sequenceId = Convert.ToInt32(selectedRow.Cells["SequenceId"].Value);

                    // Delete the selected sequence
                    Sequence sequence = new Sequence(server, database, username, password);
                    sequence.DeleteSequence(sequenceId);

                    // Refresh the DataGridView after deleting a sequence
                    LoadSequenceData();
                }
                else
                {
                    MessageBox.Show("Please select a sequence to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // Method to load Step data into the second DataGridView
        private void LoadStepData(int sequenceId)
        {
            Sequence sequence = new Sequence(server, database, username, password);
            DataTable stepData = sequence.GetStepData(sequenceId);
            if (stepData != null && stepData.Rows.Count > 0)
            {
                dataGridViewStep.DataSource = stepData;

                // Optionally, if you want to load the description of the first step as well
                LoadStepDescriptionData(Convert.ToInt32(stepData.Rows[0]["StepId"]));
            }
            else
            {
                dataGridViewStep.DataSource = null; // Clear if no steps
                dataGridViewStepDesc.DataSource = null; // Clear descriptions
            }
        }
        // Method to load Step descriptions into the dataGridViewStepDesc
        private void LoadStepDescriptionData(int stepId)
        {
            Sequence sequence = new Sequence(server, database, username, password);
            DataTable stepDescriptionData = sequence.LoadStepDescriptionData(stepId);
            if (stepDescriptionData != null)
            {
                dataGridViewStepDesc.DataSource = stepDescriptionData;
            }
        }
        // Method to load Transition data into the new DataGridView
        private void LoadTransitionData(int stepId)
        {
            Sequence sequence = new Sequence(server, database, username, password);
            DataTable transitionData = sequence.GetTransitionData(stepId); // Implement this method in Sequence class
            if (transitionData != null && transitionData.Rows.Count > 0)
            {
                dataGridViewTransition.DataSource = transitionData;
                LoadTransitionDescriptionData(Convert.ToInt32(transitionData.Rows[0]["TransitionId"])); // Optional
            }
            else
            {
                dataGridViewTransition.DataSource = null;
                dataGridViewTransitionDesc.DataSource = null;
            }
        }

        // Method to load Transition descriptions
        private void LoadTransitionDescriptionData(int transitionId)
        {
            Sequence sequence = new Sequence(server, database, username, password);
            DataTable transitionDescriptionData = sequence.LoadTransitionDescriptionData(transitionId); // Implement this method in Sequence class
            if (transitionDescriptionData != null)
            {
                dataGridViewTransitionDesc.DataSource = transitionDescriptionData;
            }
        }
        // Method to load Condition data into the DataGridView
        private void LoadConditionData(int transitionId)
        {
            Sequence sequence = new Sequence(server, database, username, password);
            DataTable conditionData = sequence.GetConditionData(transitionId);
            if (conditionData != null && conditionData.Rows.Count > 0)
            {
                dataGridViewCondition.DataSource = conditionData;
                LoadConditionDescriptionData(Convert.ToInt32(conditionData.Rows[0]["ConditionId"])); // Optional
            }
            else
            {
                dataGridViewCondition.DataSource = null;
                dataGridViewConditionDesc.DataSource = null;
            }
        }

        // Method to load Condition descriptions
        private void LoadConditionDescriptionData(int conditionId)
        {
            Sequence sequence = new Sequence(server, database, username, password);
            DataTable conditionDescriptionData = sequence.LoadConditionDescriptionData(conditionId);
            if (conditionDescriptionData != null)
            {
                dataGridViewConditionDesc.DataSource = conditionDescriptionData;
            }
        }

        // Event handler for when a row is selected in the steps DataGridView
        private void dataGridViewStep_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewStep.SelectedRows.Count > 0)
            {
                // Get the selected row from the Step grid
                DataGridViewRow selectedRow = dataGridViewStep.SelectedRows[0];

                // Extract the StepId from the selected row
                int stepId = Convert.ToInt32(selectedRow.Cells["StepId"].Value);

                // Fetch and display data for Step descriptions
                Sequence sequence = new Sequence(server, database, username, password);
                DataTable stepDescriptionData = sequence.LoadStepDescriptionData(stepId);

                // Bind the data to the second DataGridView (dataGridViewStepDesc)
                if (stepDescriptionData != null)
                {
                    dataGridViewStepDesc.DataSource = stepDescriptionData;
                }

                // Load Transition data for the selected StepId
                LoadTransitionData(stepId);

                // After loading transition data, select the first row in dataGridViewTransition
                if (dataGridViewTransition.Rows.Count > 0)
                {
                    dataGridViewTransition.Rows[0].Selected = true;

                    // Extract the TransitionId from the first row
                    int transitionId = Convert.ToInt32(dataGridViewTransition.Rows[0].Cells["TransitionId"].Value);

                    // Load Transition description data
                    LoadTransitionDescriptionData(transitionId);

                    // Load Condition data for the selected TransitionId
                    LoadConditionData(transitionId);

                    // After loading condition data, select the first row in dataGridViewCondition
                    if (dataGridViewCondition.Rows.Count > 0)
                    {
                        dataGridViewCondition.Rows[0].Selected = true;

                        // Extract the ConditionId from the first row
                        int conditionId = Convert.ToInt32(dataGridViewCondition.Rows[0].Cells["ConditionId"].Value);

                        // Load Condition description data
                        LoadConditionDescriptionData(conditionId);
                    }
                    else
                    {
                        dataGridViewCondition.DataSource = null;
                        dataGridViewConditionDesc.DataSource = null;
                    }
                }
                else
                {
                    dataGridViewTransition.DataSource = null;
                    dataGridViewTransitionDesc.DataSource = null;
                    dataGridViewCondition.DataSource = null;
                    dataGridViewConditionDesc.DataSource = null;
                }
            }
        }

        private void btnDeleteStep_Click(object sender, EventArgs e)
        {
            if (dataGridViewStep.SelectedRows.Count > 0)
            {
                // Get the selected row from the Step grid
                DataGridViewRow selectedRow = dataGridViewStep.SelectedRows[0];

                // Extract the StepId from the selected row
                int stepId = Convert.ToInt32(selectedRow.Cells["StepId"].Value);

                // Extract the SequenceId from the currently selected row in the Sequence grid
                if (dataGridViewSequence.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedSequenceRow = dataGridViewSequence.SelectedRows[0];
                    int sequenceId = Convert.ToInt32(selectedSequenceRow.Cells["SequenceId"].Value);

                    // Delete the selected step
                    Sequence sequence = new Sequence(server, database, username, password);
                    sequence.DeleteStep(stepId);

                    // Reload the step data for the selected sequence
                    LoadStepData(sequenceId); // Make sure to implement this method to reload step data
                }
                else
                {
                    MessageBox.Show("Please select a sequence to associate with this step.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a step to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnAddStep_Click(object sender, EventArgs e)
        {
            if (dataGridViewSequence.SelectedRows.Count > 0)
            {
                // Get the selected row from the Sequence grid
                DataGridViewRow selectedRow = dataGridViewSequence.SelectedRows[0];
                int sequenceId = Convert.ToInt32(selectedRow.Cells["SequenceId"].Value);

                // Prompt the user for the step number to add

                if (int.TryParse(tbxStepNumber.Text, out int stepNumber) && stepNumber >= 0 && stepNumber <= 49)
                {
                    Sequence sequence = new Sequence(server, database, username, password);
                    // Call the method to add the step using the stored procedure
                    int result = sequence.AddStep(sequenceId, stepNumber); // Assuming AddStep returns an int indicating the result

                    // Handle the result accordingly
                    if (result == 0) // Assume 0 means success
                    {
                        MessageBox.Show("Step added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == -1) // Assume -1 means step already exists
                    {
                        MessageBox.Show("Step already exists for this sequence.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (result == -2) // Assume -2 means sequence does not exist
                    {
                        MessageBox.Show("Sequence does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Refresh the Steps DataGridView after adding a new step
                    LoadStepData(sequenceId);
                }
                else
                {
                    MessageBox.Show("Please enter a valid step number between 0 and 49.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a sequence to add a step.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteTransition_Click(object sender, EventArgs e)
        {
            if (dataGridViewTransition.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewTransition.SelectedRows[0];
                int transitionId = Convert.ToInt32(selectedRow.Cells["TransitionId"].Value);

                Sequence sequence = new Sequence(server, database, username, password);
                sequence.DeleteTransition(transitionId); // Implement this method in Sequence class

                // Refresh the Transition DataGridView
                if (dataGridViewStep.SelectedRows.Count > 0)
                {
                    int stepId = Convert.ToInt32(dataGridViewStep.SelectedRows[0].Cells["StepId"].Value);
                    LoadTransitionData(stepId);

                    // After reloading, check if there are rows in dataGridViewTransition
                    if (dataGridViewTransition.Rows.Count > 0)
                    {
                        // Select the first row
                        dataGridViewTransition.Rows[0].Selected = true;

                        // Use the selected row to refresh the condition grid
                        int newTransitionId = Convert.ToInt32(dataGridViewTransition.SelectedRows[0].Cells["TransitionId"].Value);
                        LoadConditionData(newTransitionId);
                    }
                    else
                    {
                        dataGridViewCondition.DataSource = null;
                        dataGridViewConditionDesc.DataSource = null;
                    }
                }
                else
                {
                    dataGridViewTransition.DataSource = null;
                    dataGridViewCondition.DataSource = null;
                    dataGridViewConditionDesc.DataSource = null;
                }
            }
            else
            {
                MessageBox.Show("Please select a transition to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnAddTransition_Click(object sender, EventArgs e)
        {
            if (dataGridViewStep.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewStep.SelectedRows[0];
                int stepId = Convert.ToInt32(selectedRow.Cells["StepId"].Value);

                if (int.TryParse(tbxTransNumber.Text, out int transNumber) && transNumber >= 0)
                {
                    Sequence sequence = new Sequence(server, database, username, password);
                    int result = sequence.AddTransition(stepId, transNumber); // Implement this method in Sequence class

                    if (result == 0) // Assume 0 means success
                    {
                        MessageBox.Show("Transition added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to add transition.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Refresh the Transition DataGridView
                    LoadTransitionData(stepId);
                }
                else
                {
                    MessageBox.Show("Please enter a valid transition number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a sequence to add a transition.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewTransition_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewTransition.SelectedRows.Count > 0)
            {
                // Get the selected row from the Transition grid
                DataGridViewRow selectedRow = dataGridViewTransition.SelectedRows[0];

                // Extract the TransitionId from the selected row
                int transitionId = Convert.ToInt32(selectedRow.Cells["TransitionId"].Value);

                // Fetch and display data for Transition descriptions
                Sequence sequence = new Sequence(server, database, username, password);
                DataTable transitionDescriptionData = sequence.LoadTransitionDescriptionData(transitionId);

                // Bind the data to the second DataGridView (dataGridViewTransitionDesc)
                if (transitionDescriptionData != null)
                {
                    dataGridViewTransitionDesc.DataSource = transitionDescriptionData;
                }

                // Load Condition data for the selected TransitionId
                LoadConditionData(transitionId);

                // After loading condition data, select the first row in dataGridViewCondition
                if (dataGridViewCondition.Rows.Count > 0)
                {
                    dataGridViewCondition.Rows[0].Selected = true;

                    // Extract the ConditionId from the first row
                    int conditionId = Convert.ToInt32(dataGridViewCondition.Rows[0].Cells["ConditionId"].Value);

                    // Load Condition description data
                    LoadConditionDescriptionData(conditionId);
                }
                else
                {
                    dataGridViewCondition.DataSource = null;
                    dataGridViewConditionDesc.DataSource = null;
                }
            }
        }

        private void dataGridViewCondition_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewCondition.SelectedRows.Count > 0)
            {
                // Get the selected row from the Condition grid
                DataGridViewRow selectedRow = dataGridViewCondition.SelectedRows[0];

                // Extract the ConditionId from the selected row
                int conditionId = Convert.ToInt32(selectedRow.Cells["ConditionId"].Value);

                // Fetch and display data for Condition descriptions
                Sequence sequence = new Sequence(server, database, username, password);
                DataTable conditionDescriptionData = sequence.LoadConditionDescriptionData(conditionId);

                // Bind the data to the description DataGridView (dataGridViewConditionDesc)
                if (conditionDescriptionData != null)
                {
                    dataGridViewConditionDesc.DataSource = conditionDescriptionData;
                }
            }

        }

        private void btnAddCondition_Click(object sender, EventArgs e)
        {
            if (dataGridViewTransition.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewTransition.SelectedRows[0];
                int transitionId = Convert.ToInt32(selectedRow.Cells["TransitionId"].Value);

                if (int.TryParse(tbxCondNumber.Text, out int conditionNumber) && conditionNumber >= 0)
                {
                    Sequence sequence = new Sequence(server, database, username, password);
                    int result = sequence.AddCondition(transitionId, conditionNumber);

                    if (result == 0) // Assume 0 means success
                    {
                        MessageBox.Show("Condition added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh the Condition DataGridView
                        LoadConditionData(transitionId);

                        // Select the first row after adding a new condition
                        if (dataGridViewCondition.Rows.Count > 0)
                        {
                            dataGridViewCondition.ClearSelection();
                            dataGridViewCondition.Rows[0].Selected = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to add condition.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid condition number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a transition to add a condition.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteCondition_Click(object sender, EventArgs e)
        {
            if (dataGridViewCondition.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewCondition.SelectedRows[0];
                int conditionId = Convert.ToInt32(selectedRow.Cells["ConditionId"].Value);

                Sequence sequence = new Sequence(server, database, username, password);
                sequence.DeleteCondition(conditionId);

                // Refresh the Condition DataGridView
                int transitionId = Convert.ToInt32(dataGridViewTransition.SelectedRows[0].Cells["TransitionId"].Value);
                LoadConditionData(transitionId);

                // Select the first row if rows exist after reload
                if (dataGridViewCondition.Rows.Count > 0)
                {
                    dataGridViewCondition.ClearSelection();
                    dataGridViewCondition.Rows[0].Selected = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a condition to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}