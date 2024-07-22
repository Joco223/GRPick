using GRPick.Attributes;
using GRPick.TestModels;

namespace GRPick.TestServices {

	public class CarService {
		private readonly List<Car> cars = [
			new Car { Id = 1, Name = "A3", CreationDate = DateTime.Now, Manufacturer = Manufacturer.Audi, Engine = new Engine{ ModelName = "DADA", PowerKW = 110 } },
			new Car { Id = 2, Name = "318i", CreationDate = DateTime.Now, Manufacturer = Manufacturer.BMW, Engine = new Engine { ModelName = "S52B32", PowerKW = 179 } },
			new Car { Id = 3, Name = "CLA 180", CreationDate = DateTime.Now, Manufacturer = Manufacturer.Mercedes, Engine = new Engine { ModelName = "OM 607 DE 15 LA", PowerKW = 80 } },
			new Car { Id = 4, Name = "500L", CreationDate = DateTime.Now, Manufacturer = Manufacturer.Fiat, Engine = new Engine {ModelName = "0.9 TwinAir Natural Power", PowerKW = 59 } },
		];

		public CarService() { }

		[SelectFunction("GetCarByID", typeof(CarService))]
		[SelectQuery("GetCarByID", typeof(CarService))]
		public Car? GetCarById(int id) => cars.Where(car => car.Id == id).FirstOrDefault();

		[SelectFunction("GetAllCars", typeof(CarService))]
		[SelectQuery("GetAllCars", typeof(CarService))]
		public List<Car> GetAllCars() => cars;
	}
}

