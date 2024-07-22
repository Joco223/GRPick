using System.Reflection;

internal static class GRPickServiceHelpers {
	public static IEnumerable<Type> GetClassesWithCustomAttribute<TAttribute>()
	where TAttribute : Attribute {
		var assembly = Assembly.GetExecutingAssembly();
		return assembly.GetTypes()
			.Where(type => type.GetCustomAttributes(typeof(TAttribute), false).Length > 0);
	}

	public static IEnumerable<FieldInfo> GetEnumsWithCustomAttribute<TAttribute>()
	where TAttribute : Attribute {
		var assembly = Assembly.GetExecutingAssembly();
		return assembly.GetTypes()
			.Where(type => type.IsEnum)
			.SelectMany(type => type.GetFields())
			.Where(field => field.GetCustomAttributes(typeof(TAttribute), false).Length > 0);
	}

	public static MethodInfo[] GetMethodsWithCustomAttribute<TAttribute>()
		where TAttribute : Attribute {
		var assembly = Assembly.GetExecutingAssembly(); // Get the current assembly

		return assembly.GetTypes() // Get all types in the assembly
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance)) // Get public instance methods
			.Where(method => method.GetCustomAttributes(typeof(TAttribute), false).Length > 0) // Filter by attribute
			.ToArray(); // Convert to array
	}
}