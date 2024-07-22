namespace GRPick.GRPick.Models
{
	public class Query
	{
		public string QueryName { get; set; } = string.Empty;

		public List<string> Parameters { get; set; } = [];

		public List<string> Properties { get; set; } = [];

		public bool UseEnumNames { get; set; } = false;

		public Query() { }

		public Query(string queryName, List<string> parameters, List<string> properties, bool useEnumNames)
		{
			QueryName = queryName ?? throw new ArgumentNullException(nameof(queryName));
			Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
			Properties = properties ?? throw new ArgumentNullException(nameof(properties));
			UseEnumNames = useEnumNames;
		}
	}
}
