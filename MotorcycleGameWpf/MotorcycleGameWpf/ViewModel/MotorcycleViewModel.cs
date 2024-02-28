using MotorcycleGameWpf.EventArguments;
using MotorcycleGameWpf.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static MotorcycleGameWpf.Model.States;

namespace MotorcycleGameWpf.ViewModel
{
	public class MotorcycleViewModel : ViewModelBase
	{

		#region Fields

		private MotorcycleModel _model;
		private bool _pause;
		private bool _start;
		private DispatcherTimer _timer;

		#endregion

		#region Properties
		public int Size { get => _model.Size; set { OnPropertyChanged(); } }
		public int FuelTank { get => _model.FuelTank; set { OnPropertyChanged(); } }
		public string Time { get => _model.IntToTime(_model.Time); set { OnPropertyChanged(); } }
		public bool Pause { get => _pause; set { _pause = value; OnPropertyChanged(); } }
		public bool Start { get => _start; set { _start = value; OnPropertyChanged(); } }
		#endregion

		#region ObservableCollections
		public ObservableCollection<FieldViewModel> Fields { get; set; }

		#endregion

		#region DelegateCommands
		public DelegateCommand MoveMotorCommand { get; set; }
		public DelegateCommand NewGameCommand { get; set; }
		public DelegateCommand StartCommand { get; set; }
		public DelegateCommand PauseCommand { get; set; }
		public DelegateCommand SaveCommand { get; set; }
		public DelegateCommand LoadCommand { get; set; }
		#endregion

		#region EventHandlers
		public event EventHandler? LoadGame;
		public event EventHandler? SaveGame;
		#endregion
		public MotorcycleViewModel(MotorcycleModel model)
		{
			_model = model;
			_model.GameStarted += OnGameStarted;
			_model.FieldChanged += OnFieldChanged;
			_model.GameOver += OnGameFinished;

			_timer = new DispatcherTimer();
			_timer.Interval = TimeSpan.FromMilliseconds(1000);
			_timer.Tick += OnTimerTicked;

			Fields = new ObservableCollection<FieldViewModel>();

			MoveMotorCommand = new DelegateCommand(param => KeyDown(Convert.ToString(param)));
			StartCommand = new DelegateCommand(param => OnStartClicked());
			PauseCommand = new DelegateCommand(param => OnPauseClicked());
			NewGameCommand = new DelegateCommand(param => _model.StartNewGame(13));
			SaveCommand = new DelegateCommand(param => OnSaveGame());
			LoadCommand = new DelegateCommand(param => OnLoadGame());


			_model.StartNewGame(13);
		}

		private void OnStartClicked()
		{
			_timer.Start();
			Pause = true;
			Start = false;
		}
		private void OnPauseClicked()
		{
			_timer.Stop();
			Pause = false;
			Start = true;
		}
		private void KeyDown(string? key)
		{
			if(_timer.IsEnabled && key is not null)
			{
				KeyState keyValue = _model.StringToKeyState(key);
				_model.KeyPressed(keyValue);
			}
		}
		private void OnTimerTicked(object? sender, EventArgs e)
		{
			_model.TimerTicked();
			FuelTank = _model.FuelTank;
			Time = _model.IntToTime(_model.Time);
			_timer.Interval = TimeSpan.FromMilliseconds(_model.Speed);
		}
		private void OnGameFinished(object? sender, EventArguments.GameOverEventArgs e)
		{
			_timer.Stop();
			Start = true;
			Pause = false;
		}
		private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
		{
			var field = Fields.FirstOrDefault(field => field.RowIndex == e.Row && field.ColumnIndex == e.Column);
			if (field != null) {
				field.Color = CalculateColor(e.NewState);
			}
		}
		private void OnGameStarted(object? sender, GameStartedEventArgs e)
		{
			_timer.Stop();
			Pause = false;
			Start = true;
			FuelTank = _model.FuelTank;
			Time = _model.IntToTime(_model.Time);
			Fields.Clear();

			for (int i = 0; i < Size; i++)
			{
				for (int j = 0; j < Size; j++)
				{
					Fields.Add(new FieldViewModel(CalculateColor(e.Board[i,j]),i,j));
				}
			}

		}
		private string CalculateColor(FieldState field)
		{
			switch (field)
			{
				case FieldState.Empty:
					return "white";
				case FieldState.Fuel:
					return "red";
				case FieldState.Motor:
					return "black";
				default:
					return "blue";
			}
		}
		private void OnSaveGame()
		{
			if (SaveGame is not null)
			{
				SaveGame(this, EventArgs.Empty);
			}
		}
		private void OnLoadGame()
		{
			if (LoadGame is not null)
			{
				LoadGame(this, EventArgs.Empty);
			}
		}
	}
}
