﻿using System;
using System.Linq;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;

namespace Entitas.Unity.CodeGenerator {
    public static class CodeGeneratorEditor {

        [MenuItem("Entitas/Generate")]
        public static void Generate() {
            var types = Assembly.GetAssembly(typeof(Entity)).GetTypes();
            var codeGenerators = GetCodeGenerators();
            var codeGeneratorNames = codeGenerators.Select(cg => cg.Name).ToArray();
            var config = new CodeGeneratorConfig(EntitasPreferencesEditor.LoadConfig(), codeGeneratorNames);

            var enabledCodeGeneratorNames = config.enabledCodeGenerators;
            var enabledCodeGenerators = codeGenerators
                .Where(type => enabledCodeGeneratorNames.Contains(type.Name))
                .Select(type => (ICodeGenerator)Activator.CreateInstance(type))
                .ToArray();

            Entitas.CodeGenerator.CodeGenerator.Generate(types, config.pools, config.generatedFolderPath, enabledCodeGenerators);

            AssetDatabase.Refresh();
        }

        public static Type[] GetCodeGenerators() {
            return Assembly.GetAssembly(typeof(ICodeGenerator)).GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(ICodeGenerator))
                    && type != typeof(ICodeGenerator)
                    && type != typeof(IPoolCodeGenerator)
                    && type != typeof(IComponentCodeGenerator)
                    && type != typeof(ISystemCodeGenerator))
                .OrderBy(type => type.FullName)
                .ToArray();
        }
    }
}
