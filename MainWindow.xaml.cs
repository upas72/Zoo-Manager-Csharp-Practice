using System;
using System.Windows;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Zoo_Manager
{
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["Zoo_Manager.Properties.Settings.ZooManagementConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            ShowZoos();
            ShowAnimals();
        }

        private void ShowZoos()
        {
            try
            {
                string query = "SELECT * FROM zoo";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);

                    listZoos.DisplayMemberPath = "location_name";
                    listZoos.SelectedValuePath = "zoo_id";
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ShowAnimals()
        {
            try
            {
                string query = "SELECT * FROM animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    listAnimals.DisplayMemberPath = "animal_name";
                    listAnimals.SelectedValuePath = "animal_id";
                    listAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ShowAssociatedAnimals()
        {
            try
            {
                if(listZoos.SelectedValue != null)
                {
                    string query = "SELECT * FROM zoo_animal za INNER JOIN animal a ON za.fk_animal_id = a.animal_id " +
                    "WHERE za.fk_zoo_id = @zooId";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    using (sqlDataAdapter)
                    {
                        sqlCommand.Parameters.AddWithValue("@zooId", listZoos.SelectedValue);

                        DataTable associatedAnimalTable = new DataTable();
                        sqlDataAdapter.Fill(associatedAnimalTable);

                        listAssociatedAnimals.DisplayMemberPath = "animal_name";
                        listAssociatedAnimals.SelectedValuePath = "zoo_animal_id";
                        listAssociatedAnimals.ItemsSource = associatedAnimalTable.DefaultView;
                    }
                }               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ListZoosSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
        }

        private void AddZooButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (genericTextBox.Text.Length > 0)
                {
                    string query = "INSERT INTO zoo VALUES (@locationName)";

                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@locationName", genericTextBox.Text);
                    sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
                genericTextBox.Text = "";
            }
        }

        private void AddAnimalButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (genericTextBox.Text.Length > 0)
                {
                    string query = "INSERT INTO animal VALUES (@animalName)";

                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@animalName", genericTextBox.Text);
                    sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
                genericTextBox.Text = "";
            }
        }

        private void AddAssociatedAnimalsButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listZoos.SelectedValue != null && listAnimals.SelectedValue != null)
                {
                    string query = "INSERT INTO zoo_animal VALUES (@zooId, @animalName)";

                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@zooId", listZoos.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@animalName", listAnimals.SelectedValue);
                    sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }

        private void DeleteZooButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listZoos.SelectedValue != null)
                {
                    string deleteZooAnimalQuery = "DELETE FROM zoo_animal WHERE fk_zoo_id = @zooId";
                    string deleteZooQuery = "DELETE FROM zoo WHERE zoo_id = @zooId";

                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(deleteZooAnimalQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@zooId", listZoos.SelectedValue);
                    sqlCommand.ExecuteScalar();
                    sqlCommand = new SqlCommand(deleteZooQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@zooId", listZoos.SelectedValue);
                    sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
                ShowZoos();
            }
        }

        private void DeleteAnimalButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listAnimals.SelectedValue != null)
                {
                    string deleteZooAnimalQuery = "DELETE FROM zoo_animal WHERE fk_animal_id = @animalId";
                    string deleteAnimalQuery = "DELETE FROM animal WHERE animal_id = @animalId";

                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(deleteZooAnimalQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@animalId", listAnimals.SelectedValue);
                    sqlCommand.ExecuteScalar();
                    sqlCommand = new SqlCommand(deleteAnimalQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@animalId", listAnimals.SelectedValue);
                    sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
                ShowAnimals();
            }
        }

        private void RemoveAnimalButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listAssociatedAnimals.SelectedValue != null)
                {
                    string deleteZooAnimalQuery = "DELETE FROM zoo_animal WHERE zoo_animal_id = @zooAnimalId";

                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(deleteZooAnimalQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@zooAnimalId", listAssociatedAnimals.SelectedValue);
                    sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }

        private void UpdateZooButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (genericTextBox.Text.Length > 0 && listZoos.SelectedValue != null)
                {
                    string query = "UPDATE zoo SET location_name = @locationName WHERE zoo_id = @zooId";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@zooId", listZoos.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@locationName", genericTextBox.Text);
                    sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
                genericTextBox.Text = "";
            }
        }

        private void UpdateAnimalButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (genericTextBox.Text.Length > 0 && listAnimals.SelectedValue != null)
                {
                    string query = "UPDATE animal SET animal_name = @animalName WHERE animal_id = @animalId";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@animalId", listAnimals.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@animalName", genericTextBox.Text);
                    sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
                ShowAnimals();
                genericTextBox.Text = "";
            }
        }
    }
}
