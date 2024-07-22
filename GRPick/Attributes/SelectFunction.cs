using System.Xml.Linq;

namespace GRPick.Attributes {

	[AttributeUsage(AttributeTargets.Method)]
	public class SelectFunction(string? name, Type parentType) : Attribute {
		public string? Name { get; set; } = name;

		public string? Description { get; set; }

		public bool AutoInclude { get; set; } = true;

		public Type ParentType { get; set; } = parentType;

		public SelectFunction(string? name, Type parentType, string? description) : this(name, parentType) {
			Description = description;
		}

		public SelectFunction(string? name, Type parentType, bool autoInclude) : this(name, parentType) {
			AutoInclude = autoInclude;
		}
	}
}
