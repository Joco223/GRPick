using System.Reflection;
using GRPick.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Serilog;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using GRPick.GRPick.Models;

namespace GRPick.GRPick
{
    public class GRPickService
	{
		public static GRPickService Instance { get; private set; } = new();

		public List<Type> Types { get; set; } = [];
		public List<Type> Enums { get; set; } = [];
		public List<MethodInfo> Methods { get; set; } = [];
		public List<MethodInfo> Queries { get; set; } = [];

		public GRPickService()
		{
			LoadData();
		}

		public (bool, string) HandleFunction(Models.Function function) {
			var functionToCall = Methods.Where(x => x.Name.Equals(function.FunctionName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

			if (functionToCall is null)
				return (false, $"No function {function.FunctionName} found");

			// Check parameters
			List<object?> parameters = [];
			var targetParameters = functionToCall.GetParameters();

			if (targetParameters.Length != function.Parameters.Count) {
				return (false, $"Incorrect number of paramters passed, {targetParameters.Length} required");
			}

			// Go trough all provided paramteres, try to cast them into saved query types, throw error if it failed
			for (int index = 0; index < targetParameters.Length; index++) {
				try {
					parameters.Add(Convert.ChangeType(function.Parameters[index], targetParameters[index].ParameterType));
				} catch (Exception) {
					return (false, $"Failed converting paramterer < {function.Parameters[index]} > to type {targetParameters[index].ParameterType}");
				}
			}

			// Get the SelectQuery attribute from the requested query
			var selectQueryAttribute = functionToCall.GetCustomAttribute(typeof(SelectFunction), true);

			if (selectQueryAttribute is null)
				return (false, "Error getting attribute");

			// Find parent type of the query
			var parentType = ((SelectFunction)selectQueryAttribute).ParentType;

			// Make an instance of the parent type
			var classInstance = parentType.GetConstructors().First().Invoke(null);

			// Call query function with the parent type generated
			var data = functionToCall.Invoke(classInstance, [.. parameters]);

			if (data is null)
				return (false, "Error loading data from function");

			return (true, JsonConvert.SerializeObject(data));
		}

		public (bool, string) HandleQuery(Query query) {
			var queryToSearch = Queries.Where(x => x.Name.Equals(query.QueryName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

			if (queryToSearch is null)
				return (false, $"No query {query.QueryName} found");

			// Check parameters
			List<object?> parameters = [];
			var targetParameters = queryToSearch.GetParameters();

			if (targetParameters.Length != query.Parameters.Count) {
				return (false, $"Incorrect number of paramters passed, {targetParameters.Length} required");
			}

			// Go trough all provided paramteres, try to cast them into saved query types, throw error if it failed
			for (int index = 0; index < targetParameters.Length; index++) { 
				try {
					parameters.Add(Convert.ChangeType(query.Parameters[index], targetParameters[index].ParameterType));
				} catch (Exception) {
					return (false, $"Failed converting paramterer < {query.Parameters[index]} > to type {targetParameters[index].ParameterType}");
				}
			}

			// Get the SelectQuery attribute from the requested query
			var selectQueryAttribute = queryToSearch.GetCustomAttribute(typeof(SelectQuery), true);

			if (selectQueryAttribute is null)
				return (false, "Error getting attribute");

			// Find parent type of the query
			var parentType = ((SelectQuery)selectQueryAttribute).ParentType;

			// Make an instance of the parent type
			var classInstance = parentType.GetConstructors().First().Invoke(null);

			// Call query function with the parent type generated
			var data = queryToSearch.Invoke(classInstance, [.. parameters]);

			if (data is null)
				return (false, "Error loading data from query");

			// Replace enum values with their text counterparts if requested
			var settings = new JsonSerializerSettings();

			if (query.UseEnumNames)
				settings.Converters.Add(new StringEnumConverter());

			if (query.Properties.Count == 0)
				return (true, JsonConvert.SerializeObject(data, settings));

			// Remove unwanted properties, if none are provided, don't remove anything
			var emptiedData = RemoveProperties(JObject.FromObject(data), [.. query.Properties]);

			return (true, JsonConvert.SerializeObject(emptiedData, settings));
		}

		private static JObject RemoveProperties(JObject data, List<string> properties, string currentPath = "") {
			if (properties.Count == 0)
				return data;

			List<string> propertiesToRemove = [];

			// Go trough each provided property
			foreach (var property in data.Properties()) {
				string propertyPath = $"{currentPath}.{property.Name}"; // Construct path to property

				if (string.IsNullOrEmpty(currentPath)) {
					propertyPath = property.Name;
				}

				// If property is an object and not primitive type, call function recurisvely
				if (property.Value.Type == JTokenType.Object) {
					RemoveProperties((JObject)property.Value, properties, propertyPath);
				} else {
					// If the current property doesn't exist in provided properties, add into list for removal
					if (!properties.Any(p => propertyPath.StartsWith(p, StringComparison.OrdinalIgnoreCase))) {
						propertiesToRemove.Add(property.Name);
					}
				}
			}

			// Remove all saved properties
			foreach (var property in propertiesToRemove) {
				data.Remove(property);
			}

			return data;
		}

		private void LoadData() {
			Log.Debug($"[GRPick][DEBUG] =========================================================== Scanning attributes ===========================================================");
			Log.Debug("[GRPick][DEBUG] ======================== Scanning classes ========================");
			// Classes
			var selectedClasses = GRPickServiceHelpers.GetClassesWithCustomAttribute<SelectClass>();

			foreach (var selectedClass in selectedClasses) {
				var properties = selectedClass.GetProperties();

				foreach (var property in properties) {
					Log.Debug($"[GRPick][SelectObject][{selectedClass.Name}], property: {property.Name}");
				}

				var Methods = selectedClass.GetMethods();

				foreach (var method in Methods) {
					var methodParameters = method.GetParameters();
					Log.Debug($"[GRPick][SelectObject][{selectedClass.Name}], method: {method.Name}");

					foreach (var parameter in methodParameters) {
						Log.Debug($"[GRPick][SelectObject][{selectedClass.Name}], method parameter name: {parameter.Name}, method parameter type: {parameter.ParameterType.Name}");
					}
				}

				Types.Add(selectedClass);
			}

			Log.Debug("[GRPick][DEBUG] ======================== Scanning Enums ========================");

			// Enums
			var selectedEnums = GRPickServiceHelpers.GetClassesWithCustomAttribute<SelectEnum>();

			foreach (var selectedEnum in selectedEnums) {
				Array enumValues = Enum.GetValues(selectedEnum);

				foreach (var value in enumValues) {
					Log.Debug($"[GRPick][SelectEnum][{selectedEnum.Name}], value: {value}");
				}

				Enums.Add(selectedEnum);
			}

			Log.Debug("[GRPick][DEBUG] ======================== Scanning functions ========================");

			// Functions
			var selectedFuncs = GRPickServiceHelpers.GetMethodsWithCustomAttribute<SelectFunction>();

			foreach (var selectedFunc in selectedFuncs) {
				var methodParameters = selectedFunc.GetParameters();
				Log.Debug($"[GRPick][SelectFunction][{selectedFunc.Name}], return type: {selectedFunc.ReturnType}");

				foreach (var parameter in methodParameters) {
					Log.Debug($"[GRPick][SelectFunction][{selectedFunc.Name}], method parameter name: {parameter.Name}, method parameter type: {parameter.ParameterType.Name}");
				}

				Methods.Add(selectedFunc);
			}

			// Queries
			var selectedQueries = GRPickServiceHelpers.GetMethodsWithCustomAttribute<SelectQuery>();

			foreach (var selectedQuery in selectedQueries) {
				var methodParameters = selectedQuery.GetParameters();
				Log.Debug($"[GRPick][SelectQuery][{selectedQuery.Name}], return type: {selectedQuery.ReturnType}");

				foreach (var parameter in methodParameters) {
					Log.Debug($"[GRPick][SelectQuery][{selectedQuery.Name}], method parameter name: {parameter.Name}, method parameter type: {parameter.ParameterType.Name}");
				}

				Queries.Add(selectedQuery);
			}
		}
	}
}
