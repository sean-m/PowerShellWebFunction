using PowerShellWebFunction.Functions;

namespace PowerShellWebFunction {
    public class FunctionCollection {
        public FunctionCollection() { }


        public List<RunnableFunctionBase> Functions { get; set; } = new List<RunnableFunctionBase>();

        public void AddFunction(RunnableFunctionBase function)
        {
            Functions.Add(function);
        }

        public RunnableFunctionBase? GetFunction(string name)
        {
            return Functions.FirstOrDefault(f => f.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);
        }
    }
}
