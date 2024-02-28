using Microsoft.Win32;
using MotorcycleGameWpf.Model;
using MotorcycleGameWpf.Persistence;
using MotorcycleGameWpf.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MotorcycleGameWpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private MainWindow _mainWindow;
		private MotorcycleModel _model;
		private MotorcycleViewModel _viewModel;

		public App()
		{
			_mainWindow = new MainWindow();
			_model = new MotorcycleModel(new DataAccess());
			_viewModel = new MotorcycleViewModel(_model);
			Startup += App_Startup;
		}

		private void App_Startup(object sender, StartupEventArgs e)
		{
			_model.GameOver += OnGameOver;
			_mainWindow.DataContext = _viewModel;
			_viewModel.SaveGame += new EventHandler(OnSaveGame);
			_viewModel.LoadGame += new EventHandler(OnLoadGame);
			_mainWindow.Show();
			
		}

		private void OnGameOver(object? sender, EventArguments.GameOverEventArgs e)
		{
			MessageBox.Show($"Game over! Collapsed time: {_model.IntToTime(e.Time)}", "Motorcycle Game");
		}

		private async void OnLoadGame(object? sender, EventArgs e)
		{
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Title = "MotorcycleGame game loading";
				openFileDialog.Filter = "MotorcycleGame Game|*.txt";
				if (openFileDialog.ShowDialog() == true)
				{
					await _model.LoadGame(openFileDialog.FileName);
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Error while loading game!", "MotorcycleGame", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			
		}

		private async void OnSaveGame(object? sender, EventArgs e)
		{
			try
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Title = "MotorcycleGame saving";
				saveFileDialog.Filter = "MotorcycleGame|*.txt";
				if (saveFileDialog.ShowDialog() == true)
				{
					try
					{
						await _model.SaveGame(saveFileDialog.FileName);
					}
					catch (Exception)
					{
						MessageBox.Show("Error while saving game!", "Motorcycle", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			catch
			{
				MessageBox.Show("Error while saving game!", "Motorcycle", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
