namespace GRPick.GRPick.Models
{
	public class Function
	{
		public string FunctionName { get; set; } = string.Empty;

		public List<string> Parameters { get; set; } = [];

		public Function() { }

		public Function(string functionName, List<string> parameters)
		{
			FunctionName = functionName ?? throw new ArgumentNullException(nameof(functionName));
			Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
		}
	}
}
