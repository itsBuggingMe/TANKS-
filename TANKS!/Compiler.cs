using System;
using System.CodeDom.Compiler;
using System.Reflection;


namespace TANKS_
{
    public static class Compiler
    {
        private static CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        public static bool TryCompileCode(string code, string outputDllPath)
        {
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                OutputAssembly = outputDllPath
            };

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);
            return !results.Errors.HasErrors;
        }
    }
}
