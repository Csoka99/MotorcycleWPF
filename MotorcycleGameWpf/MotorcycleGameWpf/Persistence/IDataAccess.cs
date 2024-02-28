using MotorcycleGameWpf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MotorcycleGameWpf.Model.States;

namespace MotorcycleGameWpf.Persistence
{
	public interface IDataAccess
	{
		Task<(int, int, int, int, FieldState[,])> LoadGameAsync(string path);
		public Task<bool> SaveGameAsync(string path, MotorcycleModel model);
	}
}
