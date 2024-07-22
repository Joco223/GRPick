using GRPick.Attributes;
using GRPick.TestModels;
using GRPick.TestRepositories;

namespace GRPick.TestServices {

	public class CarService {
		private CarRepository carRepository = CarRepository.Instance;

		public CarService() { }

		[SelectQuery("GetCarByID", typeof(CarService))]
		public Car? GetCarById(int id) => carRepository.GetCarById(id);

		[SelectQuery("GetAllCars", typeof(CarService))]
		public List<Car> GetAllCars() => carRepository.GetAllCars();

		[SelectFunction("UpdateCarName", typeof(CarService))]
		public Car? UpdateCarName(int id, string name) => carRepository.UpdateCarName(id, name);
	}
}

