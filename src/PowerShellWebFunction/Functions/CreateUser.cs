using Microsoft.CodeAnalysis.CSharp.Syntax;
using SMM.Automation;

namespace PowerShellWebFunction.Functions {
    public class ExampleCreateUser : RunnableFunctionBase {
        public ExampleCreateUser()
        {
            Name = "ExampleCreateUser";
            Source = @"param($username, $email) Write-Host Creating user: $username with address: $email";
            Description = "Example function that could actually create a user account somewhere.";
            ParametersType = typeof(CreateUserRequest);
        }

        public override string Run(dynamic parameters)
        {
            string _resultString = string.Empty;
            if (string.IsNullOrEmpty(Source))
            {
                throw new ArgumentException("Source script cannot be null or empty.");
            }

            CreateUserRequest? request = parameters as CreateUserRequest;
            using (SimpleScriptRunner runner = new SimpleScriptRunner(Source))
            {
                if (request != null)
                {
                    runner.Parameters.Add("username", request.UserName ?? "NULL_USERNAME");
                    runner.Parameters.Add("email", request.Email ?? "NULL_EMAIL");
                }
                else
                {
                    throw new ArgumentException("Parameters must be of type CreateUserRequest.");
                }
                try
                {
                    runner.Run();
                    _resultString = runner.Results.FirstOrDefault()?.ToString() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error running script: " + ex.Message, ex);
                }
            }
            return _resultString;
        }
    }

    public class CreateUserRequest {
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
