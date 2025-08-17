using Newtonsoft.Json.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace PowerShellWebFunction.Functions {
    public abstract class RunnableFunctionBase {
        public RunnableFunctionBase() { }
        public RunnableFunctionBase(string name, string source)
        {
            Name = name;
            Source = source;
        }

        public string? Name { get; internal set; }
        public string? Description { get; internal set; }

        private string? _source;
        public string? Source {
            get => _source;
            internal set => _source = value;
        }

        public Type? ParametersType { get; internal set; }

        private ScriptBlock? _script;
        public ScriptBlock? Script { get  {
                if (_script == default(ScriptBlock) && !string.IsNullOrEmpty(_source))
                {
                    _script = ScriptBlock.Create(_source);
                }
                return _script;
            }
        }
        public ParamBlockAst? ParamBlock { get {
                dynamic? ast = Script?.Ast;
                return ast?.ParamBlock;
            }
        }

        public abstract string Run(dynamic parameters);

        public object GetFunctionHelp()
        {
            Dictionary<string, object> help = new Dictionary<string, object>();
            help["Name"] = Name;
            help["Description"] = Description ?? "No description provided.";
            help["ParameterTemplate"] = Activator.CreateInstance(ParametersType ?? typeof(object));
            return help;
        }
    }
}
