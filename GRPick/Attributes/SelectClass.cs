namespace GRPick.Attributes {

	[AttributeUsage(AttributeTargets.Class)]
	public class SelectClass(string? name) : Attribute {

		public string? Name { get; set; } = name;

		public string? Description { get; set; }

		public bool AutoInclude { get; set; } = true;

		public SelectClass(string? name, string? description) : this(name) {
			Description = description;
		}

		public SelectClass(string? name, bool autoInclude) : this(name) {
			AutoInclude = autoInclude;
		}
	}
}
