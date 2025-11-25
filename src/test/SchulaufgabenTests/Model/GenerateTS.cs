// MIT - Florian Grimm

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Brimborium.Schulaufgaben.Model;

public class GenerateTS {
    [Test]
    public async Task GenTS() {
        Type[] listType = [
            typeof(SAMediaSearchRequest),
            typeof(SAMediaInfo),
            typeof(SADocumentDescription),
            typeof(SADocument),
            typeof(SANode),
            typeof(SAContent),
            typeof(SAMedia),
            typeof(SAText),
            typeof(SAImage),
            typeof(SAVideo),
            typeof(SAAudio),
            typeof(SABorder),
            typeof(SABox),
            typeof(SAScalarUnit),
            typeof(SAUnit),
            typeof(SAColor),
            ];
        List<KlassInfo> listKlassInfo = new();
        {
            listKlassInfo.Add(new(typeof(string)) { TSName = "string", IsNative = true });
            listKlassInfo.Add(new(typeof(Guid)) { TSName = "string", IsNative = true });
            listKlassInfo.Add(new(typeof(Single)) { TSName = "number", IsNative = true });
            listKlassInfo.Add(new(typeof(bool)) { TSName = "boolean", IsNative = true });
            foreach (Type typeClass in listType) {
                var klassInfo = new KlassInfo(typeClass);
                listKlassInfo.Add(klassInfo);
            }
            foreach (KlassInfo klassInfo in listKlassInfo) {
                if (klassInfo.IsNative) { continue; }
                var typeClass = klassInfo.Type;
                if (typeClass.IsClass) {
                    foreach (var propertyInfo in typeClass.GetProperties()) {
                        var propInfo = klassInfo.AddProperty(propertyInfo);
                        await Assert.That(propInfo.ValueType).IsNotNull().Because($"{klassInfo.Type.Name}.{propInfo.Name}");
                        var valueKlassInfo = listKlassInfo.FirstOrDefault(k => k.Type.Equals(propInfo.ValueType));
                        await Assert.That(valueKlassInfo).IsNotNull().Because($"{propInfo.ValueType.Name} : {klassInfo.Type.Name}.{propInfo.Name}");
                    }
                }
                if (typeClass.IsEnum) {
                    var listValues = System.Enum.GetValuesAsUnderlyingType(typeClass);
                    Dictionary<string, string> listEnum = new();
                    foreach (var value in listValues) {
                        var name = System.Enum.GetName(typeClass, value);
                        if (name is string { Length: > 0 }
                            && value.ToString() is string { Length: > 0 } txt) {
                            listEnum.Add(name, txt);
                        }
                    }
                    klassInfo.ListEnum = listEnum;
                }
            }
            foreach (var klassInfo in listKlassInfo) {
                if (klassInfo.IsNative) { continue; }
                if (klassInfo.ListEnum is { }) { continue; }
                foreach (var propInfo in klassInfo.ListProp) {
                    var valueType = propInfo.ValueType;
                    var propInfoKlassInfo = listKlassInfo.FirstOrDefault(k => k.Type.Equals(valueType));
                    await Assert.That(propInfoKlassInfo).IsNotNull();
                    if (propInfo.IsArray) {
                        propInfo.TSValueType = propInfoKlassInfo.TSName + "[]";
                    } else {
                        propInfo.TSValueType = propInfoKlassInfo.TSName;
                    }
                }
            }
        }
        {
            StringBuilder sbOutput = new();
            foreach (var klassInfo in listKlassInfo) {
                if (klassInfo.IsNative) { continue; }
                if (klassInfo.ListEnum is { } listEnum) {
                    sbOutput.AppendLine($"export enum {klassInfo.TSName} {{");
                    foreach (var valueEnum in klassInfo.ListEnum) {
                        sbOutput.AppendLine($"    {valueEnum.Key} = {valueEnum.Value},");
                    }
                    sbOutput.AppendLine("}");
                    sbOutput.AppendLine("");
                } else {
                    sbOutput.AppendLine($"export type {klassInfo.TSName} = {{");
                    foreach (var propInfo in klassInfo.ListProp) {
                        sbOutput.AppendLine($"    {propInfo.Name} : {propInfo.TSValueType};");
                    }
                    sbOutput.AppendLine("}");
                    sbOutput.AppendLine("");
                }
            }
            await Verify(sbOutput.ToString());
        }
    }
    private class KlassInfo {
        public KlassInfo(Type type) {
            this.Type = type;
            this.TSName = type.Name;
        }
        public Type Type { get; }
        public string TSName { get; set; }
        public bool IsNative { get; set; }
        public Dictionary<string, string>? ListEnum { get; set; }
        public List<PropInfo> ListProp { get; } = new();

        internal PropInfo AddProperty(PropertyInfo propertyInfo) {
            var propertyType = propertyInfo.PropertyType;
            Type? listItemType = null;
            if (propertyType.IsArray) {
                listItemType = propertyType.GenericTypeArguments[0];
            } else if (propertyType.IsGenericType
                && propertyType.GetGenericTypeDefinition().Equals(typeof(List<>))) {
                listItemType = propertyType.GenericTypeArguments[0];
            }
            if (listItemType is { }) {
                PropInfo propInfo = new(
                    name: propertyInfo.Name,
                    propertyInfo: propertyInfo,
                    ValueType: listItemType,
                    isArray: true);
                this.ListProp.Add(propInfo);
                return propInfo;
            }

            {
                PropInfo propInfo = new(
                    name: propertyInfo.Name,
                    propertyInfo: propertyInfo,
                    ValueType: propertyType,
                    isArray: false);
                this.ListProp.Add(propInfo);
                return propInfo;
            }
        }
    }
    private class PropInfo {
        public PropInfo(
            string name,
            PropertyInfo propertyInfo,
            Type ValueType,
            bool isArray) {
            this.Name = name;
            this.ValueType = ValueType;
            this.IsArray = isArray;
        }

        public string Name { get; }
        public Type ValueType { get; }
        public bool IsArray { get; }
        public string TSValueType { get; set; } = "";
    }
}
