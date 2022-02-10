﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using IdokladSDK.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace IdokladSdk.SourceGenerators
{
    [Generator]
    public class RequiredFieldsSourceGenerator : ISourceGenerator
    {
        private const string ConstantGeneratedFileName = "Constants.g.cs";
        private const string RequiredAttributeName = "Required";
        private const string ValidableModelClassName = "ValidatableModel";
        private const string ConstantsClassName = "Constants";

        public void Execute(GeneratorExecutionContext context)
        {
            var @namespace = GetNamespace(context);
            var validableModels = GetAllValidatableModels(context);
            var requiredPropertiesPerModel = GetRequiredPropertiesForModels(validableModels);
            var generatedClass = GenerateConstantClass(requiredPropertiesPerModel, @namespace);
            context.AddSource(ConstantGeneratedFileName, SourceText.From(generatedClass, Encoding.UTF8));
        }

        private static string GetNamespace(GeneratorExecutionContext context)
        {
            IEnumerable<SyntaxNode> nodes = context.Compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());
            return nodes
                .Where(node => node.IsKind(SyntaxKind.ClassDeclaration))
                .OfType<ClassDeclarationSyntax>()
                .Where(node => node.Identifier.Text == ConstantsClassName)
                .Select(node => node.Parent)
                .OfType<NamespaceDeclarationSyntax>()
                .Select(@namespace => @namespace.Name)
                .OfType<IdentifierNameSyntax>()
                .Select(identifier => identifier.Identifier.Text)
                .FirstOrDefault();
        }

        private static string GenerateConstantClass(List<KeyValuePair<string, IEnumerable<string>>> requiredPropertiesPerModel, string @namespace)
        {
            var builder = new StringBuilder(@"
using System.Collections.Generic;
using System;

namespace ");
            builder.Append(@namespace);
            builder.Append(@"
{
    public static partial class Constants
    {
        /// <summary>
        /// Dictionary with required field for all models
        /// </summary>
        public static Dictionary<Type, IEnumerable<string>> RequiredFields = new Dictionary<Type, IEnumerable<string>>
        {
");
            foreach (var route in requiredPropertiesPerModel)
            {
                var values = string.Join(",", route.Value.Select(item => $"\"{item}\""));
                builder.AppendLine($" {{Type.GetType(\"{route.Key}\"),new List<string>(){{{values}}} }},");
            }
            builder.Append(@"
        };
    }
}");
            return builder.ToString();
        }

        private static List<KeyValuePair<string, IEnumerable<string>>> GetRequiredPropertiesForModels(IEnumerable<ClassDeclarationSyntax> validableModels)
        {
            return validableModels
                            .Select(model =>
                            {
                                var properties = model.ChildNodes()
                                .Where(childNode => childNode.IsKind(SyntaxKind.PropertyDeclaration))
                                .Cast<PropertyDeclarationSyntax>()
                                .Where(childNode => childNode.AttributeLists
                                    .Any(childNodeAttribute => childNodeAttribute.Attributes
                                        .Any(attribute => attribute.Name.ToString() == RequiredAttributeName)))
                                .Select(childNode => childNode.Identifier.Text)
                                .ToList();
                                return new KeyValuePair<string, IEnumerable<string>>(model.GetFullName(), properties);
                            })
                            .Where(modelProperties => modelProperties.Value.Any())
                            .ToList();
        }

        private static IEnumerable<ClassDeclarationSyntax> GetAllValidatableModels(GeneratorExecutionContext context)
        {
            IEnumerable<SyntaxNode> nodes = context.Compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());
            IEnumerable<ClassDeclarationSyntax> models = nodes
                .Where(node => node.IsKind(SyntaxKind.ClassDeclaration))
                .OfType<ClassDeclarationSyntax>()
                .Where(node => node.BaseList?.Types.Any(baseNode =>
                {
                    if (baseNode.Type.IsKind(SyntaxKind.IdentifierName))
                    {
                        return ((IdentifierNameSyntax)baseNode.Type).Identifier.Text == ValidableModelClassName;
                    }
                    return false;

                }) == true);
            return models;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
