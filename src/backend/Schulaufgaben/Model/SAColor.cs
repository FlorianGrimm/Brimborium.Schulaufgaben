// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Model {
    [JsonDerivedType(typeof(SAColor), typeDiscriminator: "SAColor")]
    public class SAColor {
        /// <summary>
        /// An RGBA color value.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// In <see cref="SADocument"/>.DefinedColor the name is defined.
        /// If used in <see cref="SAContent"/> the value of the DefinedColor is used.
        /// </summary>
        public string? Name { get; set; }
    }
}