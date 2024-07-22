namespace GRPick.TestModels {
	public class Engine {
		public string? ModelName { get; set; }

		public int PowerKW { get; set; }

		public Engine(string? modelName, int powerKW) {
			ModelName = modelName;
			PowerKW = powerKW;
		}

		public Engine() {
		}
	}
}
