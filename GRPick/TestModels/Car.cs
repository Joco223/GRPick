using GRPick.Attributes;

namespace GRPick.TestModels {

	[SelectEnum("Manufacturer")]
	public enum Manufacturer {
		Audi,
		BMW,
		Mercedes,
		Fiat
	}

	[SelectClass("Car")]
	public class Car {
		public int Id { get; set; }

		public string? Name { get; set; }

		public string? Description { get; set; } = "Defult description";

		public Manufacturer? Manufacturer { get; set; }

		public DateTime CreationDate { get; set; }

		public Engine Engine { get; set; } = new();

		public string? GetDescription(string arg1, int arg2) => Description;

		public Car(int id, string? name, string? description, Manufacturer? manufacturer, DateTime creationDate, Engine engine) {
			Id = id;
			Name = name;
			Description = description;
			Manufacturer = manufacturer;
			CreationDate = creationDate;
			Engine = engine;
		}

		public Car() {
		}
	}
}
