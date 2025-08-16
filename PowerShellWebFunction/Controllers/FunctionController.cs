using Microsoft.AspNetCore.Mvc;
using PowerShellWebFunction.Functions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerShellWebFunction.Controllers {
    [Route("v1/function/")]
    [Route("v1/selfservice/")]
    [ApiController]
    public class FunctionController : ControllerBase {

        private readonly FunctionCollection Functions;

        public FunctionController(FunctionCollection functions)
        {
            Functions = functions ?? throw new ArgumentNullException(nameof(functions));
        }

        public string? FunctionName { get; set; }

        // GET: api/<FunctionController>
        [HttpGet("{functionName}")]
        public IActionResult Get(string? functionName)
        {
            FunctionName = functionName;


            if (string.IsNullOrEmpty(functionName) || functionName.Trim().Equals("help", StringComparison.CurrentCultureIgnoreCase))
            {
                var functionNames = Functions.Functions.Select(f => f.Name).ToList();
                var response = new
                {
                    Functions = functionNames,
                    Message = "No function name provided. Available functions are listed."
                };
                return Ok(response);
            }

            RunnableFunctionBase function = Functions.GetFunction(functionName) ??
                                           throw new ArgumentException($"Function '{functionName}' not found.");

            dynamic template = function.GetFunctionHelp();
            return Ok(template);
        }

        // POST api/<FunctionController>
        [HttpPost("{functionName}")]
        public IActionResult Post(string functionName, [FromBody] Dictionary<string,string> value)
        {
            FunctionName = functionName;

            RunnableFunctionBase function = Functions.GetFunction(functionName) ??
                                           throw new ArgumentException($"Function '{functionName}' not found.");

            if (function == null) return NotFound($"Function '{functionName}' not found.");
            if (value == null || value.Count == 0)
            {
                return BadRequest("No parameters provided.");
            }

            try
            {
                Type paramType = function.ParametersType ?? typeof(object);
                if (paramType == null) throw new HttpRequestException($"Function '{functionName}' does not have a defined parameter type.");

                string paramString = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                dynamic parameters = Newtonsoft.Json.JsonConvert.DeserializeObject(paramString, paramType);
                string result = function.Run(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error executing function '{functionName}': {ex.Message}");
            }

            Ok();
        }
    }
}
