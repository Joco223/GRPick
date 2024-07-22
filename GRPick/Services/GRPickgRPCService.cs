using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

using Grpc.Core;
using GRPick.Attributes;
using GRPick.GRPick;
using GRPick.GRPick.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace GRPick.Services
{
    public class GRPickgRPCService : GRPickResponder.GRPickResponderBase {
		private readonly GRPickService pickService = GRPickService.Instance;

		public override Task<DataResponse> QueryData(QueryRequest request, ServerCallContext context) {
			var response = new DataResponse();

			Query query = new(request.QueryName, request.Paramterers.ToList(), request.Properties.ToList(), request.UseEnumNames);
			var result = pickService.HandleQuery(query);

			if (!result.Item1) {
				response.Failed = true;
				response.Message = result.Item2;
			} else {
				response.Failed = false;
				response.DataJSON = result.Item2;
			}

			return Task.FromResult(response);
		}

		public override Task<DataResponse> CallFunction(DataRequest request, ServerCallContext context) {
			var response = new DataResponse();

			GRPick.Models.Function function = new(request.FunctionName, request.Paramterers.ToList());

			var result = pickService.HandleFunction(function);

			if (!result.Item1) {
				response.Failed = true;
				response.Message = result.Item2;
			} else {
				response.Failed = false;
				response.DataJSON = result.Item2;
			}

			return Task.FromResult(response);
		}

		public override Task<GetFunctionsResponse> GetAllFunctions(GetFunctions request, ServerCallContext context) {
			var response = new GetFunctionsResponse();

			foreach (var function in pickService.Methods) {
				var pickFunction = new Function {
					Name = function.Name,
				};

				// Arguments
				foreach (var argument in function.GetParameters()) {
					var pickArgument = new FunctionArgument {
						Name = argument.Name,
						Type = argument.ParameterType.ToString()
					};
					pickFunction.Arguments.Add(pickArgument);
				}

				response.Functions.Add(pickFunction);
			}

			return Task.FromResult(response);
		}

		public override Task<GetObjectsResponse> GetAllObjects(GetObjects request, ServerCallContext context) {
			var response = new GetObjectsResponse();

			foreach (var objectType in pickService.Types) {
				var pickObject = new Object {
					Name = objectType.Name
				};

				// Properties
				foreach (var property in objectType.GetProperties()) {
					var pickProperty = new Property {
						Name = property.Name,
						Type = property.PropertyType.ToString()
					};
					pickObject.Properties.Add(pickProperty);
				}

				// Functions
				foreach (var function in objectType.GetMethods()) {
					var pickFunction = new Function {
						Name = function.Name,
					};

					// Arguments
					foreach (var argument in function.GetParameters()) {
						var pickArgument = new FunctionArgument {
							Name = argument.Name,
							Type = argument.ParameterType.ToString()
						};
						pickFunction.Arguments.Add(pickArgument);
					}

					pickObject.Methods.Add(pickFunction);
				}

				response.Objects.Add(pickObject);
			}

			return Task.FromResult(response);
		}
	}
}
