namespace GRPick.Attributes {

	[AttributeUsage(AttributeTargets.Enum)]
	public class SelectEnum(string? name) : Attribute {
		public string? Name { get; set; } = name;

		public string? Description { get; set; }

		public bool AutoInclude { get; set; } = true;

		public SelectEnum(string? name, string? description) : this(name) {
			Description = description;
		}

		public SelectEnum(string? name, bool autoInclude) : this(name) {
			AutoInclude = autoInclude;
		}
	}
}
